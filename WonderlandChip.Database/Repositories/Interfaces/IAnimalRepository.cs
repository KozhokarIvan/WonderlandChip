using System.Collections.Generic;
using System.Threading.Tasks;
using WonderlandChip.Database.DTO.Animal;

namespace WonderlandChip.Database.Repositories.Interfaces
{
    public interface IAnimalRepository
    {
        public Task<AnimalGetDTO> GetAnimalById(long id);
        public Task<List<AnimalGetDTO>> SearchAnimal(AnimalSearchDTO animal);
    }
}
