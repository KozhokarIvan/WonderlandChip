using System;

namespace WonderlandChip.Database.DTO.AnimalVisitedLocation
{
    public class AnimalVisitedLocationGetDTO
    {
        public long Id { get; set; }
        public DateTime DateTimeOfVisitLocationPoint { get; set; }
        public long LocationPointId { get; set; }
    }
}
