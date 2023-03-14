using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.DTO.Animal;
using WonderlandChip.Database.Repositories.Interfaces;
using WonderlandChip.WebAPI.Services;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/animals")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly AuthenticationService _authenticationService;
        public AnimalController
            (IAnimalRepository animalRepository, 
            AuthenticationService authenticationService)
        {
            _animalRepository = animalRepository;
            _authenticationService = authenticationService;

        }
        [HttpGet("{animalId}")]
        public async Task<IActionResult> GetAnimalAsync(long animalId)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization) &&
                !await _authenticationService.TryAuthenticate(Request.Headers.Authorization))
                return Unauthorized();
            if (animalId <= 0)
                return BadRequest();
            AnimalGetDTO animal = await _animalRepository.GetAnimalById(animalId);
            if (animal is null)
                return NotFound();
            return Ok(animal);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchAnimalAsync([FromQuery] AnimalSearchDTO request)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization) &&
                !await _authenticationService.TryAuthenticate(Request.Headers.Authorization))
                return Unauthorized();
            if (request is not null &&
                (request.From < 0 || request.Size <= 0 ||
                request.ChipperId is not null && request.ChipperId >= 0 ||
                request.ChippingLocationId is not null && request.ChippingLocationId >= 0 ||
                request.LifeStatus is not null && request.LifeStatus != "ALIVE" && request.LifeStatus != "DEAD" ||
                request.Gender is not null &&
                request.Gender != "MALE" && request.Gender != "FEMALE" && request.Gender != "OTHER"))
                return BadRequest();
            List<AnimalGetDTO> animals = await _animalRepository.SearchAnimal(request);
            return Ok(animals);
        }
    }
}
