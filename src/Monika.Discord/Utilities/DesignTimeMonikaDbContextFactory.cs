using Monika.Models;
using Monika.Setup;

namespace Monika.Utilities
{
    public class DesignTimeMonikaDbContextFactory
        : DesignTimeDbContextFactory<MonikaDbContext>
    { }
}