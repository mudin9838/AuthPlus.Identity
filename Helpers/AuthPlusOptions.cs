using AuthPlus.Identity.Providers;

namespace AuthPlus.Identity.Helpers;

public sealed class AuthPlusOptions
{
    internal IDatabaseProvider DatabaseProvider { get; private set; } = null!;

    public void UseSqlServer()
        => DatabaseProvider = new SqlServerProvider();

    public void UsePostgres()
        => DatabaseProvider = new PostgresProvider();

}
