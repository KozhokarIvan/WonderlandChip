using System.Threading.Tasks;
using WonderlandChip.Database.DTO.LocationPoint;

namespace WonderlandChip.Database.Repositories.Interfaces
{
    public interface ILocationPointRepository
    {
        public Task<LocationPointWithIdDTO?> GetLocationById(long? id);
        public Task<LocationPointWithIdDTO> CreateLocation(LocationPointDTO location);
        public Task<LocationPointWithIdDTO?> UpdateLocation(LocationPointWithIdDTO location);
        public Task<long?> DeleteLocation(long? locationId);
    }
}
