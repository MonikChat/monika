using System.Collections.Generic;

namespace Monika.Models
{
    public class ChatLine
    {
        public ulong Id { get; set; }

        public IList<ChatTrigger> Triggers { get; set; }
        public IList<ChatResponse> Responses { get; set; }
    }
}