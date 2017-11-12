using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Monika.Commands
{
    [Name("Help")]
    [Group("help")]
    [Summary("Help with using Monika")]
    public class HelpCommands : ModuleBase
    {
        private readonly CommandService _commands;

        public HelpCommands(CommandService commands)
        {
            _commands = commands;
        }

        [Command]
        [Name("Command Help")]
        [Summary("Retrieves help for the given command")]
        public async Task GenerateHelpAsync([Remainder]string query)
        {
            var matchingModules = _commands.Commands
                .Where(x => x.Aliases.Any(y => y.StartsWith(query)))
                .GroupBy(x => x.Module);

            StringBuilder responseBuilder = new StringBuilder();

            foreach (var module in matchingModules)
            {
                responseBuilder.AppendLine(
                    $"{module.Key.Name}: {module.Key.Summary}");
                foreach (var command in module)
                {
                    responseBuilder.AppendLine(
                        $"\t{command.Name} ({command.Aliases.First()}): " +
                        $"{command.Summary}"
                    );
                }
            }

            await ReplyAsync(
                Format.Code(responseBuilder.ToString()));
        }
    }
}