using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthPlus.Identity.Providers;

public interface IDatabaseProvider
{
    void Configure(DbContextOptionsBuilder options, IConfiguration configuration);
}