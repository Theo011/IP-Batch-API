using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IPInfoProvider
{
    public interface IIPInfoProvider
    {
        Task<IPDetails> GetDetails(string ip);
    }

    public interface IPDetails
    {
        string City { get; set; }
        string Country { get; set; }
        string Continent { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
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
                BaseAddress = new Uri("https://api.ipstack.com/")
            };
            this.apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<IPDetails> GetDetails(string ip)
        {
            try
            {
                string uri = $"https://api.ipstack.com/{ip}?access_key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false);

                string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                IPDetails data = JsonConvert.DeserializeObject<IPDetails>(json)!;

                return data;
            }
            catch (HttpRequestException ex)
            {
                throw new IPServiceNotAvailableException("Service is not available.", ex);
            }
        }
    }
}