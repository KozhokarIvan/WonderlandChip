using System;
using System.Collections.Generic;

namespace WonderlandChip.Database.Models
{
    public class Animal
    {
        public long Id { get; set; }
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; } = null!;
        public string LifeStatus { get; set; } = null!;
        public DateTimeOffset ChippingDateTime { get; set; }
        public int ChipperId { get; set; }
        public long ChippingLocationId { get; set; }
        public DateTimeOffset? DeathDateTime { get; set; }
        public Account Chipper { get; set; } = null!;
        public LocationPoint ChippingLocation { get; set; } = null!;
        public List<AnimalType>? AnimalTypes { get; set; }
        public List<AnimalVisitedLocation>? VisitedLocations { get; set; }
    }
}
