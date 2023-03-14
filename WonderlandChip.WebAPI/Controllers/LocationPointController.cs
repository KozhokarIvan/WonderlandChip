using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.DTO.LocationPoint;
using WonderlandChip.Database.Repositories.Interfaces;
using WonderlandChip.WebAPI.Services;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/locations")]
    [ApiController]
    public class LocationPointController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;

        private readonly AuthenticationService _authenticationService;
        public LocationPointController
            (ILocationRepository locationRepository,
            AuthenticationService authenticationService)
        {
            _locationRepository = locationRepository;
            _authenticationService = authenticationService;
        }
        [HttpGet("{pointId}")]
        public async Task<IActionResult> GetLocationPointId(long pointId)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization) &&
                !await _authenticationService.TryAuthenticate(Request.Headers.Authorization))
                return Unauthorized();
            if (pointId <= 0)
                return BadRequest();
            LocationPointGetDTO location = await _locationRepository.GetLocationById(pointId);
            if (location is null)
                return NotFound();
            return Ok(location);
        }
    }
}
