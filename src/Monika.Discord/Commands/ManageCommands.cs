using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Monika.Models;

namespace Monika.Commands
{
    [Group("manage")]
    [RequireOwner]
    public class ManageCommands : ModuleBase
    {
        private readonly MonikaDbContext _dbContext;

        public ManageCommands(MonikaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Command("trigger add")]
        public async Task AddTrigger([Remainder]string text)
        {
            var trigger = new ChatTrigger
            {
                Text = text
            };

            _dbContext.ChatTriggers.Add(trigger);
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Added trigger id `{trigger.Id}`.");
        }

        [Command("trigger edit text")]
        public async Task EditTriggerTextAsync(ulong id, [Remainder]string text)
        {
            var trigger = _dbContext.ChatTriggers
                .FirstOrDefault(x => x.Id == id);

            if (trigger == null)
            {
                await ReplyAsync("No such trigger");
                return;
            }

            trigger.Text = text;
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Updated trigger `{trigger.Text}`.");
        }

        [Command("trigger edit line")]
        public async Task EditTriggerLineAsync(ulong id, ulong lineId)
        {
            var trigger = _dbContext.ChatTriggers
                .FirstOrDefault(x => x.Id == id);
            var line = _dbContext.ChatLines
                .FirstOrDefault(x => x.Id == lineId);

            if (trigger == null)
            {
                await ReplyAsync("No such trigger");
                return;
            }
            if (line == null)
            {
                await ReplyAsync("No such line");
                return;
            }

            trigger.ChatLine = line;
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Updated trigger `{trigger.Text}`.");
        }

        [Command("trigger del")]
        public async Task DelTrigger(ulong id)
        {
            var trigger = _dbContext.ChatTriggers
                .FirstOrDefault(x => x.Id == id);

            if (trigger == null)
            {
                await ReplyAsync("No such trigger");
                return;
            }

            _dbContext.ChatTriggers.Remove(trigger);
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Removed trigger `{trigger.Text}`.");
        }

        [Command("response add")]
        public async Task AddResponse(ChatTypes chatTypes, [Remainder]string text)
        {
            var response = new ChatResponse
            {
                Text = text,
                SupportedChatTypes = chatTypes
            };

            _dbContext.ChatResponses.Add(response);
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Added response id `{response.Id}`.");
        }

        [Command("response edit text")]
        public async Task EditResponseText(ulong id, [Remainder] string text)
        {
            var response = _dbContext.ChatResponses
                .FirstOrDefault(x => x.Id == id);

            if (response == null)
            {
                await ReplyAsync("No such response");
                return;
            }

            response.Text = text;
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Updated response `{response.Text}`.");
        }

        [Command("response edit ctypes")]
        public async Task EditResponseText(ulong id, params ChatTypes[] chatTypes)
        {
            var response = _dbContext.ChatResponses
                .FirstOrDefault(x => x.Id == id);

            if (response == null)
            {
                await ReplyAsync("No such response");
                return;
            }

            ChatTypes types = chatTypes[0];
            foreach (var type in chatTypes)
                types |= type;

            response.SupportedChatTypes = types;
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Updated response `{response.Text}`.");
        }

        [Command("response edit line")]
        public async Task EditResponseLine(ulong id, ulong lineId)
        {
            var response = _dbContext.ChatResponses
                .FirstOrDefault(x => x.Id == id);
            var line = _dbContext.ChatLines
                .FirstOrDefault(x => x.Id == lineId);

            if (response == null)
            {
                await ReplyAsync("No such response");
                return;
            }
            if (line == null)
            {
                await ReplyAsync("No such line");
                return;
            }

            response.ChatLine = line;
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Updated response `{response.Text}`.");
        }

        [Command("response del")]
        public async Task DelResponse(ulong id)
        {
            var response = _dbContext.ChatResponses
                .FirstOrDefault(x => x.Id == id);

            if (response == null)
            {
                await ReplyAsync("No such trigger");
                return;
            }

            _dbContext.ChatResponses.Remove(response);
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Removed response `{response.Text}`.");
        }

        [Command("line add")]
        public async Task AddLine(ulong triggerId, ulong responseId)
        {
            var triggers = _dbContext.ChatTriggers
                .Where(x => x.Id == triggerId);
            var responses = _dbContext.ChatResponses
                .Where(x => x.Id == responseId);

            if (!triggers.Any())
            {
                await ReplyAsync("No such trigger");
                return;
            }
            if (!responses.Any())
            {
                await ReplyAsync("No such response");
                return;
            }

            var line = new ChatLine
            {
                Triggers = triggers.ToList(),
                Responses = responses.ToList()
            };

            _dbContext.ChatLines.Add(line);
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Added line id `{line.Id}`.");
        }

        [Command("line del")]
        public async Task DelLine(ulong id)
        {
            var line = _dbContext.ChatLines
                .Include(x => x.Triggers)
                .Include(x => x.Responses)
                .FirstOrDefault(x => x.Id == id);

            if (line == null)
            {
                await ReplyAsync("No such line");
                return;
            }

            foreach (var trigger in line.Triggers)
                trigger.ChatLine = null;
            foreach (var response in line.Responses)
                response.ChatLine = null;

            _dbContext.ChatLines.Remove(line);
            await _dbContext.SaveChangesAsync();

            await ReplyAsync($"Removed line id `{line.Id}`.");
        }
    }
}