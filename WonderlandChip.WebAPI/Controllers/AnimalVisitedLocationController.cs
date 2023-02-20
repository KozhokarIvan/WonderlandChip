using System.Globalization;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.WebAPI.ApiModels.AnimalVisitedLocation;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/animals")]
    [ApiController]
    public class AnimalVisitedLocationController : ControllerBase
    {
        [HttpGet("{animalId}/locations")]
        public async Task<IActionResult> GetVisitedLocationsAsync(long? animalId, [FromQuery] AnimalVisitedLocationSearchDTO request)
        {
            if (/*Unauthorized*/false)
                return Unauthorized();
            if (animalId is null || animalId <= 0 || request is not null && (request.From > 0 || request.Size <= 0) 
                /*or one of datetimes or both are wrong*/)
                return BadRequest();
            if (/*NotFound*/animalId is not null && animalId > 10000)
                return NotFound();
            AnimalVisitedLocationGetDTO response = new AnimalVisitedLocationGetDTO()
            {
                Id = 1,
                DateTimeOfVisitLocationPoint = DateTime.Now,
                LocationPointId = 1
            };
            return Ok(new AnimalVisitedLocationGetDTO[] { response });
        }
    }
}
