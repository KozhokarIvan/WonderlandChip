using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.CustomExceptions;
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
        public async Task<IActionResult> GetAnimalAsync(long? animalId)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization))
            {
                int? authenticatedUserId = await _authenticationService.GetAuthenticatedUserId(Request.Headers.Authorization);
                if (authenticatedUserId is null) return Unauthorized();
            }
            if (animalId is null || animalId <= 0)
                return BadRequest();
            AnimalGetDTO animal = await _animalRepository.GetAnimalById(animalId);
            if (animal is null)
                return NotFound();
            return Ok(animal);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchAnimalAsync([FromQuery] AnimalSearchDTO request)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization))
            {
                int? authenticatedUserId = await _authenticationService.GetAuthenticatedUserId(Request.Headers.Authorization);
                if (authenticatedUserId is null) return Unauthorized();
            }
            if (request is null || request.From < 0 || request.Size <= 0 ||
                request.ChipperId is not null && request.ChipperId <= 0 ||
                request.ChippingLocationId is not null && request.ChippingLocationId >= 0 ||
                request.LifeStatus is not null && request.LifeStatus != "ALIVE" && request.LifeStatus != "DEAD" ||
                request.Gender is not null &&
                request.Gender != "MALE" && request.Gender != "FEMALE" && request.Gender != "OTHER")
                return BadRequest();
            List<AnimalGetDTO> animals = await _animalRepository.SearchAnimals(request);
            return Ok(animals);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAnimalAsync(AnimalCreateDTO animalCreate)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (animalCreate.AnimalTypes is null || animalCreate.AnimalTypes.Length <= 0 ||
                animalCreate.AnimalTypes.Any(at => at is null || at <= 0) ||
                animalCreate.Weight is null || animalCreate.Weight <= 0 ||
                animalCreate.Length is null || animalCreate.Length <= 0 ||
                animalCreate.Height is null || animalCreate.Height <= 0 ||
                animalCreate.Gender is null || (animalCreate.Gender != "MALE" &&
                animalCreate.Gender != "FEMALE" && animalCreate.Gender != "OTHER") ||
                animalCreate.ChipperId is null || animalCreate.ChipperId <= 0 ||
                animalCreate.ChippingLocationId is null ||
                animalCreate.ChippingLocationId <= 0)
                return BadRequest();
            try
            {
                AnimalGetDTO animal = await _animalRepository.AddAnimal(animalCreate);
                if (animal == null) return NotFound();
                return Created("/", animal);
            }
            catch (AnimalTypesHasDuplicatesException)
            {
                return Conflict();
            }
        }
        [HttpPut("{animalId}")]
        public async Task<IActionResult> UpdateAnimalAsync(long? animalId, AnimalUpdateDTO animalUpdate)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (animalUpdate is null || animalUpdate is not null &&
                (animalUpdate.Weight is null || animalUpdate.Weight <= 0 ||
                animalUpdate.Length is null || animalUpdate.Length <= 0 ||
                animalUpdate.Height is null || animalUpdate.Height <= 0 ||
                animalUpdate.Gender is null || (animalUpdate.Gender != "MALE" &&
                animalUpdate.Gender != "FEMALE" && animalUpdate.Gender != "OTHER") ||
                animalUpdate.LifeStatus is null || (animalUpdate.LifeStatus != "ALIVE" &&
                animalUpdate.LifeStatus != "DEAD") ||
                animalUpdate.ChipperId is null || animalUpdate.ChippingLocationId is null ||
                animalUpdate.ChippingLocationId <= 0))
                return BadRequest();
            try
            {
                animalUpdate.AnimalId = animalId;
                AnimalGetDTO animal = await _animalRepository
                    .UpdateAnimal(animalUpdate);
                if (animal is null) return NotFound();
                return Ok(animal);
            }
            catch (AnimalResurrectingException)
            {
                return BadRequest();
            }
            catch (NewLocationIsFirstException)
            {
                return BadRequest();
            }

        }
        [HttpDelete("{animalId}")]
        public async Task<IActionResult> DeleteAnimalAsync(long? animalId)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (animalId is null || animalId <= 0)
                return BadRequest();
            try
            {
                long? deletedAnimalId = await _animalRepository.DeleteAnimal(animalId);
                if (deletedAnimalId is null) return NotFound();
                return Ok(deletedAnimalId);
            }
            catch (AnimalHasLocationPointsException)
            {
                return BadRequest();
            }
        }
        [HttpPost("{animalId}/types/{typeId}")]
        public async Task<IActionResult> CreateAnimalTypeAsync(
            long? animalId,
            long? typeId)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (
                animalId is null || animalId <= 0 ||
                typeId is null || typeId <= 0)
                return BadRequest();
            try
            {
                AnimalGetDTO animal = await _animalRepository.AddAnimalType(
                    new AnimalCreateTypeDTO() { AnimalId = animalId, TypeId = typeId });
                if (animal is null) return NotFound();
                return Created("/", animal);
            }
            catch (AnimalAlreadyHasTypeException)
            {
                return Conflict();
            }
        }
        [HttpPut("{animalId}/types")]
        public async Task<IActionResult> UpdateAnimalTypeAsync(long? animalId, AnimalUpdateTypeDTO updateType)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (animalId is null || animalId <= 0 ||
                updateType is null || updateType is not null &&
                (updateType.OldTypeId is null || updateType.OldTypeId <= 0 ||
                updateType.NewTypeId is null || updateType.NewTypeId <= 0))
                return BadRequest();
            try
            {
                updateType.AnimalId = animalId;
                AnimalGetDTO animal = await _animalRepository.UpdateAnimalType(updateType);
                if (animal is null) return NotFound();
                return Ok(animal);
            }
            catch (AnimalAlreadyHasTypeException)
            {
                return Conflict();
            }
        }
        [HttpDelete("{animalId}/types/{typeId}")]
        public async Task<IActionResult> DeleteAnimalTypeAsync(long? animalId, long? typeId)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (animalId is null || animalId <= 0 ||
                typeId is null || typeId <= 0)
                return BadRequest();
            try
            {
                AnimalGetDTO? deletedAnimalTypeId = await _animalRepository
                    .DeleteAnimalType(new AnimalDeleteTypeDTO()
                    {
                        AnimalId = animalId,
                        TypeId = typeId
                    });
                if (deletedAnimalTypeId is null) return NotFound();
                return Ok();
            }
            catch (AnimalWontHaveTypesException)
            {
                return BadRequest();
            }
        }
    }
}
