using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.WebAPI.ApiModels.Account;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountAsync(int accountId)
        {
            if (/*wrong auth*/false)
                return Unauthorized();
            if (accountId <= 0)
                return BadRequest();
            if (/*id was not found*/accountId > 10000)
                return NotFound();
            return Ok(new AccountGetDTO
            {
                Id = (int)accountId,
                FirstName = "Thomas",
                LastName = "Shelby",
                Email = "tommyshelbyinc@yandex.ru"
            });
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchAccountAsync([FromQuery] AccountSearchDTO request)
        {
            if (/*Unauthorized*/ false)
                return Unauthorized();
            if (request is not null && (request.From < 0 || request.Size <= 0))
                return BadRequest();
            //TODO sort result by id asc 0,1,2
            AccountGetDTO response = new AccountGetDTO()
            {
                Id = 1,
                Email = request.Email ?? "tommyshelbyinc@yandex.ru",
                FirstName = request.FirstName ?? "Thomas",
                LastName = request.LastName ?? "Shleby"
            };
            return Ok(new AccountGetDTO[] { response });
        }
    }
}
