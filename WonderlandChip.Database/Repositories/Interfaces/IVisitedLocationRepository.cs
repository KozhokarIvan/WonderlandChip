using System.Collections.Generic;
using System.Threading.Tasks;
using WonderlandChip.Database.DTO.AnimalVisitedLocation;

namespace WonderlandChip.Database.Repositories.Interfaces
{
    public interface IVisitedLocationRepository
    {
        public Task<List<AnimalVisitedLocationGetDTO>> SearchVisitedLocation(AnimalVisitedLocationSearchDTO visitedLocation);
    }
}
