using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monika.Options;
using Monika.Services;

using static Monika.Extensions.StringExtensions;

namespace Monika.Commands
{
    public class ChatCommands : ModuleBase
    {
        private readonly ChatService _chatApi;
        private readonly ILogger _logger;
        private readonly Optional<string> Username;

        public ChatCommands(
            ChatService chatApi,
            ILogger<ChatCommands> logger)
        {
            _chatApi = chatApi;
            _logger = logger;
        }

        [Command("chat", RunMode = RunMode.Async)]
        [Alias("say", "c", "s")]
        [Summary("Chat with Monika")]
        public async Task ChatAsync([Remainder]string text)
        {
            try
            {
                // Ignore the text param as it contains unresolved text. We do
                // manual resolving here.
                var content = Context.Message.Resolve(
                    userHandling: TagHandling.FullName)
                    .Substring($"<@{Context.Client.CurrentUser.Id}> c ".Length);

                await ReplyAsync(
                    await _chatApi.GetResponseForUserAsync(
                        Context.User, content));
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Could not make the request");
                await ReplyAsync(
                    $"Sorry {Context.User.Mention}, I don't know what " +
                    "you're asking!");
            }
        }
    }
}