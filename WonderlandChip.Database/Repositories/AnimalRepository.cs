using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WonderlandChip.Database.CustomExceptions;
using WonderlandChip.Database.DbContexts;
using WonderlandChip.Database.DTO.Animal;
using WonderlandChip.Database.Models;
using WonderlandChip.Database.Repositories.Interfaces;

namespace WonderlandChip.Database.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ChipizationDbContext _dbContext;
        public AnimalRepository(ChipizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AnimalGetDTO> GetAnimalById(long? id)
        {
            Animal foundAnimal = await _dbContext.Animals.FindAsync(id);
            if (foundAnimal is null) return null;
            var entry = _dbContext.Entry(foundAnimal);
            await entry.Collection(e => e.AnimalTypes).LoadAsync();
            await entry.Collection(e => e.VisitedLocations).LoadAsync();
            AnimalGetDTO returnAnimal = new AnimalGetDTO()
            {
                Id = foundAnimal.Id,
                AnimalTypes = foundAnimal!.AnimalTypes!.Select(at => at.Id).ToArray(),
                Weight = foundAnimal.Weight,
                Length = foundAnimal.Length,
                Height = foundAnimal.Height,
                Gender = foundAnimal.Gender,
                LifeStatus = foundAnimal.LifeStatus,
                ChippingDateTime = foundAnimal.ChippingDateTime,
                ChipperId = foundAnimal.ChipperId,
                ChippingLocationId = foundAnimal.ChippingLocationId,
                VisitedLocations = foundAnimal.VisitedLocations?.Select(vl => vl.Id).ToArray(),
                DeathDateTime = foundAnimal.DeathDateTime
            };
            return returnAnimal;
        }
        public async Task<List<AnimalGetDTO>> SearchAnimals(AnimalSearchDTO animal)
        {
            List<Animal> foundAnimals = await _dbContext.Animals
                .Where(a =>
                (!animal.StartDateTime.HasValue || a.ChippingDateTime >= animal.StartDateTime) &&
                (!animal.EndDateTime.HasValue || a.ChippingDateTime <= animal.EndDateTime) &&
                (!animal.ChipperId.HasValue || a.ChipperId == animal.ChipperId) &&
                (!animal.ChippingLocationId.HasValue || a.ChippingLocationId == animal.ChippingLocationId) &&
                (string.IsNullOrWhiteSpace(animal.LifeStatus) || a.LifeStatus == animal.LifeStatus) &&
                (string.IsNullOrWhiteSpace(animal.Gender) || a.Gender == animal.Gender)
                )
                .Include(a => a.AnimalTypes)
                .Include(a => a.VisitedLocations)
                .OrderBy(a => a.Id)
                .Skip(animal.From)
                .Take(animal.Size)
                .ToListAsync();
            List<AnimalGetDTO> returnAnimals = foundAnimals.Select(a => new AnimalGetDTO()
            {
                Id = a.Id,
                AnimalTypes = a.AnimalTypes.Select(at => at.Id).ToArray(),
                Weight = a.Weight,
                Length = a.Length,
                Height = a.Height,
                Gender = a.Gender,
                LifeStatus = a.LifeStatus,
                ChippingDateTime = a.ChippingDateTime,
                ChipperId = a.ChipperId,
                ChippingLocationId = a.ChippingLocationId,
                VisitedLocations = a.VisitedLocations.Select(vl => vl.Id).ToArray(),
                DeathDateTime = a.DeathDateTime
            }).ToList();
            return returnAnimals;
        }
        public async Task<AnimalGetDTO> AddAnimal(AnimalCreateDTO animal)
        {
            bool isChipperFound = await _dbContext.Accounts
                .AnyAsync(a => a.Id == animal.ChipperId);
            if (!isChipperFound) return null;

            bool doesChippingLocationExist = await _dbContext.LocationPoints
                .AnyAsync(lp => lp.Id == animal.ChippingLocationId);
            if (!doesChippingLocationExist) return null;
            bool doesAnimalTypesHaveDuplicates = animal.AnimalTypes
                .GroupBy(at => at).Any(g => g.Count() > 1);
            if (doesAnimalTypesHaveDuplicates)
                throw new AnimalTypesHasDuplicatesException();
            List<AnimalType> animalTypes = await _dbContext.AnimalTypes
                .Where(at => animal.AnimalTypes.Contains(at.Id))
                .ToListAsync();
            bool isAllTypesFound = animalTypes.Count == animal.AnimalTypes.Length;
            if (!isAllTypesFound) return null;
            Animal? dbAnimal = new Animal()
            {
                AnimalTypes = animalTypes,
                Weight = animal.Weight ?? throw new NullReferenceException(),
                Length = animal.Length ?? throw new NullReferenceException(),
                Height = animal.Height ?? throw new NullReferenceException(),
                Gender = animal.Gender ?? throw new NullReferenceException(),
                LifeStatus = "ALIVE",
                ChippingDateTime = DateTimeOffset.UtcNow,
                ChipperId = animal.ChipperId ?? throw new NullReferenceException(),
                ChippingLocationId = animal.ChippingLocationId ?? throw new NullReferenceException()
            };
            await _dbContext.Animals.AddAsync(dbAnimal);
            await _dbContext.SaveChangesAsync();
            return new AnimalGetDTO()
            {
                Id = dbAnimal.Id,
                AnimalTypes = dbAnimal.AnimalTypes.Select(at => at.Id).ToArray() ?? new long[0],
                Weight = dbAnimal.Weight,
                Length = dbAnimal.Length,
                Height = dbAnimal.Height,
                Gender = dbAnimal.Gender,
                LifeStatus = dbAnimal.LifeStatus,
                ChippingDateTime = dbAnimal.ChippingDateTime,
                ChipperId = dbAnimal.ChipperId,
                ChippingLocationId = dbAnimal.ChippingLocationId,
                VisitedLocations = dbAnimal.VisitedLocations?.Select(vl => vl.Id).ToArray() ?? new long[0],
                DeathDateTime = dbAnimal.DeathDateTime
            };
        }

        public async Task<AnimalGetDTO> UpdateAnimal(AnimalUpdateDTO animal)
        {
            bool isChipperFound = await _dbContext.Accounts
                .AnyAsync(a => a.Id == animal.ChipperId);
            if (!isChipperFound) return null;
            bool doesChippingLocationExist = await _dbContext.LocationPoints
                .AnyAsync(lp => lp.Id == animal.ChippingLocationId);
            if (!doesChippingLocationExist) return null;
            Animal? dbAnimal = await _dbContext.Animals.FindAsync(animal.AnimalId);
            if (dbAnimal is null) return null;
            if (dbAnimal.LifeStatus == "DEAD" && animal.LifeStatus == "ALIVE")
                throw new AnimalResurrectingException();
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.AnimalTypes)
                .LoadAsync();
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.VisitedLocations)
                .LoadAsync();
            if (dbAnimal.VisitedLocations?.Count > 0 &&
                animal.ChippingLocationId == dbAnimal.VisitedLocations?.First().LocationPointId)
                throw new NewLocationIsFirstException();
            dbAnimal.Weight = animal.Weight ?? throw new NullReferenceException();
            dbAnimal.Length = animal.Length ?? throw new NullReferenceException();
            dbAnimal.Height = animal.Height ?? throw new NullReferenceException();
            dbAnimal.LifeStatus = animal.LifeStatus ?? throw new NullReferenceException();
            if (dbAnimal.LifeStatus == "DEAD")
                dbAnimal.DeathDateTime = DateTimeOffset.UtcNow;
            dbAnimal.Gender = animal.Gender;
            dbAnimal.ChipperId = animal.ChipperId ?? throw new NullReferenceException();
            dbAnimal.ChippingLocationId = animal.ChippingLocationId ?? throw new NullReferenceException();
            await _dbContext.SaveChangesAsync();
            return new AnimalGetDTO()
            {
                Id = dbAnimal.Id,
                AnimalTypes = dbAnimal.AnimalTypes?.Select(at => at.Id).ToArray(),
                Weight = dbAnimal.Weight,
                Length = dbAnimal.Length,
                Height = dbAnimal.Height,
                Gender = dbAnimal.Gender,
                LifeStatus = dbAnimal.LifeStatus,
                ChippingDateTime = dbAnimal.ChippingDateTime,
                ChipperId = dbAnimal.ChipperId,
                ChippingLocationId = dbAnimal.ChippingLocationId,
                VisitedLocations = dbAnimal.VisitedLocations?.Select(at => at.Id).ToArray(),
                DeathDateTime = dbAnimal.DeathDateTime
            };
        }
        public async Task<long?> DeleteAnimal(long? id)
        {
            Animal? dbAnimal = await _dbContext.Animals.FindAsync(id);

            if (dbAnimal == null) return null;
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.VisitedLocations)
                .LoadAsync();
            if (dbAnimal.VisitedLocations?.Count > 0)
                throw new AnimalHasLocationPointsException();
            _dbContext.Remove(dbAnimal);
            await _dbContext.SaveChangesAsync();
            return dbAnimal.Id;
        }

        public async Task<AnimalGetDTO> AddAnimalType(AnimalCreateTypeDTO addTypeInfo)
        {
            AnimalType? dbAnimalType = await _dbContext.AnimalTypes
                .FindAsync(addTypeInfo.TypeId);
            if (dbAnimalType is null)
                return null;
            Animal? dbAnimal = await _dbContext.Animals.FindAsync(addTypeInfo.AnimalId);
            if (dbAnimal is null)
                return null;
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.AnimalTypes)
                .LoadAsync();
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.VisitedLocations)
                .LoadAsync();
            if (dbAnimal.AnimalTypes.Contains(dbAnimalType)) throw new AnimalAlreadyHasTypeException();
            dbAnimal.AnimalTypes.Add(dbAnimalType);
            await _dbContext.SaveChangesAsync();
            return new AnimalGetDTO()
            {
                Id = dbAnimal.Id,
                AnimalTypes = dbAnimal.AnimalTypes.Select(at => at.Id).ToArray(),
                Weight = dbAnimal.Weight,
                Length = dbAnimal.Length,
                Height = dbAnimal.Height,
                LifeStatus = dbAnimal.LifeStatus,
                Gender = dbAnimal.Gender,
                ChippingDateTime = dbAnimal.ChippingDateTime,
                ChipperId = dbAnimal.ChipperId,
                ChippingLocationId = dbAnimal.ChippingLocationId,
                VisitedLocations = dbAnimal.VisitedLocations?.Select(at => at.Id).ToArray() ?? new long[0],
                DeathDateTime = dbAnimal.DeathDateTime
            };
        }

        public async Task<AnimalGetDTO> UpdateAnimalType(AnimalUpdateTypeDTO updateTypeInfo)
        {
            AnimalType? oldType = await _dbContext.AnimalTypes.FindAsync(updateTypeInfo.OldTypeId);
            if (oldType is null) return null;
            AnimalType? newType = await _dbContext.AnimalTypes.FindAsync(updateTypeInfo.NewTypeId);
            if (newType is null) return null;
            Animal? dbAnimal = await _dbContext.Animals.FindAsync(updateTypeInfo.AnimalId);
            if (dbAnimal is null) return null;
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.AnimalTypes)
                .LoadAsync();
            if (!dbAnimal.AnimalTypes.Contains(oldType)) return null;
            if (dbAnimal.AnimalTypes.Contains(newType)) throw new AnimalAlreadyHasTypeException();
            dbAnimal.AnimalTypes.Remove(oldType);
            dbAnimal.AnimalTypes.Add(newType);
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.VisitedLocations)
                .LoadAsync();
            await _dbContext.SaveChangesAsync();
            return new AnimalGetDTO()
            {
                Id = dbAnimal.Id,
                AnimalTypes = dbAnimal.AnimalTypes.Select(at => at.Id).ToArray(),
                Weight = dbAnimal.Weight,
                Length = dbAnimal.Length,
                Height = dbAnimal.Height,
                LifeStatus = dbAnimal.LifeStatus,
                Gender = dbAnimal.Gender,
                ChippingDateTime = dbAnimal.ChippingDateTime,
                ChipperId = dbAnimal.ChipperId,
                ChippingLocationId = dbAnimal.ChippingLocationId,
                VisitedLocations = dbAnimal.VisitedLocations.Select(at => at.Id).ToArray(),
                DeathDateTime = dbAnimal.DeathDateTime
            };
        }
        public async Task<AnimalGetDTO> DeleteAnimalType(AnimalDeleteTypeDTO deleteTypeInfo)
        {
            AnimalType? dbAnimalType = await _dbContext.AnimalTypes
                .FindAsync(deleteTypeInfo.TypeId);
            if (dbAnimalType is null) return null;
            Animal? dbAnimal = await _dbContext.Animals.FindAsync(deleteTypeInfo.AnimalId);
            if (dbAnimal is null) return null;
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.AnimalTypes)
                .LoadAsync();
            if (!dbAnimal.AnimalTypes.Contains(dbAnimalType)) return null;
            if (dbAnimal.AnimalTypes.Count <= 1) throw new AnimalWontHaveTypesException();
            dbAnimal.AnimalTypes.Remove(dbAnimalType);
            await _dbContext.SaveChangesAsync();
            return new AnimalGetDTO()
            {
                Id = dbAnimal.Id,
                AnimalTypes = dbAnimal.AnimalTypes.Select(at => at.Id).ToArray(),
                Weight = dbAnimal.Weight,
                Length = dbAnimal.Length,
                Height = dbAnimal.Height,
                LifeStatus = dbAnimal.LifeStatus,
                ChippingDateTime = dbAnimal.ChippingDateTime,
                ChipperId = dbAnimal.ChipperId,
                ChippingLocationId = dbAnimal.ChippingLocationId,
                VisitedLocations = dbAnimal.VisitedLocations?.Select(at => at.Id).ToArray(),
                DeathDateTime = dbAnimal.DeathDateTime
            };
        }
    }
}
