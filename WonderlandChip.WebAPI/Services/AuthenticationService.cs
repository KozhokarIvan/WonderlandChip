using System;
using System.Text;
using System.Threading.Tasks;
using WonderlandChip.Database.DTO.Account;
using WonderlandChip.Database.Repositories.Interfaces;

namespace WonderlandChip.WebAPI.Services
{
    public class AuthenticationService
    {
        private readonly IAccountRepository _accountRepository;
        public AuthenticationService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<int?> GetAuthenticatedUserId(string authenticationHeader)
        {
            string[] credentials;
            string encodedUsernamePassword = authenticationHeader
                .Substring("Basic ".Length).Trim();
            credentials = Encoding.UTF8
                .GetString(Convert.FromBase64String(encodedUsernamePassword)).Split(':');
            return await _accountRepository
                .TryAuthenticate(new AuthorizeDTO() { Email = credentials[0], Password = credentials[1] });
        }
    }
}
