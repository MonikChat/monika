using System;
using Microsoft.EntityFrameworkCore;

namespace Monika.Models
{
    public class MonikaDbContext : DbContext
    {
        [DbFunction("RANDOM")]
        public static int Random()
            => throw new NotImplementedException();

        public MonikaDbContext(DbContextOptions<MonikaDbContext> options)
            : base(options)
        { }

        public DbSet<ChatLine> ChatLines { get; set; }
        public DbSet<ChatResponse> ChatResponses { get; set; }
        public DbSet<ChatTrigger> ChatTriggers { get; set; }
    }
}