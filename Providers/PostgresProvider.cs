using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthPlus.Identity.Providers;

public sealed class PostgresProvider : IDatabaseProvider
{
    public void Configure(DbContextOptionsBuilder options, IConfiguration config)
    {
        options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
    }
}