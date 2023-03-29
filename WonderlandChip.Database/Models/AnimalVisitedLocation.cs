using System;

namespace WonderlandChip.Database.Models
{
    public class AnimalVisitedLocation
    {
        public long Id { get; set; }
        public DateTimeOffset DateTimeOfVisit { get; set; }
        public long AnimalId { get; set; }
        public long LocationPointId { get; set; }
        public Animal Animal { get; set; } = null!;
        public LocationPoint LocationPoint { get; set; } = null!;
    }
}