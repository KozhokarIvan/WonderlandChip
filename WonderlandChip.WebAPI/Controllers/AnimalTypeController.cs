using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.CustomExceptions;
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
        public async Task<IActionResult> GetAnimalTypeIdAsync(long? typeId)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization))
            {
                int? authenticatedUserId = await _authenticationService.GetAuthenticatedUserId(Request.Headers.Authorization);
                if (authenticatedUserId is null) return Unauthorized();
            }
            if (typeId is null || typeId <= 0)
                return BadRequest();
            AnimalTypeDTO animalType = await _animalTypeRepository.GetAnimalTypeById(typeId);
            if (animalType is null)
                return NotFound();
            return Ok(animalType);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAnimalTypeAsync(AnimalTypeWithoutIdDTO animalTypeRequest)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (string.IsNullOrWhiteSpace(animalTypeRequest.Type))
                return BadRequest();
            try
            {
                AnimalTypeDTO animalType = await _animalTypeRepository.CreateAnimalType(animalTypeRequest.Type);
                return Created("/", animalType);
            }
            catch (Exception)
            {
                return Conflict();
            }
        }
        [HttpPut("{typeId}")]
        public async Task<IActionResult> UpdateAnimalTypeAsync(long? typeId, AnimalTypeWithoutIdDTO animalTypeRequest)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (string.IsNullOrWhiteSpace(animalTypeRequest.Type) || typeId is null || typeId <= 0)
                return BadRequest();
            try
            {
                AnimalTypeDTO animal = await _animalTypeRepository
                    .UpdateAnimalTypeById(new AnimalTypeDTO() { Id = typeId, Type = animalTypeRequest.Type });
                if (animal is null) return NotFound();
                return Ok(animal);
            }
            catch (AnimalTypeAlreadyExistsException)
            {
                return Conflict();
            }
        }
        [HttpDelete("{typeId}")]
        public async Task<IActionResult> DeleteAnimalTypeAsync(long? typeId)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (typeId is null || typeId <= 0)
                return BadRequest();
            try
            {
                long? deletedAnimalTypeId = await _animalTypeRepository.DeleteAnimalTypeById(typeId);
                if (deletedAnimalTypeId is null) return NotFound();
                return Ok(deletedAnimalTypeId);
            }
            catch (TypeIsBoundToAnimalException)
            {
                return BadRequest();
            }
        }
    }
}
