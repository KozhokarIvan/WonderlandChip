using System.Threading.Tasks;
using WonderlandChip.Database.DTO.LocationPoint;

namespace WonderlandChip.Database.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        public Task<LocationPointGetDTO> GetLocationById(long id);
    }
}
