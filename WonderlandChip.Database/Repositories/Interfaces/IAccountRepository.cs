using System.Collections.Generic;
using System.Threading.Tasks;
using WonderlandChip.Database.DTO.Account;
using WonderlandChip.Database.DTO.Authentication;

namespace WonderlandChip.Database.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        public Task<AccountGetDTO> GetAccountById(int id);
        public Task<List<AccountGetDTO>> SearchAccounts(AccountSearchDTO user);
        public Task<RegisterGetDTO> RegisterAccount(RegisterPostDTO user);
        public Task<bool> TryAuthenticate(AuthorizeDTO credentials);
        public Task<bool> DoesEmailExist(string email);
    }
}
