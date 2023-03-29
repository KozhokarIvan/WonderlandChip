using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.Database.CustomExceptions;
using WonderlandChip.Database.DTO.LocationPoint;
using WonderlandChip.Database.Repositories.Interfaces;
using WonderlandChip.WebAPI.Services;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/locations")]
    [ApiController]
    public class LocationPointController : ControllerBase
    {
        private readonly ILocationPointRepository _locationRepository;

        private readonly AuthenticationService _authenticationService;
        public LocationPointController
            (ILocationPointRepository locationRepository,
            AuthenticationService authenticationService)
        {
            _locationRepository = locationRepository;
            _authenticationService = authenticationService;
        }
        [HttpGet("{pointId}")]
        public async Task<IActionResult> GetLocationPointIdAsync(long? pointId)
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers.Authorization))
            {
                int? authenticatedUserId = await _authenticationService.GetAuthenticatedUserId(Request.Headers.Authorization);
                if (authenticatedUserId is null) return Unauthorized();
            }
            if (pointId is null || pointId <= 0)
                return BadRequest();
            LocationPointWithIdDTO location = await _locationRepository.GetLocationById(pointId);
            if (location is null)
                return NotFound();
            return Ok(location);
        }
        [HttpPost]
        public async Task<IActionResult> AddLocationPointAsync(LocationPointDTO location)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (location is null || location is not null &&
                (location.Latitude is null || location.Latitude < -90 || location.Latitude > 90 ||
                location.Longitude is null || location.Longitude < -180 || location.Longitude > 180))
                return BadRequest();
            try
            {
                LocationPointWithIdDTO locationPoint = await _locationRepository.CreateLocation(location);
                return Created("/", locationPoint);
            }
            catch (PointAlreadyExistsException)
            {
                return Conflict();
            }
        }
        [HttpPut("{pointId}")]
        public async Task<IActionResult> UpdateLocationPointAsync(
            long? pointId,
            LocationPointDTO requestLocation)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (pointId is null || pointId <= 0 || requestLocation is null
                || requestLocation.Latitude is null || requestLocation.Latitude < -90 || requestLocation.Latitude > 90
                || requestLocation.Longitude is null || requestLocation.Longitude < -180 || requestLocation.Longitude > 180)
                return BadRequest();
            try
            {
                LocationPointWithIdDTO locationPoint = await _locationRepository
                    .UpdateLocation(new LocationPointWithIdDTO()
                    {
                        Id = pointId ?? throw new NullReferenceException(),
                        Latitude = requestLocation.Latitude ?? throw new NullReferenceException(),
                        Longitude = requestLocation.Longitude ?? throw new NullReferenceException()
                    });
                if (locationPoint is null) return NotFound();
                return Ok(locationPoint);
            }
            catch (PointAlreadyExistsException)
            {
                return Conflict();
            }
        }
        [HttpDelete("{pointId}")]
        public async Task<IActionResult> DeleteLocationPointAsync(long? pointId)
        {
            if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
                return Unauthorized();
            int? authenticatedUserId = await _authenticationService
                .GetAuthenticatedUserId(Request.Headers.Authorization);
            if (authenticatedUserId is null) return Unauthorized();
            if (pointId is null || pointId <= 0)
                return BadRequest();
            try
            {
                long? deletedLocationPointId = await _locationRepository.DeleteLocation(pointId);
                if (deletedLocationPointId is null) return NotFound();
                return Ok();
            }
            catch (PointIsReferencedException)
            {
                return BadRequest();
            }
        }
    }
}
