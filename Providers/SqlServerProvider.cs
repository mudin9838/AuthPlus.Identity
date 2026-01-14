using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthPlus.Identity.Providers;


public sealed class SqlServerProvider : IDatabaseProvider
{
    public void Configure(DbContextOptionsBuilder options, IConfiguration config)
    {
        options.UseSqlServer(config.GetConnectionString("Default"));
    }
}