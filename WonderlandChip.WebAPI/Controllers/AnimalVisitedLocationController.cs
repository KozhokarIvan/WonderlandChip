using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.CustomExceptions;
using WonderlandChip.Database.DTO.AnimalVisitedLocation;
using WonderlandChip.Database.Repositories.Interfaces;
using WonderlandChip.WebAPI.Services;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/animals")]
    [ApiController]
    public class AnimalVisitedLocationController : ControllerBase
    {
        private readonly IAnimalVisitedLocationRepository _animalVisitedLocationRepository;

        private readonly AuthenticationService _authenticationService;
        public AnimalVisitedLocationController
            (IAnimalVisitedLocationRepository visitedLocationRepository,
            AuthenticationService authenticationService)
        {
            _animalVisitedLocationRepository = visitedLocationRepository;
            _authenticationService = authenticationService;
        }
        [HttpGet("{animalId}/locations")]
        public async Task<IActionResult> SearchVisitedLocationsAsync(
            long? animalId,
            [FromQuery] AnimalVisitedLocationSearchDTO request)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization))
            {
                int? authenticatedUserId = await _authenticationService.GetAuthenticatedUserId(Request.Headers.Authorization);
                if (authenticatedUserId is null) return Unauthorized();
            }
            if (animalId is null || animalId <= 0 || request is not null &&
                (request.From > 0 || request.Size <= 0))
                return BadRequest();
            request.AnimalId = (long)animalId;
            List<AnimalVisitedLocationGetDTO> visitedLocations;
            try
            {
                visitedLocations =
                    await _animalVisitedLocationRepository.SearchVisitedLocation(request);
            }
            catch (AnimalIsDeadException)
            {
                return BadRequest();
            }
            if (visitedLocations is null)
                return NotFound();
            return Ok(visitedLocations);
        }
        [HttpPost("{animalId}/locations/{pointId}")]
        public async Task<IActionResult> CreateVisitedLocationAsync(
            long? animalId,
            long? pointId)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (animalId is null || animalId <= 0 ||
                pointId is null || pointId <= 0)
                return BadRequest();
            try
            {
                AnimalVisitedLocationGetDTO animalVisitedLocation =
                    await _animalVisitedLocationRepository
                    .AddVisitedLocation(new AnimalVisitedLocationAddDTO() { AnimalId = animalId, PointId = pointId });
                if (animalVisitedLocation is null) return NotFound();
                return Created("/", animalVisitedLocation);
            }
            catch (AnimalIsAtChippingPointException)
            {
                return BadRequest();
            }
            catch (AddingChippingPointException)
            {
                return BadRequest();
            }
            catch (AnimalIsAlreadyHereException)
            {
                return BadRequest();
            }
            catch (AnimalIsDeadException)
            {
                return BadRequest();
            }
        }
        [HttpPut("{animalId}/locations")]
        public async Task<IActionResult> UpdateAnimalVisitedLocationAsync(
            long? animalId,
            AnimalVisitedLocationUpdateDTO? locationUpdate)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (animalId is null || animalId <= 0 ||
                locationUpdate is null || locationUpdate is not null &&
                (locationUpdate.VisitedLocationPointId is null || locationUpdate.VisitedLocationPointId <= 0 ||
                locationUpdate.LocationPointId is null || locationUpdate.LocationPointId <= 0))
                return BadRequest();
            try
            {
                locationUpdate.AnimalId = animalId ?? throw new NullReferenceException();
                AnimalVisitedLocationGetDTO location = await _animalVisitedLocationRepository
                    .UpdateVisitedLocation(locationUpdate);
                if (location is null) return NotFound();
                return Ok(location);
            }
            catch (AnimalLocationPointIsTheSameException)
            {
                return BadRequest();
            }
            catch (FirstLocationMatchesChippingLocationException)
            {
                return BadRequest();
            }
        }
        [HttpDelete("{animalId}/locations/{visitedPointId}")]
        public async Task<IActionResult> DeleteAnimalVisitedLocationAsync(
            long? animalId,
            long? visitedPointId)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (animalId is null || animalId <= 0 ||
                visitedPointId is null || visitedPointId <= 0)
                return BadRequest();
            long? deletedLocationId = await _animalVisitedLocationRepository
                    .DeleteVisitedLocation(new AnimalVisitedLocationDeleteDTO()
                    {
                        AnimalId = animalId,
                        VisitedPointId = visitedPointId
                    });
            if (deletedLocationId is null) return NotFound();
            return Ok();
        }
    }
}
