using System.Collections.Generic;

namespace WonderlandChip.Database.Models
{
    public class LocationPoint
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<AnimalVisitedLocation>? AnimalVisitedLocations { get; set; }
    }
}
