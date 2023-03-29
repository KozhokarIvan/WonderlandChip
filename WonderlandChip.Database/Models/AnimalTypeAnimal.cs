namespace WonderlandChip.Database.Models
{
    public class AnimalTypeAnimal
    {
        public long Id { get; set; }
        public long AnimalTypeId { get; set; }
        public long AnimalId { get; set; }
        public Animal Animal { get; set; } = null!;
        public AnimalType AnimalType { get; set; } = null!;
    }
}
