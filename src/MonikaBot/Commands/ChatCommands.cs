using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Options;
using Monika.Options;

namespace Monika.Commands
{
    using System;
    using System.Collections.Generic;
    using Discord;
    using static Extensions.StringExtensions;

    public class ChatCommands : ModuleBase
    {
        private readonly ChatOptions _options;
        private readonly Random _random;

        public ChatCommands(IOptions<ChatOptions> options)
        {
            _options = options.Value;
            _random = new Random();
        }

        [Command("chat")]
        [Alias("say", "c", "s")]
        [Priority(-1)]
        public async Task ChatAsync([Remainder]string text)
        {
            // Convert to lowercase because it's easier to match against
            text = text.ToLower();

            var validResponses =
                (from option in _options.Messages
                where text.ContainsAny(option.Triggers)
                from response in option.Responses
                where !response.RequiresGuild || Context.Guild != null
                select response.Text).ToArray();

            if (validResponses.Length > 0)
            {
                var response = validResponses[_random.Next(validResponses.Length)];

                response = response.NamedFormat(new Dictionary<string, string>
                {
                    ["{user}"] = Context.User.Mention,
                    ["{guild}"] = Context.Guild?.Name,
                    ["{channel}"] = (Context.Channel as IMentionable)?.Mention
                });

                await ReplyAsync(response);
            }
            else
            {
                await ReplyAsync(
                    $"Sorry {Context.User.Mention}, I don't know what " +
                    "you're asking!");
            }
        }
    }
}