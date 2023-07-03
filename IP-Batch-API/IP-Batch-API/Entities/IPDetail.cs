using System.ComponentModel.DataAnnotations;

namespace IP_Batch_API.Entities
{
    public class IPDetail
    {
        [Key]
        public string Ip { get; set; }

        public string? City { get; private set; }

        public string? Country { get; private set; }

        public string? Continent { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public IPDetail(string ip, string? city, string? country, string? continent, double latitude, double longitude)
        {
            Ip = ip;
            City = city;
            Country = country;
            Continent = continent;
            Latitude = latitude;
            Longitude = longitude;
        }

        public IPDetail() { }
    }
}