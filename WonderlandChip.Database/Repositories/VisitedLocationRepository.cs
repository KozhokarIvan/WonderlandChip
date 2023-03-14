using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WonderlandChip.Database.DbContexts;
using WonderlandChip.Database.DTO.AnimalVisitedLocation;
using WonderlandChip.Database.Models;
using WonderlandChip.Database.Repositories.Interfaces;

namespace WonderlandChip.Database.Repositories
{
    public class VisitedLocationRepository : IVisitedLocationRepository
    {
        private readonly ChipizationDbContext _dbContext;
        public VisitedLocationRepository(ChipizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<AnimalVisitedLocationGetDTO>> SearchVisitedLocation(AnimalVisitedLocationSearchDTO visitedLocation)
        {
            List<AnimalVisitedLocation> foundLocations = await _dbContext.AnimalsVisitedLocations
                .Where(vl =>
                (!visitedLocation.StartDateTime.HasValue || vl.DateTimeOfVisit >= visitedLocation.StartDateTime) &&
                (!visitedLocation.EndDateTime.HasValue || vl.DateTimeOfVisit <= visitedLocation.EndDateTime)
                )
                .OrderBy(l => l.DateTimeOfVisit)
                .Skip(visitedLocation.From)
                .Take(visitedLocation.Size)
                .ToListAsync();
            List<AnimalVisitedLocationGetDTO> returnLocations =
                foundLocations.Select(l => new AnimalVisitedLocationGetDTO()
                {
                    Id = l.AnimalId,
                    DateTimeOfVisitLocationPoint = l.DateTimeOfVisit,
                    LocationPointId = l.LocationPointId
                }).ToList();
            return returnLocations;
        }
    }
}
