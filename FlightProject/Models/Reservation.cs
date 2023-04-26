using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FlightProject.Models
{
    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string client { get; set; }
        public string  vol { get; set; }
    }
}
