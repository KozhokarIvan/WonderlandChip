using System.Collections.Generic;

namespace WonderlandChip.Database.Models
{
    public class AnimalType
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Animal>? Animals { get; set; }
    }
}
