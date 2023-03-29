using System.Collections.Generic;
using System.Threading.Tasks;
using WonderlandChip.Database.DTO.Animal;

namespace WonderlandChip.Database.Repositories.Interfaces
{
    public interface IAnimalRepository
    {
        public Task<AnimalGetDTO?> AddAnimal(AnimalCreateDTO animal);
        public Task<AnimalGetDTO?> GetAnimalById(long? id);
        public Task<List<AnimalGetDTO>?> SearchAnimals(AnimalSearchDTO animal);
        public Task<AnimalGetDTO?> UpdateAnimal(AnimalUpdateDTO animal);
        public Task<long?> DeleteAnimal(long? id);
        public Task<AnimalGetDTO?> AddAnimalType(AnimalCreateTypeDTO addTypeInfo);
        public Task<AnimalGetDTO?> UpdateAnimalType(AnimalUpdateTypeDTO updateTypeInfo);
        public Task<AnimalGetDTO?> DeleteAnimalType(AnimalDeleteTypeDTO deleteTypeInfo);
    }
}
