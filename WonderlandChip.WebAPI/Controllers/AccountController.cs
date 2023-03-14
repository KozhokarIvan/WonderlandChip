using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.DTO.Account;
using WonderlandChip.Database.Repositories.Interfaces;
using WonderlandChip.WebAPI.Services;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository accountRepository;
        private readonly AuthenticationService _authenticationService;
        public AccountController
            (IAccountRepository accountRepository, 
            AuthenticationService authenticationService)
        {
            this.accountRepository = accountRepository;
            _authenticationService = authenticationService;
        }
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountAsync(int accountId)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization) &&
                !await _authenticationService.TryAuthenticate(Request.Headers.Authorization))
                return Unauthorized();
            if (accountId <= 0)
                return BadRequest();
            AccountGetDTO account = await accountRepository.GetAccountById(accountId);
            if (account is null)
                return NotFound();
            return Ok(account);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchAccountAsync([FromQuery] AccountSearchDTO request)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization) &&
                !await _authenticationService.TryAuthenticate(Request.Headers.Authorization))
                return Unauthorized();
            if (request is not null && (request.From < 0 || request.Size <= 0))
                return BadRequest();
            List<AccountGetDTO> accounts = await accountRepository.SearchAccounts(request);
            if (accounts is null)
                return NotFound();
            return Ok(accounts);
        }
    }
}
