using FlightProject.Models;
using FlightProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using log4net;

namespace FlightProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationService _services;
        private readonly VolsService _vsols;
        private readonly JwtService _jwtService;
        private readonly ILog _log;
        public ReservationsController(ReservationService services, VolsService volsService, JwtService jwtService, ILog log)
        {
            _services = services;
            _vsols = volsService;
            _jwtService = jwtService;
            _log = log;
        }
        [HttpGet]
        public async Task<List<Reservation>> Get()
        {
            _log.Info($"GET /api/Reservations/");
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
        [HttpPost]
        [EnableCors("AllowAllOrigins")]
        [Authorize]
        public async Task<IActionResult> Post(Reservation reservation)
        {
            var token = reservation.client;

            var idClient = _jwtService.GetUserIdFromToken(token);
            _log.Info($"Post /api/Reservations/{idClient}");
            if (idClient == null)
            {
                return NotFound("try to reconnect ");
            }
            else
            {
                var oldReservation = await _services.CheckReservation(reservation.vol, idClient);
                if (oldReservation == null)
                {
                   
                    var vol = await _vsols.GetVolAsync(reservation.vol);
                    if (vol == null)
                    {
                        return NotFound("vols introuvable");
                    }
                    else if (vol.placeUsed >= vol.nbrPlace)
                    {
                        return BadRequest("vol est complet");

                    }

                    vol.placeUsed ++;
                    reservation.client = idClient;
                    Console.WriteLine(idClient);
                    Console.WriteLine("//////////////////////////////////////////////////////////////////////////////");
                    vol.passagers.Add(idClient);
                    
                    var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                    var stringWriter = new System.IO.StringWriter();
                    jsonSerializer.Serialize(stringWriter, vol);
                    var json = stringWriter.ToString();
                    _log.Info(json);
                    await _vsols.UpdateVolAsync(reservation.vol, vol);
                    await _services.CreateReservation(reservation);
                    return CreatedAtAction(nameof(Get), reservation);

                }
                return Conflict("A reservation with the same vol and client already exists.");
            }
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
