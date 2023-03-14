using System.Threading.Tasks;
using WonderlandChip.Database.DTO.AnimalType;

namespace WonderlandChip.Database.Repositories.Interfaces
{
    public interface IAnimalTypeRepository
    {
        public Task<AnimalTypeGetDTO> GetAnimalTypeById(long id);
    }
}
