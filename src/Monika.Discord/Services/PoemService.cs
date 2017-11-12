using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Monika.Options;
using Newtonsoft.Json;

namespace Monika.Services
{
    public class PoemService : IDisposable
    {
        private readonly HttpClient _apiClient;

        public PoemService(IOptions<PoemServiceOptions> options)
        {
            _apiClient = new HttpClient();
            _apiClient.BaseAddress = options.Value.BaseAddress;
        }

        public async Task<Stream> GenerateImageAsync(string contents,
            string font)
        {
            var data = new PoemData
            {
                Poem = contents,
                Font = font
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(data),
                Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync("generate", content)
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStreamAsync()
                    .ConfigureAwait(false);
            else
            {
                var error = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);
                throw new InvalidDataException(
                    $"Could not generate a poem: {error}");
            }
        }

        public void Dispose()
        {
            _apiClient.Dispose();
        }

        private class PoemData
        {
            [JsonProperty("poem")]
            public string Poem { get; set; }
            [JsonProperty("font")]
            public string Font { get; set; }
        }
    }
}