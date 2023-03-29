namespace WonderlandChip.Database.DTO.Account
{
    public class AccountDeleteDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
