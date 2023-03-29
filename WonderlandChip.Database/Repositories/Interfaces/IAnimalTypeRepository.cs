using System.Threading.Tasks;
using WonderlandChip.Database.DTO.AnimalType;

namespace WonderlandChip.Database.Repositories.Interfaces
{
    public interface IAnimalTypeRepository
    {
        public Task<AnimalTypeDTO> CreateAnimalType(string? type);
        public Task<AnimalTypeDTO?> GetAnimalTypeById(long? id);
        public Task<AnimalTypeDTO?> UpdateAnimalTypeById(AnimalTypeDTO? animal);
        public Task<long?> DeleteAnimalTypeById(long? id);
    }
}
