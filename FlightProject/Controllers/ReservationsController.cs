using FlightProject.Models;
using FlightProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationService _services;
        private readonly VolsService _vsols;
        public ReservationsController(ReservationService services, VolsService volsService)
        {
            _services = services;
            _vsols = volsService;
        }
        [HttpGet]
        public async Task<List<Reservation>> Get()
        {
           return  await _services.GetReservationsAsync();
        }
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Reservation>> Get(string id)
        {
            var reservation = await _services.GetReservationAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }
        [HttpPost("{id:length(24)}")]
        public async Task<IActionResult> Post(Reservation reservation)
        {
            var oldReservation = await _services.CheckReservation(reservation.vol, reservation.client);
            if (oldReservation == null)
            {
                await _services.CreateReservation(reservation);
                var vol = await _vsols.GetVolAsync(reservation.vol);
                if (vol == null)
                {
                    return NotFound("vols introuvable");
                }
                else if(vol.placeUsed >= vol.nbrPlace)
                {
                    return BadRequest("vol est complet");
                }
                vol.placeUsed = vol.placeUsed + 1;
                vol.passagers.Add(reservation.client);
                await _vsols.UpdateVolAsync(vol.Id, vol);
                return CreatedAtAction(nameof(Get), reservation);
            }
            return Conflict("A reservation with the same vol and client already exists.");
        }
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Reservation newReservation)
        {
            var reservation = await _services.GetReservationAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            await _services.UpdateReservation(id, reservation);
            return NoContent();
        }
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var reservation = await _services.GetReservationAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            await _services.DeleteReservation(id);
            return NoContent();
        }
    }
}
