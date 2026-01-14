using AuthPlus.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthPlus.Identity.Services;

public class PostgresProvider : IDatabaseProvider
{
    public void Configure(DbContextOptionsBuilder options, IConfiguration config)
    {
        options.UseNpgsql(config.GetConnectionString("Default"));
    }
}