﻿namespace FlightProject.Models
{
    public class JwtSettign
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int ExpirationMinutes { get; set; }
    }
}
