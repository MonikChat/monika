using System.Collections.Generic;

namespace Monika.Options
{
    public class ChatOptions
    {
        public IList<ChatOption> Messages { get; set; }
    }
    public class ChatOption
    {
        public IList<string> Triggers { get; set; }
        public IList<ChatResponse> Responses { get; set; }
    }

    public class ChatResponse
    {
        public string Text { get; set; }

        public bool RequiresGuild { get; set; }
    }
}