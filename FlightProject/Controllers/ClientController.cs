using FlightProject.Models;
using FlightProject.Services;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Operators;
using System.Web.Http.Cors;

namespace FlightProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ClientsService _clientsServices;
        private readonly JwtService _jwtService;
        public ClientController (ClientsService clientsServices, JwtService jwtService)
        {
            _clientsServices = clientsServices;
            _jwtService = jwtService;
        }
        [HttpGet]
        public async Task<List<Client>> Get()
        {
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
        [HttpPost("Register")]
        [EnableCors(origins: "http://www.example.com", headers: "*", methods: "*")]
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
            return Ok(token);
        }
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Client updateClient)
        {
           var client = _clientsServices.GetClientsAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            await _clientsServices.UpdateAsync(id, updateClient);
            return Ok();
        }
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var client = _clientsServices.GetClientsAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            await _clientsServices.RemoveAsync(id);
            return Ok();
        }
    }
}
