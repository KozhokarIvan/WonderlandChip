namespace WonderlandChip.Database.DTO.Animal
{
    public class AnimalUpdateDTO
    {
        public long? AnimalId { get; set; }
        public float? Weight { get; set; }
        public float? Length { get; set; }
        public float? Height { get; set; }
        public string? Gender { get; set; } = null!;
        public string? LifeStatus { get; set; } = null!;
        public int? ChipperId { get; set; }
        public long? ChippingLocationId { get; set; }

    }
}
