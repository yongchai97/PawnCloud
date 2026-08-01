using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace PawnCloud.EntityFrameworkCore;

public static class PawnCloudDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<PawnCloudDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<PawnCloudDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
