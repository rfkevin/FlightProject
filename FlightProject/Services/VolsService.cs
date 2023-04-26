using FlightProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FlightProject.Services
{
    public class VolsService
    {
        private readonly IMongoCollection<Vol> _volCollections;
        public VolsService(IOptions<FlightDatabaseSettings> flightDatabaseSettings)
        {
            var mongoClient = new MongoClient(flightDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(flightDatabaseSettings.Value.DatabaseName);
            _volCollections = mongoDatabase.GetCollection<Vol>(flightDatabaseSettings.Value.VolCollectionName);
        }
        public async Task<List<Vol>> GetVolAsync()
        {
            return await _volCollections.Find( _ => true).ToListAsync();
        }
        public async Task<Vol?> GetVolAsync(string Id)
        {
            return await _volCollections.Find(c => c.Id == Id).FirstOrDefaultAsync();
        }
        public async Task CreateVolAsync(Vol vol)
        {
            await _volCollections.InsertOneAsync(vol);
        }
        public async Task UpdateVolAsync(string Id, Vol vol)
        {
            await _volCollections.ReplaceOneAsync(Id, vol);
        }
        public async Task DeleteVolAsync(string Id)
        {
            await _volCollections.FindOneAndDeleteAsync(Id);
        }
    }
}
