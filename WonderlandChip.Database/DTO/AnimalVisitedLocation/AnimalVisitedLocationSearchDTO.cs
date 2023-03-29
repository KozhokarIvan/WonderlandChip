using System;

namespace WonderlandChip.Database.DTO.AnimalVisitedLocation
{
    public class AnimalVisitedLocationSearchDTO
    {

        public long AnimalId { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public int From { get; set; } = 0;
        public int Size { get; set; } = 10;
    }
}
