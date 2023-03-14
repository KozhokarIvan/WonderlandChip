using System.Threading.Tasks;
using WonderlandChip.Database.DbContexts;
using WonderlandChip.Database.DTO.LocationPoint;
using WonderlandChip.Database.Models;
using WonderlandChip.Database.Repositories.Interfaces;

namespace WonderlandChip.Database.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ChipizationDbContext _dbContext;
        public LocationRepository(ChipizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<LocationPointGetDTO> GetLocationById(long id)
        {
            LocationPoint foundLocation = await _dbContext.LocationPoints.FindAsync(id);
            if (foundLocation == null) return null;
            LocationPointGetDTO returnLocation = new LocationPointGetDTO()
            {
                Id = foundLocation.Id,
                Latitude = foundLocation.Latitude,
                Longitude = foundLocation.Longitude
            };
            return returnLocation;
        }
    }
}
