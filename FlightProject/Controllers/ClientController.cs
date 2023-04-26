using FlightProject.Models;
using FlightProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Operators;
using System.Data;

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
        public ClientController (ClientsService clientsServices, JwtService jwtService, VolsService volsServices, ReservationService reservation)
        {
            _clientsServices = clientsServices;
            _jwtService = jwtService;
            _reservationServices = reservation;
            _volsService = volsServices;
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<List<Client>> Get()
        {
            return await _clientsServices.GetClientsAsync();
        }
        [HttpGet("{id:length(24)}")]
        [Authorize(Roles = "admin")]
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
            return Ok();
        }
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginUser loginUser)
        {
            var client = await _clientsServices.GetClientsbyEmailAsync(loginUser.email);
            if (client == null)
            {
                return NotFound("Email ou mot de passe incorrect");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginUser.password, client.password);
            if (!isPasswordValid)
            {
                return NotFound("Email ou mot de passe incorrect");
            }

            var token = _jwtService.GenerateToken(client);
            return Ok(new { token });
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
        [Authorize]
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
        public async Task<ActionResult<List<Vol>>> showMyReservations(string client)
        {
            List<Vol> result = new List<Vol>();
            var reservations = await _reservationServices.GetReservationsClientsAsync(client);
            if (reservations == null)
            {
                return NotFound();
            }
            foreach (Reservation resevation in reservations)
            {
                var flight = await _volsService.GetVolAsync(resevation.vol);
                if (flight == null)
                {
                    return BadRequest("probleme systeme contact us");
                }
                result.Add(flight);
            }
            return Ok(result);

        }
    }
}
