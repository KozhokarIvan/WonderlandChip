using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.DTO.AnimalType;
using WonderlandChip.Database.Repositories.Interfaces;
using WonderlandChip.WebAPI.Services;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/animals/types")]
    [ApiController]
    public class AnimalTypeController : ControllerBase
    {
        private readonly IAnimalTypeRepository _animalTypeRepository;

        private readonly AuthenticationService _authenticationService;
        public AnimalTypeController
            (IAnimalTypeRepository animalTypeRepository, 
            AuthenticationService authenticationService)
        {
            _animalTypeRepository = animalTypeRepository;
            _authenticationService = authenticationService;
        }
        [HttpGet("{typeId}")]
        public async Task<IActionResult> GetAnimalTypeIdAsync(long typeId)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization) &&
                !await _authenticationService.TryAuthenticate(Request.Headers.Authorization))
                return Unauthorized();
            if (typeId <= 0)
                return BadRequest();
            AnimalTypeGetDTO animalType = await _animalTypeRepository.GetAnimalTypeById(typeId);
            if (animalType is null)
                return NotFound();
            return Ok(animalType);
        }
    }
}
