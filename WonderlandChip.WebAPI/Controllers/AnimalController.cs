using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WonderlandChip.WebAPI.ApiModels.Animal;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/animals")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        [HttpGet("{animalId}")]
        public async Task<IActionResult> GetAnimalAsync(long animalId)
        {
            if (/*Unauthorized*/false)
                return Unauthorized();
            if (animalId <= 0)
                return BadRequest();
            if (/*id was not found*/animalId > 10000)
                return NotFound();
            return Ok(new AnimalGetDTO
            {
                Id = animalId,
                AnimalTypes = new long[] { 1, 2, 3 },
                Weight = 1,
                Length = 2,
                Height = 3,
                Gender = "FEMALE",
                LifeStatus = "ALIVE",
                ChippingDateTime = DateTime.Now,
                ChipperId = 1,
                ChippingLocationId = 1,
                VisitedLocations = new long[] { 1, 2, 3 },
                DeathDateTime = null
            });
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchAnimalAsync([FromQuery] AnimalSearchDTO request)
        {
            if (/*Unauthorized*/false)
                return Unauthorized();
            //TODO sort by id asc 0,1,2
            if (request is not null &&
                (request.From < 0 || request.Size <= 0 ||
                request.ChipperId is not null && request.ChipperId >= 0 ||
                request.ChippingLocationId is not null && request.ChippingLocationId >= 0 ||
                request.LifeStatus is not null && request.LifeStatus != "ALIVE" && request.LifeStatus != "DEAD" ||
                request.Gender is not null &&
                request.Gender != "MALE" && request.Gender != "FEMALE" && request.Gender != "OTHER"))
                return BadRequest();
            AnimalGetDTO response = new AnimalGetDTO()
            {
                Id = 1,
                AnimalTypes = new long[] { 1, 2, 3 },
                Weight = 23,
                Length = 24,
                Height = 25,
                ChippingDateTime = DateTime.Now,
                VisitedLocations = new long[] { 1, 2, 3 }
            };
            if (request is not null)
            {
                response.Gender = request?.Gender ?? "MALE";
                response.LifeStatus = request?.LifeStatus ?? "ALIVE";
                response.ChipperId = request?.ChipperId ?? 1;
                response.ChippingLocationId = request?.ChippingLocationId ?? 1;
            }
            return Ok(new AnimalGetDTO[] { response });
        }
    }
}
