using FlightProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FlightProject.Services
{
    public class ReservationService
    {
        private readonly IMongoCollection<Reservation> _reservations;

        public ReservationService(IOptions<FlightDatabaseSettings> flightDatabaseSettings) { 
            var mongoClient = new MongoClient(flightDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(flightDatabaseSettings.Value.DatabaseName);
            _reservations = mongoDatabase.GetCollection<Reservation>(flightDatabaseSettings.Value.ReservationCollectionName);

        }

        public async Task<List<Reservation>> GetReservationsAsync()
        {
            return await _reservations.Find(_ => true).ToListAsync();
        }
        public async Task<Reservation?> GetReservationAsync(string Id)
        {
            return await _reservations.Find(x => x.Id == Id).FirstOrDefaultAsync();
        }
        public async Task<Reservation?> CheckReservation(string vol, string client)
        {
            return await _reservations.Find(u => (u.vol == vol & u.client == client)).FirstOrDefaultAsync();
        }
        public async Task CreateReservation(Reservation reservation)
        {
            await _reservations.InsertOneAsync(reservation);
        }
        public async Task UpdateReservation(string Id, Reservation reservation)
        {
            await _reservations.FindOneAndReplaceAsync(Id, reservation);
        }
        public async Task DeleteReservation(string Id)
        {
            await _reservations.FindOneAndDeleteAsync(Id);
        }
    }
}
