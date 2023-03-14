using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index(RegisterPostDTO request)
        {
            if (request is null ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                !new EmailAddressAttribute().IsValid(request.Email))
                return BadRequest();
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Forbid();
            if (await _accountRepository.DoesEmailExist(request.Email))
                return Conflict();
            RegisterGetDTO user = await _accountRepository.RegisterAccount(request);
            return Created("/", user);
        }
    }
}
