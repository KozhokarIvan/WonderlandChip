namespace WonderlandChip.Database.DTO.Animal
{
    public class AnimalCreateDTO
    {
        public long?[] AnimalTypes { get; set; } = null!;
        public float? Weight { get; set; }
        public float? Length { get; set; }
        public float? Height { get; set; }
        public string? Gender { get; set; }
        public int? ChipperId { get; set; }
        public long? ChippingLocationId { get; set; }
    }
}
