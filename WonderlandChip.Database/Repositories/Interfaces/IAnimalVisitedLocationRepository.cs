using System.Collections.Generic;
using System.Threading.Tasks;
using WonderlandChip.Database.DTO.AnimalVisitedLocation;

namespace WonderlandChip.Database.Repositories.Interfaces
{
    public interface IAnimalVisitedLocationRepository
    {
        public Task<List<AnimalVisitedLocationGetDTO>?> SearchVisitedLocation(AnimalVisitedLocationSearchDTO visitedLocation);
        public Task<AnimalVisitedLocationGetDTO?> AddVisitedLocation(AnimalVisitedLocationAddDTO addInfo);
        public Task<AnimalVisitedLocationGetDTO?> UpdateVisitedLocation(AnimalVisitedLocationUpdateDTO updateInfo);
        public Task<long?> DeleteVisitedLocation(AnimalVisitedLocationDeleteDTO deleteInfo);
    }
}
