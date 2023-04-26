using FlightProject.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FlightProject.Services
{
    public class ClientsService
    {
        private readonly IMongoCollection<Client> _clientsCollections;
        public ClientsService(IOptions<FlightDatabaseSettings> flightDatabaseSettings)
        {
            var mongoClient = new MongoClient(flightDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(flightDatabaseSettings.Value.DatabaseName);

             _clientsCollections = mongoDatabase.GetCollection<Client>(flightDatabaseSettings.Value.ClientCollectionName);
        }

        public async Task<List<Client>> GetClientsAsync() =>
            await _clientsCollections.Find( _ => true).ToListAsync();
        public async Task<Client?> GetClientsAsync(string id) => 
            await _clientsCollections.Find(c => c.Id == id ).FirstOrDefaultAsync();
        public async Task<Client?> GetClientsAsync(string firstName, string lastName) =>
            await _clientsCollections.Find(u => (u.firstName == firstName & u.lastName == lastName)).FirstOrDefaultAsync();
        public async Task<Client?> GetClientsbyEmailAsync(string email) =>
           await _clientsCollections.Find(u => u.email == email).FirstOrDefaultAsync();
        public async Task CreateAsync(Client client) => 
            await _clientsCollections.InsertOneAsync(client);
        public async Task UpdateAsync(string id, Client updatedClient) => 
            await _clientsCollections.ReplaceOneAsync(id, updatedClient);
        public async Task RemoveAsync(string id) => 
            await _clientsCollections.DeleteOneAsync(id);


    }
}
