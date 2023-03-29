using System;

namespace WonderlandChip.Database.DTO.Animal
{
    public class AnimalGetDTO
    {
        public long Id { get; set; }
        public long[]? AnimalTypes { get; set; }
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; } = null!;
        public string LifeStatus { get; set; } = null!;
        public DateTimeOffset ChippingDateTime { get; set; }
        public int ChipperId { get; set; }
        public long ChippingLocationId { get; set; }
        public long[]? VisitedLocations { get; set; }
        public DateTimeOffset? DeathDateTime { get; set; }

    }
}
