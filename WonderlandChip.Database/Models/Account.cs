using System.Collections.Generic;

namespace WonderlandChip.Database.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public List<Animal>? Animals { get; set; }
    }
}
