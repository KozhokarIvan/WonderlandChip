using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.CustomExceptions;
using WonderlandChip.Database.DTO.Account;
using WonderlandChip.Database.Repositories.Interfaces;
using WonderlandChip.WebAPI.Services;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly AuthenticationService _authenticationService;
        public AccountController
            (IAccountRepository accountRepository,
            AuthenticationService authenticationService)
        {
            this._accountRepository = accountRepository;
            _authenticationService = authenticationService;
        }
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountAsync(int? accountId)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization))
            {
                int? authenticatedUserId = await _authenticationService.GetAuthenticatedUserId(Request.Headers.Authorization);
                if (authenticatedUserId is null) return Unauthorized();
            }
            if (accountId is null || accountId <= 0)
                return BadRequest();
            AccountGetDTO account = await _accountRepository.GetAccountById(accountId);
            if (account is null)
                return NotFound();
            return Ok(account);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchAccountAsync([FromQuery] AccountSearchDTO request)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization))
            {
                int? authenticatedUserId = await _authenticationService.GetAuthenticatedUserId(Request.Headers.Authorization);
                if (authenticatedUserId is null) return Unauthorized();
            }
            if (request is not null && (request.From < 0 || request.Size <= 0))
                return BadRequest();
            List<AccountGetDTO> accounts = await _accountRepository.SearchAccounts(request);
            if (accounts is null)
                return NotFound();
            return Ok(accounts);
        }
        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateAccountAsync(int? accountId, AccountUpdateDTO request)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (accountId is null || accountId <= 0 ||
                request is null ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                !new EmailAddressAttribute().IsValid(request.Email))
                return BadRequest();
            request!.Id = accountId ?? throw new NullReferenceException();
            if (authenticatedUserId != request.Id) return StatusCode(StatusCodes.Status403Forbidden);
            try
            {
                AccountGetDTO? account = await _accountRepository.UpdateAccount(request);
                if (account is null) return StatusCode(StatusCodes.Status403Forbidden);
                if (account.Id != authenticatedUserId)
                    return Unauthorized();
                return Ok(account);

            }
            catch (EmailAlreadyExistsException)
            {
                return Conflict();
            }
        }
        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteAcountAsync(int? accountId)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (accountId is null || accountId <= 0)
                return BadRequest();
            if (authenticatedUserId != accountId) return StatusCode(StatusCodes.Status403Forbidden);
            try
            {

                int? deletedAccount = await _accountRepository.DeleteAccount(accountId);
                if (deletedAccount is null) return StatusCode(StatusCodes.Status403Forbidden);
                return Ok();
            }
            catch (AccountHasAnimalsException)
            {
                return BadRequest();
            }
        }
    }
}
