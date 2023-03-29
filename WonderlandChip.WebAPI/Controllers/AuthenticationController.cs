using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.DTO.Authentication;
using WonderlandChip.Database.Repositories.Interfaces;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/registration")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        public AuthenticationController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Index(RegisterAccountPostDTO credentials)
        {
            if (credentials is null ||
                string.IsNullOrWhiteSpace(credentials.FirstName) ||
                string.IsNullOrWhiteSpace(credentials.LastName) ||
                string.IsNullOrWhiteSpace(credentials.Email) ||
                string.IsNullOrWhiteSpace(credentials.Password) ||
                !new EmailAddressAttribute().IsValid(credentials.Email))
                return BadRequest();
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return StatusCode(StatusCodes.Status403Forbidden);
            bool doesEmailExist = await _accountRepository.DoesEmailExist(credentials.Email);
            if (doesEmailExist)
                return Conflict();
            RegisterGetDTO user = await _accountRepository.RegisterAccount(credentials);
            return Created("/", user);
        }
    }
}
