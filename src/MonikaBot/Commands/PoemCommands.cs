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

        [Command("monika")]
        public Task GenerateMonikaPoem([Remainder] string text)
            => GeneratePoemAsync(text, "m1");

        [Command("sayori")]
        public Task GenerateSayoriPoem([Remainder] string text)
            => GeneratePoemAsync(text, "s1");

        [Command("natsuki")]
        public Task GenerateNatsukiPoem([Remainder] string text)
            => GeneratePoemAsync(text, "n1");

        [Command("yuri normal")]
        public Task GenerateYuriNormalPoem([Remainder] string text)
            => GeneratePoemAsync(text, "y1");

        [Command("yuri fast")]
        public Task GenerateYuriFastPoem([Remainder] string text)
            => GeneratePoemAsync(text, "y2");

        [Command("yuri obsessed")]
        public Task GenerateYuriObsesseddPoem([Remainder] string text)
            => GeneratePoemAsync(text, "y3");

        private async Task GeneratePoemAsync(string text, string font)
        {
            text = text.Replace("&", "&amp;");
            var response = await _generator.GenerateImageAsync(text, font);
            await Context.Channel.SendFileAsync(response, "poem.png",
                $"Hi {Context.User.Mention}! Here's your poem~");
        }
    }
}