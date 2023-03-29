namespace WonderlandChip.Database.DTO.Animal
{
    public class AnimalUpdateTypeDTO
    {
        public long? AnimalId { get; set; }
        public long? OldTypeId { get; set; }
        public long? NewTypeId { get; set; }
    }
}
