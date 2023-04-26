using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FlightProject.Models
{
    public class Client
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string birthday { get; set; }
        public string numeroPasseport { get; set; }

        public bool admin = false;



    }
}
