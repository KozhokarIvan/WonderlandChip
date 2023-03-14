using System.Threading.Tasks;
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
        public async Task<AnimalTypeGetDTO> GetAnimalTypeById(long id)
        {
            AnimalType foundAnimalType = await _dbContext.AnimalTypes.FindAsync(id);
            if (foundAnimalType == null) return null;
            AnimalTypeGetDTO returnAnimalType = new AnimalTypeGetDTO()
            {
                Id = foundAnimalType.Id,
                Type = foundAnimalType.Name
            };
            return returnAnimalType;
        }
    }
}
