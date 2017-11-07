using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Commands
{
    public static class SocketUserMessageExtensions
    {
        public static bool HasMentionPrefix(this SocketUserMessage message,
            IUser user, out int argPos)
        
        {
            argPos = 0;
            return message.HasMentionPrefix(user, ref argPos);
        }
    }
}