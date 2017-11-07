using Microsoft.EntityFrameworkCore;

namespace Monika.Models
{
    public class MonikaDbContext : DbContext
    {
        public MonikaDbContext(DbContextOptions<MonikaDbContext> options)
            : base(options)
        { }

        public DbSet<ChatLine> ChatLines { get; set; }
        public DbSet<ChatResponse> ChatResponses { get; set; }
        public DbSet<ChatTrigger> ChatTriggers { get; set; }
    }
}