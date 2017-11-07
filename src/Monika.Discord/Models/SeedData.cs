using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Monika.Models
{
    static class SeedData
    {
        public static void Seed(MonikaDbContext dbContext)
        {
            dbContext.Database.Migrate();

            if (dbContext.ChatLines.Any())
                return;

            var options = JsonConvert.DeserializeObject<IList<Option>>(
                File.ReadAllText("seeddata.json"));

            var lines = options.Select(x => new ChatLine
            {
                Triggers = x.Triggers.Select(y =>
                    new ChatTrigger{
                        Text = y
                    }).ToList(),
                Responses = x.Responses.Select(y =>
                    new ChatResponse{
                        Text = y.Text,
                        SupportedChatTypes = CalculateFlags(y)
                    }).ToList()
            });

            dbContext.ChatLines.AddRange(lines);
            dbContext.SaveChanges();
        }

        private static ChatTypes CalculateFlags(Response response)
        {
            ChatTypes flags = ChatTypes.All;

            if (response.RequiresGuild)
                flags ^= ChatTypes.DM;
            if (response.RequiresDM)
                flags ^= ChatTypes.Guild;

            return flags;
        }

        private class Option
        {
            public IList<string> Triggers { get; set; }
            public IList<Response> Responses { get; set; }
        }
        private class Response
        {
            public string Text { get; set; }

            public bool RequiresGuild { get; set; }
            public bool RequiresDM { get; set; }
        }
    }
}