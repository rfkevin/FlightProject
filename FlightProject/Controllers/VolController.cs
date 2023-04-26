using FlightProject.Models;
using FlightProject.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace FlightProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAllOrigins")]
    public class VolController : ControllerBase
    {
        private readonly VolsService _volServices;

        public VolController(VolsService volServices)
        {
            _volServices = volServices;
        }
        [HttpGet]
        [EnableCors("AllowAllOrigins")]
        public async Task<List<Vol>> Get()
        {
            return await _volServices.GetVolAsync();
        }
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Vol?>> Get(string id)
        {
            var vol = await _volServices.GetVolAsync(id);
            if (vol == null) 
            { 
              return null;
            }
            return Ok(vol);
        }
        [HttpPost]
        [EnableCors("AllowAllOrigins")]
        public async Task<IActionResult> Create(Vol vol)
        {
            await _volServices.CreateVolAsync(vol);
            return CreatedAtAction(nameof(Create), vol);
        }
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Vol updateVol)
        {
            var vol = await _volServices.GetVolAsync(id);
            if(vol == null)
            {
                return NotFound();
            }
            await _volServices.UpdateVolAsync(id, vol);
            return NoContent();
        }
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete (string id)
        {
            var vol = await _volServices.GetVolAsync(id);
            if (vol == null)
            {
                return NotFound();
            }
            await _volServices.DeleteVolAsync(id);
            return NoContent(); 
        }
    }

}
