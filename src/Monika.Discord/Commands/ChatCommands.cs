using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monika.Models;
using Monika.Options;

using static Monika.Extensions.StringExtensions;

namespace Monika.Commands
{
    public class ChatCommands : ModuleBase
    {
        private readonly MonikaDbContext _dbContext;
        private readonly Random _random;

        public ChatCommands(MonikaDbContext dbContext)
        {
            _dbContext = dbContext;
            _random = new Random();
        }

        [Command("chat")]
        [Alias("say", "c", "s")]
        [Summary("Chat with Monika")]
        public async Task ChatAsync([Remainder]string text)
        {
            ChatTypes flags = ChatTypes.All;
            if (Context.Guild == null)
                flags ^= ChatTypes.Guild;
            else
                flags ^= ChatTypes.DM;

            // Convert to lowercase because it's easier to match against
            text = text.ToLower();

            var validResponses = _dbContext.ChatTriggers
                .Include(x => x.ChatLine)
                .ThenInclude(x => x.Responses)
                .Where(x => text.Contains(x.Text))
                .SelectMany(x => x.ChatLine.Responses)
                .Where(x => (x.SupportedChatTypes & flags) == flags)
                .ToArray();

            if (validResponses.Length > 0)
            {
                var response = validResponses[
                    _random.Next(validResponses.Length)];

                var responseText = response.Text
                    .NamedFormat(new Dictionary<string, string>
                {
                    ["{user}"] = Context.User.Mention,
                    ["{guild}"] = Context.Guild?.Name,
                    ["{channel}"] = (Context.Channel as IMentionable)?.Mention
                });

                await ReplyAsync(responseText);
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