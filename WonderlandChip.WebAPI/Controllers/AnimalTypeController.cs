using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WonderlandChip.WebAPI.ApiModels.AnimalType;

namespace WonderlandChip.WebAPI.Controllers
{
    [Route("/animals/types")]
    [ApiController]
    public class AnimalTypeController : ControllerBase
    {
        [HttpGet("{typeId}")]
        public async Task<IActionResult> GetAnimalTypeIdAsync(long typeId)
        {
            if (/*Unauthorized*/false)
                return Unauthorized();
            if (typeId <= 0)
                return BadRequest();
            if (/*Not found*/typeId>10000)
                return NotFound();
            return Ok(new AnimalTypeGetDTO()
            {
                Id = typeId,
                Type = "There should be a type"
            });
        }
    }
}
