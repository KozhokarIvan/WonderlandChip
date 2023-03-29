using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WonderlandChip.Database.CustomExceptions;
using WonderlandChip.Database.DbContexts;
using WonderlandChip.Database.DTO.AnimalVisitedLocation;
using WonderlandChip.Database.Models;
using WonderlandChip.Database.Repositories.Interfaces;

namespace WonderlandChip.Database.Repositories
{
    public class AnimalVisitedLocationRepository : IAnimalVisitedLocationRepository
    {
        private readonly ChipizationDbContext _dbContext;
        public AnimalVisitedLocationRepository(ChipizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AnimalVisitedLocationGetDTO> AddVisitedLocation(AnimalVisitedLocationAddDTO addInfo)
        {
            Animal? dbAnimal = await _dbContext.Animals.FindAsync(addInfo.AnimalId);
            if (dbAnimal is null) return null;
            if (dbAnimal.LifeStatus == "DEAD") throw new AnimalIsDeadException();
            LocationPoint? dbLocation = await _dbContext.LocationPoints.FindAsync(addInfo.PointId);
            if (dbLocation is null) return null;
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.VisitedLocations)
                .LoadAsync();
            if (dbAnimal.VisitedLocations?.Count <= 0 && dbAnimal.ChippingLocationId == addInfo.PointId ||
                dbAnimal.VisitedLocations?.Count > 0 && dbAnimal.VisitedLocations?.Last().LocationPointId == addInfo.PointId)
                throw new AnimalIsAlreadyHereException();
            AnimalVisitedLocation animalVisitedLocation = new AnimalVisitedLocation()
            {
                Animal = dbAnimal,
                LocationPoint = dbLocation,
                DateTimeOfVisit = DateTimeOffset.UtcNow
            };
            await _dbContext.AnimalsVisitedLocations.AddAsync(animalVisitedLocation);
            await _dbContext.SaveChangesAsync();
            return new AnimalVisitedLocationGetDTO()
            {
                Id = animalVisitedLocation.Id,
                DateTimeOfVisitLocationPoint = animalVisitedLocation.DateTimeOfVisit,
                LocationPointId = animalVisitedLocation.LocationPointId
            };

        }

        public async Task<AnimalVisitedLocationGetDTO> UpdateVisitedLocation(AnimalVisitedLocationUpdateDTO updateInfo)
        {
            Animal dbAnimal = await _dbContext.Animals.FindAsync(updateInfo.AnimalId);
            if (dbAnimal is null) return null;
            AnimalVisitedLocation? dbAnimalVisitedLocation = await _dbContext.AnimalsVisitedLocations
                .FindAsync(updateInfo.VisitedLocationPointId);
            if (dbAnimalVisitedLocation is null) return null;
            if (dbAnimalVisitedLocation.LocationPointId == updateInfo.LocationPointId)
                throw new AnimalLocationPointIsTheSameException();
            LocationPoint dbLocation = await _dbContext.LocationPoints.FindAsync(updateInfo.LocationPointId);
            if (dbLocation is null) return null;
            List<AnimalVisitedLocation> visitedLocations = await _dbContext.AnimalsVisitedLocations
                .Where(vl => vl.AnimalId == dbAnimal.Id)
                .OrderBy(vl => vl.Id)
                .ToListAsync();
            if (visitedLocations is null ||
                !visitedLocations
                .Any(a => a.Id == updateInfo.VisitedLocationPointId))
                return null;
            visitedLocations = visitedLocations
                .OrderBy(vl => vl.Id)
                .ToList();
            if (visitedLocations.Count > 0 &&
                dbAnimal.ChippingLocationId == updateInfo.LocationPointId &&
                visitedLocations.First().Id == updateInfo.VisitedLocationPointId)
                throw new FirstLocationMatchesChippingLocationException();
            int indexOfEntry = visitedLocations
                .FindIndex(a => a.LocationPointId == dbAnimalVisitedLocation.LocationPointId);
            if (visitedLocations.Count > 1 &&
                indexOfEntry - 1 >= 0 &&
                updateInfo.LocationPointId ==
                visitedLocations[indexOfEntry - 1].LocationPointId)
                throw new AnimalLocationPointIsTheSameException();
            if (visitedLocations.Count > 1 &&
                indexOfEntry + 1 < visitedLocations.Count &&
                updateInfo.LocationPointId ==
                visitedLocations[indexOfEntry + 1].LocationPointId)
                throw new AnimalLocationPointIsTheSameException();
            dbAnimalVisitedLocation.LocationPointId = updateInfo.LocationPointId ?? throw new NullReferenceException();
            await _dbContext.SaveChangesAsync();
            return new AnimalVisitedLocationGetDTO()
            {
                Id = dbAnimalVisitedLocation.Id,
                DateTimeOfVisitLocationPoint = dbAnimalVisitedLocation.DateTimeOfVisit,
                LocationPointId = dbAnimalVisitedLocation.LocationPoint.Id
            };

        }
        public async Task<long?> DeleteVisitedLocation(AnimalVisitedLocationDeleteDTO deleteInfo)
        {
            Animal? dbAnimal = await _dbContext.Animals.FindAsync(deleteInfo.AnimalId);
            if (dbAnimal is null) return null;
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.VisitedLocations)
                .LoadAsync();
            AnimalVisitedLocation? dbAnimalVisitedLocation = await _dbContext.AnimalsVisitedLocations
                .FindAsync(deleteInfo.VisitedPointId);
            if (dbAnimalVisitedLocation is null) return null;
            if (!dbAnimal.VisitedLocations.Contains(dbAnimalVisitedLocation)) return null;
            _dbContext.AnimalsVisitedLocations.Remove(dbAnimalVisitedLocation);
            await _dbContext.SaveChangesAsync();
            if (dbAnimal.VisitedLocations.Count > 0 &&
                dbAnimal.VisitedLocations.First().LocationPointId == dbAnimal.ChippingLocationId)
                _dbContext.AnimalsVisitedLocations.Remove(dbAnimal.VisitedLocations.First());
            await _dbContext.SaveChangesAsync();
            return dbAnimalVisitedLocation.Id;
        }

        public async Task<List<AnimalVisitedLocationGetDTO>> SearchVisitedLocation(AnimalVisitedLocationSearchDTO visitedLocation)
        {
            Animal dbAnimal = await _dbContext.Animals.FindAsync(visitedLocation.AnimalId);
            if (dbAnimal is null) return null;
            await _dbContext
                .Entry(dbAnimal)
                .Collection(a => a.VisitedLocations)
                .LoadAsync();
            List<AnimalVisitedLocation> foundLocations = dbAnimal.VisitedLocations
                .Where(vl =>
                (!visitedLocation.StartDateTime.HasValue || vl.DateTimeOfVisit >= visitedLocation.StartDateTime) &&
                (!visitedLocation.EndDateTime.HasValue || vl.DateTimeOfVisit <= visitedLocation.EndDateTime)
                )
                .OrderBy(l => l.DateTimeOfVisit)
                .Skip(visitedLocation.From)
                .Take(visitedLocation.Size)
                .ToList();
            List<AnimalVisitedLocationGetDTO> returnLocations =
                foundLocations.Select(l => new AnimalVisitedLocationGetDTO()
                {
                    Id = l.Id,
                    DateTimeOfVisitLocationPoint = l.DateTimeOfVisit,
                    LocationPointId = l.LocationPointId
                }).ToList();
            return returnLocations;
        }

    }
}
