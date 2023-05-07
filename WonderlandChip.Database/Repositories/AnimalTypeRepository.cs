using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WonderlandChip.Database.CustomExceptions;
using WonderlandChip.Database.DbContexts;
using WonderlandChip.Database.DTO.AnimalType;
using WonderlandChip.Database.Models;
using WonderlandChip.Database.Repositories.Interfaces;

namespace WonderlandChip.Database.Repositories
{
    public class AnimalTypeRepository : IAnimalTypeRepository
    {
        private readonly ChipizationDbContext _dbContext;
        public AnimalTypeRepository(ChipizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AnimalTypeDTO> CreateAnimalType(string type)
        {
            bool doesTypeExist = await _dbContext.AnimalTypes.AnyAsync(at => at.Name == type);
            if (doesTypeExist)
                throw new AnimalTypeAlreadyExistsException();
            AnimalType animalType = new AnimalType() { Name = type };
            _dbContext.AnimalTypes.Add(animalType);
            await _dbContext.SaveChangesAsync();
            return new AnimalTypeDTO() { Id = animalType.Id, Type = animalType.Name! };
        }

        public async Task<AnimalTypeDTO?> GetAnimalTypeById(long id)
        {
            AnimalType? dbAnimalType = await _dbContext.AnimalTypes.FindAsync(id);
            if (dbAnimalType == null) return null;
            AnimalTypeDTO returnAnimalType = new AnimalTypeDTO()
            {
                Id = dbAnimalType.Id,
                Type = dbAnimalType.Name
            };
            return returnAnimalType;
        }

        public async Task<AnimalTypeDTO?> UpdateAnimalTypeById(AnimalTypeDTO? animalTypeInfo)
        {
            bool doesTypeExist = await _dbContext.AnimalTypes
                .AnyAsync(at => at.Name == animalTypeInfo.Type);
            if (doesTypeExist)
                throw new AnimalTypeAlreadyExistsException();
            AnimalType? dbAnimalType = await _dbContext.AnimalTypes.FindAsync(animalTypeInfo!.Id);
            if (dbAnimalType is null) return null;
            dbAnimalType.Name = animalTypeInfo!.Type;
            await _dbContext.SaveChangesAsync();
            return new AnimalTypeDTO() { Id = dbAnimalType.Id, Type = dbAnimalType.Name };
        }

        public async Task<long?> DeleteAnimalTypeById(long id)
        {
            AnimalType? dbAnimalType = await _dbContext.AnimalTypes.FindAsync(id);
            if (dbAnimalType is null) return null;
            await _dbContext.Entry(dbAnimalType)
                .Collection(at => at.Animals)
                .LoadAsync();
            if (dbAnimalType.Animals?.Count > 0) throw new TypeIsBoundToAnimalException();
            _dbContext.AnimalTypes.Remove(dbAnimalType);
            await _dbContext.SaveChangesAsync();
            return dbAnimalType.Id;
        }
    }
}
