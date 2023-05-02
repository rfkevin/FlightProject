using FlightProject.Models;
using FlightProject.Services;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlightProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ClientsService _clientsServices;
        private readonly ReservationService _reservationServices;
        private readonly VolsService _volsService;
        private readonly JwtService _jwtService;
        private readonly ILog _log;
        public ClientController(ClientsService clientsServices, JwtService jwtService, VolsService volsServices, ReservationService reservation, ILog log)
        {
            _clientsServices = clientsServices;
            _jwtService = jwtService;
            _reservationServices = reservation;
            _volsService = volsServices;
            _log = log;
        }
        [HttpGet]
        [EnableCors("AllowAllOrigins")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<List<Client>> Get()
        {
            _log.Info($"Requst all Client");

            return await _clientsServices.GetClientsAsync();
        }
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Client>> Get(string id)
        {
            var client = await _clientsServices.GetClientsAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }
        [EnableCors("AllowAllOrigins")]
        [HttpPost("Register")]

        public async Task<IActionResult> CreateUsers(Client newClient)
        {
            var client = await _clientsServices.GetClientsbyEmailAsync(newClient.email);
            if (client != null)
            {
                return BadRequest("L'email est déjà utilisé pour un autre compte.");
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newClient.password);
            var user = new Client
            {
                firstName = newClient.firstName,
                lastName = newClient.lastName,
                address = newClient.address,
                email = newClient.email,
                password = hashedPassword,
                phone = newClient.phone,
                birthday = newClient.birthday,
                numeroPasseport = newClient.numeroPasseport
            };
            await _clientsServices.CreateAsync(user);
            _log.Info($"client create for email { user.email} ");
            return Ok();
        }

        [HttpPost("Login")]
        [EnableCors("AllowAllOrigins")]
        public async Task<ActionResult<string>> Login(LoginUser loginUser)
        {
            var client = await _clientsServices.GetClientsbyEmailAsync(loginUser.email);
            if (client == null)
            {
                return BadRequest("Email ou mot de passe incorrect");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginUser.password, client.password);
            if (!isPasswordValid)
            {
                return BadRequest("Email ou mot de passe incorrect");
            }
            var token = _jwtService.GenerateToken(client);
            _log.Info($"connection  {client.email} ");
            return Ok(token);
        }
        [HttpPut("{id:length(24)}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, Client updateClient)
        {
            var client = await _clientsServices.GetClientsAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            await _clientsServices.UpdateAsync(id, updateClient);
            return Ok();
        }
        [HttpDelete("{id:length(24)}")]
        [EnableCors("AllowAllOrigins")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(string id)
        {
            var client = await _clientsServices.GetClientsAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            await _clientsServices.RemoveAsync(id);
            return Ok();
        }
        [HttpGet("clientReservation")]
        [Authorize]
        public async Task<ActionResult<List<Vol>>> showMyReservations()
        {
            string client = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _log.Info($"client create for email {client.email} ");
            if (User.IsInRole("client") && client == null)
            {
                return BadRequest("Client ID not found");
            }
            if (User.IsInRole("client") && client != null && client != User.Identity.Name)
            {
                return Unauthorized();
            }
            List<Vol> result = new List<Vol>();
            var reservations = await _reservationServices.GetReservationsClientsAsync(client);
            if (reservations == null)
            {
                return NotFound();
            }
            foreach (Reservation reservation in reservations)
            {
                var flight = await _volsService.GetVolAsync(reservation.vol);
                if (flight == null)
                {
                    return BadRequest("Problème système, contactez-nous");
                }
                result.Add(flight);
            }
            return Ok(result);
        }
    }
}
