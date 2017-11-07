using System.Collections.Generic;

namespace Monika.Models
{
    public class ChatResponse
    {
        public ulong Id { get; set; }
        
        public string Text { get; set; }
        public ChatLine ChatLine { get; set; }

        public ChatTypes SupportedChatTypes { get; set; }
    }
}