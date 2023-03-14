using System;

namespace WonderlandChip.Database.DTO.Animal
{
    public class AnimalSearchDTO
    {
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int? ChipperId { get; set; }
        public long? ChippingLocationId { get; set; }
        public string? LifeStatus { get; set; }
        public string? Gender { get; set; }
        public int From { get; set; } = 0;
        public int Size { get; set; } = 10;
    }
}
