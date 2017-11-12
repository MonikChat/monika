using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Monika.Services;

namespace Monika.Commands
{
    [Group("poem")]
    public class PoemCommands : ModuleBase
    {
        private readonly PoemService _generator;

        public PoemCommands(PoemService generator)
        {
            _generator = generator;
        }

        [Command("monika", RunMode = RunMode.Async)]
        public Task GenerateMonikaPoem([Remainder] string text)
            => GeneratePoemAsync(text, "m1");

        [Command("sayori", RunMode = RunMode.Async)]
        public Task GenerateSayoriPoem([Remainder] string text)
            => GeneratePoemAsync(text, "s1");

        [Command("natsuki", RunMode = RunMode.Async)]
        public Task GenerateNatsukiPoem([Remainder] string text)
            => GeneratePoemAsync(text, "n1");

        [Command("yuri normal", RunMode = RunMode.Async)]
        public Task GenerateYuriNormalPoem([Remainder] string text)
            => GeneratePoemAsync(text, "y1");

        [Command("yuri fast", RunMode = RunMode.Async)]
        public Task GenerateYuriFastPoem([Remainder] string text)
            => GeneratePoemAsync(text, "y2");

        [Command("yuri obsessed", RunMode = RunMode.Async)]
        public Task GenerateYuriObsesseddPoem([Remainder] string text)
            => GeneratePoemAsync(text, "y3");

        private async Task GeneratePoemAsync(string text, string font)
        {
            try
            {
                var response = await _generator
                    .GenerateImageAsync(text, font);
                await Context.Channel.SendFileAsync(response, "poem.png",
                    $"Hi {Context.User.Mention}! Here's your poem~");
            }
            catch (InvalidDataException)
            {
                await ReplyAsync(
                    $"Sorry {Context.User.Mention}! Something went wrong " +
                    "and I couldn't get that poem written for you!");

                // re-throw the exception so that we can log it
                throw;
            }
        }
    }
}