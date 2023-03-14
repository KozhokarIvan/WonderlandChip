using System;

namespace WonderlandChip.Database.DTO.AnimalVisitedLocation
{
    public class AnimalVisitedLocationSearchDTO
    {

        public long AnimalId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int From { get; set; } = 0;
        public int Size { get; set; } = 10;
    }
}
