using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monika.Utilities;

namespace Monika.Services
{
    class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public CommandHandlingService(IOptions<CommandServiceConfig> options,
            ILogger<CommandHandlingService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _commands = new CommandService(options.Value);
            _commands.Log += LogWrapper.WrapLogger(logger);
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task RegisterAsync(DiscordSocketClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

            client.MessageReceived += (x) => HandleMessageReceived(x, client);
        }

        private async Task HandleMessageReceived(SocketMessage message,
            DiscordSocketClient client)
        {
            if (message is SocketUserMessage userMessage)
            {
                if (userMessage.HasMentionPrefix(client.CurrentUser,
                    out int argPos))
                {
                    if (message.Channel is SocketGuildChannel guildChannel)
                        _logger.LogTrace(
                            "{User} in {Guild}/{Channel}: {Message}",
                            message.Author.Username,
                            guildChannel.Guild.Name, guildChannel.Name,
                            message.Content);
                    else
                        _logger.LogTrace(
                            "{User} in DMs: {Message}",
                            message.Author.Username,
                            message.Content);

                    var context = new SocketCommandContext(client,
                        userMessage);
                    using (var scope = _scopeFactory.CreateScope())
                        await _commands.ExecuteAsync(context, argPos,
                            scope.ServiceProvider);
                }
            }
        }
    }
}