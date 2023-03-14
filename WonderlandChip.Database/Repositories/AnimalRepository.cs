using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public async Task<AnimalGetDTO> GetAnimalById(long id)
        {
            Animal foundAnimal = await _dbContext.Animals.FindAsync(id);
            if (foundAnimal == null) return null;
            AnimalGetDTO returnAnimal = new AnimalGetDTO()
            {
                Id = foundAnimal.Id,
                AnimalTypes = foundAnimal.AnimalTypes.Select(at => at.Id).ToArray(),
                Weight = foundAnimal.Weight,
                Length = foundAnimal.Length,
                Height = foundAnimal.Height,
                Gender = foundAnimal.Gender,
                LifeStatus = foundAnimal.LifeStatus,
                ChippingDateTime = foundAnimal.ChippingDateTime,
                ChipperId = foundAnimal.ChipperId,
                ChippingLocationId = foundAnimal.ChippingLocationId,
                VisitedLocations = foundAnimal.VisitedLocations.Select(vl => vl.AnimalId).ToArray(),
                DeathDateTime = foundAnimal.DeathDateTime
            };
            return returnAnimal;
        }

        public async Task<List<AnimalGetDTO>> SearchAnimal(AnimalSearchDTO animal)
        {
            List<Animal> foundAnimals = await _dbContext.Animals
                .Where(a =>
                (!animal.StartDateTime.HasValue || a.ChippingDateTime >= animal.StartDateTime) &&
                (!animal.EndDateTime.HasValue || a.ChippingDateTime <= animal.EndDateTime) &&
                (!animal.ChipperId.HasValue || a.ChipperId == animal.ChipperId) &&
                (!animal.ChippingLocationId.HasValue || a.ChippingLocationId == animal.ChippingLocationId) &&
                (!string.IsNullOrWhiteSpace(animal.LifeStatus) || a.LifeStatus == animal.LifeStatus) &&
                (!string.IsNullOrWhiteSpace(animal.Gender) || a.Gender == animal.Gender)
                )
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
                VisitedLocations = a.VisitedLocations.Select(vl => vl.AnimalId).ToArray(),
                DeathDateTime = a.DeathDateTime
            }).ToList();
            return returnAnimals;
        }
    }
}
