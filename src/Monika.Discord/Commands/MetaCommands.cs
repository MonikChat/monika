using System.Threading.Tasks;
using Discord.Commands;

namespace Monika.Commands
{
    [Group("meta")]
    [RequireOwner]
    public class MetaCommands : ModuleBase<SocketCommandContext>
    {
        [Command("setNick")]
        public async Task SetNickAsync([Remainder]string nickname)
        {
            await Context.Guild.CurrentUser.ModifyAsync(
                x => x.Nickname = nickname);

            await ReplyAsync("Done~");
        }
    }
}