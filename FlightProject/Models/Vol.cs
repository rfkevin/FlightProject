using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FlightProject.Models
{
    public class Vol
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string numeroDeVol { get; set; }
        public string villeDepart { get; set; }
        public string villeArriv { get; set; }
        public string heureDepart { get; set; }
        public string heureArriv { get; set; }
        public string identifiantAvion { get; set; }
        public int nbrPlace { get; set; }
        public int placeUsed { get; set; }

        public List<string> passagers { get; set; }

    }
}
