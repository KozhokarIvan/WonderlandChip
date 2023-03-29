using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WonderlandChip.Database.CustomExceptions;
using WonderlandChip.Database.DbContexts;
using WonderlandChip.Database.DTO.LocationPoint;
using WonderlandChip.Database.Models;
using WonderlandChip.Database.Repositories.Interfaces;

namespace WonderlandChip.Database.Repositories
{
    public class LocationPointRepository : ILocationPointRepository
    {
        private readonly ChipizationDbContext _dbContext;
        public LocationPointRepository(ChipizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LocationPointWithIdDTO> CreateLocation(LocationPointDTO location)
        {
            bool doesPointExist = await _dbContext.LocationPoints.AnyAsync(l =>
            l.Latitude == location.Latitude && l.Longitude == location.Longitude);
            if (doesPointExist)
                throw new PointAlreadyExistsException();
            LocationPoint locationPoint = new LocationPoint()
            {
                Latitude = location.Latitude ?? throw new NullReferenceException(),
                Longitude = location.Longitude ?? throw new NullReferenceException()
            };
            await _dbContext.LocationPoints.AddAsync(locationPoint);
            await _dbContext.SaveChangesAsync();
            return new LocationPointWithIdDTO()
            {
                Id = locationPoint.Id,
                Latitude = locationPoint.Latitude,
                Longitude = locationPoint.Longitude
            };
        }


        public async Task<LocationPointWithIdDTO> GetLocationById(long? id)
        {
            LocationPoint dbLocation = await _dbContext.LocationPoints.FindAsync(id);
            if (dbLocation is null) return null;
            LocationPointWithIdDTO returnLocation = new LocationPointWithIdDTO()
            {
                Id = dbLocation.Id,
                Latitude = dbLocation.Latitude,
                Longitude = dbLocation.Longitude
            };
            return returnLocation;
        }

        public async Task<LocationPointWithIdDTO> UpdateLocation(LocationPointWithIdDTO location)
        {
            bool doesPointExist = await _dbContext.LocationPoints.AnyAsync(l =>
            l.Latitude == location.Latitude && l.Longitude == location.Longitude);
            if (doesPointExist)
                throw new PointAlreadyExistsException();
            LocationPoint? dbLocation = await _dbContext.LocationPoints.FindAsync(location.Id);
            if (dbLocation is null) return null;
            dbLocation.Latitude = location.Latitude;
            dbLocation.Longitude = location.Longitude;
            await _dbContext.SaveChangesAsync();
            return new LocationPointWithIdDTO()
            {
                Id = dbLocation.Id,
                Latitude = dbLocation.Latitude,
                Longitude = dbLocation.Longitude
            };
        }
        public async Task<long?> DeleteLocation(long? locationId)
        {
            LocationPoint dbLocation = await _dbContext.LocationPoints.FindAsync(locationId);
            if (dbLocation is null) return null;
            await _dbContext
                .Entry(dbLocation)
                .Collection(l => l.AnimalVisitedLocations)
                .LoadAsync();
            bool isTheLocationChippingPoint = await _dbContext.Animals
                .Where(a => a.ChippingLocationId == locationId)
                .AnyAsync();
            if (dbLocation.AnimalVisitedLocations?.Count > 0 || isTheLocationChippingPoint) throw new PointIsReferencedException();
            _dbContext.LocationPoints.Remove(dbLocation);
            await _dbContext.SaveChangesAsync();
            return dbLocation.Id;
        }
    }
}
