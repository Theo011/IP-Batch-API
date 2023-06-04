namespace IP_Batch_API.Models
{
    public class IPDetailForCreationAndUpdateDto
    {
        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Continent { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}