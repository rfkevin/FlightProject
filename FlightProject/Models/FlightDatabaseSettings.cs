namespace FlightProject.Models
{
    public class FlightDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public string ClientCollectionName { get; set; }

        public string VolCollectionName { get; set;}

        public string ReservationCollectionName { get; set; }
    }
}
