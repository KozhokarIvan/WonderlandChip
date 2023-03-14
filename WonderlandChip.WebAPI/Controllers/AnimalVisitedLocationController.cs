using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.DTO.AnimalVisitedLocation;
using WonderlandChip.Database.Repositories.Interfaces;
using WonderlandChip.WebAPI.Services;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/animals")]
    [ApiController]
    public class AnimalVisitedLocationController : ControllerBase
    {
        private readonly IVisitedLocationRepository _visitedLocationRepository;

        private readonly AuthenticationService _authenticationService;
        public AnimalVisitedLocationController
            (IVisitedLocationRepository visitedLocationRepository,
            AuthenticationService authenticationService)
        {
            _visitedLocationRepository = visitedLocationRepository;
            _authenticationService = authenticationService;
        }
        [HttpGet("{animalId}/locations")]
        public async Task<IActionResult> GetVisitedLocationsAsync(long? animalId, [FromQuery] AnimalVisitedLocationSearchDTO request)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization) &&
                !await _authenticationService.TryAuthenticate(Request.Headers.Authorization))
                return Unauthorized();
            if (animalId is null || animalId <= 0 || request is not null && 
                (request.From > 0 || request.Size <= 0))
                return BadRequest();
            request.AnimalId = (long)animalId;
            List<AnimalVisitedLocationGetDTO> visitedLocations = await _visitedLocationRepository.SearchVisitedLocation(request);
            if (visitedLocations is null)
                return NotFound();
            return Ok(visitedLocations);
        }
    }
}
