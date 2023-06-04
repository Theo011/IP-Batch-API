using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IPInfoProvider
{
    public interface IIPInfoProvider
    {
        Task<IPDetails> GetDetails(string ip);
    }

    public class IPDetails
    {
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("country_name")]
        public string? Country { get; set; }

        [JsonPropertyName("continent_name")]
        public string? Continent { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    public class IPServiceNotAvailableException : ApplicationException
    {
        public IPServiceNotAvailableException(string message, Exception inner) : base(message, inner) { }
    }

    public class IPInfoProvider : IIPInfoProvider
    {
        private readonly HttpClient client;
        private readonly string apiKey;

        public IPInfoProvider(string apiKey)
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("http://api.ipstack.com/")
            };
            this.apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<IPDetails> GetDetails(string ip)
        {
            try
            {
                string uri = $"http://api.ipstack.com/{ip}?access_key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false);

                string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                IPDetails data = JsonSerializer.Deserialize<IPDetails>(json)!;

                /*#region debug
                string logContent = $"apiKey: {apiKey}, ipDetails: {data.ToString()}, response: {json}";

                using (StreamWriter fileStream = new("LogFile.txt", true))
                {
                    await fileStream.WriteLineAsync(logContent);
                }
                #endregion*/

                return data;
            }
            catch (HttpRequestException ex)
            {
                throw new IPServiceNotAvailableException("Service is not available.", ex);
            }
        }
    }
}