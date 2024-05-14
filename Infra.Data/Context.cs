using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data
{
    [ExcludeFromCodeCoverage]
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
