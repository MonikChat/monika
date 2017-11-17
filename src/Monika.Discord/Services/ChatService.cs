using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Options;
using Monika.Options;
using Newtonsoft.Json;

namespace Monika.Services
{
    public class ChatService
    {
        private readonly HttpClient _apiClient;

        private readonly string _projectId;

        public ChatService(IOptions<ChatServiceOptions> options)
        {
            _apiClient = new HttpClient();
            _apiClient.BaseAddress =
                new Uri("https://dialogflow.googleapis.com/v2beta1/");
            _apiClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", options.Value.ApiKey);

            _projectId = options.Value.ProjectId;
        }

        public async Task<string> GetResponseForUserAsync(IUser user,
            string message, string language = "en")
        {
            var payload = new
            {
                queryInput = new
                {
                    text = new
                    {
                        text = message,
                        languageCode = language
                    }
                }
            };

            var body = JsonConvert.SerializeObject(payload);
            var content = new StringContent(body, Encoding.UTF8,
                "application/json");
            var response = await _apiClient.PostAsync(
                $"projects/{_projectId}/agent/sessions/{user.Id}:detectIntent",
                content)
                .ConfigureAwait(false);

            var responseString = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            var responsePayload =
                JsonConvert.DeserializeObject<DetectIntentResult>(
                    responseString);

            if (responsePayload.Error != null)
                throw new Exception(responsePayload.Error.Message);

            return responsePayload?.QueryResult?.FulfillmentText;
        }

        private class DetectIntentResult
        {
            [JsonProperty("responseId")]
            public string ResponseId { get; set; }

            [JsonProperty("queryResult")]
            public QueryResult QueryResult { get; set; }

            [JsonProperty("error")]
            public ErrorResult Error { get; set; }
        }

        private class QueryResult
        {
            [JsonProperty("action")]
            public string Action { get; set; }

            [JsonProperty("fulfillmentText")]
            public string FulfillmentText { get; set; }
        }

        private class ErrorResult
        {
            [JsonProperty("code")]
            public int ErrorCode { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}