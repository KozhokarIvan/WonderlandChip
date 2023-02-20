using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.WebAPI.ApiModels.LocationPoint;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/locations")]
    [ApiController]
    public class LocationPointController : ControllerBase
    {
        [HttpGet("{pointId}")]
        public async Task<IActionResult> GetLocationPointId(long pointId)
        {
            if (/*Unauthorized*/false)
                return Unauthorized();
            if (pointId <= 0)
                return BadRequest();
            if (/*not found*/pointId > 10000)
                return NotFound();
            return Ok(new LocationPointGetDTO()
            {
                Id = pointId,
                Latitude = pointId - 1,
                Longitude = pointId + 1
            });
        }
    }
}
