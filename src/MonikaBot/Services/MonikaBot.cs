using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monika.Options;
using Monika.Utilities;

namespace Monika.Services
{
    class MonikaBot : IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandlingService _commands;
        private readonly ILogger _logger;
        private readonly MonikaOptions _options;

        public MonikaBot(
            IOptions<DiscordSocketConfig> discordOptions,
            IOptions<MonikaOptions> options,
            CommandHandlingService commands,
            ILogger<MonikaBot> logger,
            ILogger<DiscordSocketClient> discordLogger)
        {
            _client = new DiscordSocketClient(discordOptions.Value);
            _client.Log += LogWrapper.WrapLogger(discordLogger);
            _commands = commands;
            _logger = logger;
            _options = options.Value;
        }

        public async Task StartAsync()
        {
            await _commands.RegisterAsync(_client);
            await _client.LoginAsync(_options.TokenType, _options.Token);
            await _client.StartAsync();
        }

        public async Task StopAsync()
        {
            await _client.StopAsync();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}