using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthPlus.Identity.Interfaces;

public interface IDatabaseProvider
{
    void Configure(DbContextOptionsBuilder options, IConfiguration configuration);
}