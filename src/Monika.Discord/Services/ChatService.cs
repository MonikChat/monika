using System;
using System.Text;
using System.Threading.Tasks;
using ApiAiSDK;
using ApiAiSDK.Model;
using ApiAiSDK.NETCore;
using Discord;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Monika.Options;
using Newtonsoft.Json;

namespace Monika.Services
{
    public class ChatService
    {
        private readonly ChatServiceOptions _options;
        private readonly IMemoryCache _sessionCache;

        public ChatService(
            IOptions<ChatServiceOptions> options,
            IMemoryCache sessionCache)
        {
            _options = options.Value;
            _sessionCache = sessionCache;
        }

        public void EraseSession(IUser user)
        {
            _sessionCache.Remove(user.Id);
        }

        public async Task<string> GetResponseForUserAsync(IUser user,
            string message)
        {
            ApiAi client = _sessionCache.GetOrCreate(user.Id, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
                return new ApiAi(
                    new AIConfiguration(_options.ApiKey,
                        SupportedLanguage.English));
            });

            var response = await client.TextRequestAsync(message);

            if (response.IsError)
                throw new Exception(response.Status.ErrorDetails);

            return response.Result.Fulfillment.Speech;
        }
    }
}