using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
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

        [Command("guildCount")]
        public async Task GetGuildCountAsync()
        {
            var guilds = Context.Client.Guilds;
            await ReplyAsync(
                $"{guilds.Count} guilds.\n" +
                $"{guilds.Sum(x => x.Channels.Count)} channels.\n" +
                $"{guilds.Sum(x => x.Users.Count)} users total.\n");
        }

        [Command("setAvatar")]
        public async Task SetAvatarAsync()
        {
            var image = Context.Message.Attachments.FirstOrDefault();
            if (image == null)
                await ReplyAsync("I need an image, silly~");
            else
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(image.Url);
                    var body = await response.Content.ReadAsStreamAsync();
                    await Context.Client.CurrentUser.ModifyAsync(x =>
                    {
                        x.Avatar = new Image(body);
                    });
                }
                await ReplyAsync("Done~");
            }
        }
    }
}