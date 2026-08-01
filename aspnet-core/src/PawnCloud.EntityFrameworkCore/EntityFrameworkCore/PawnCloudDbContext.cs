using Abp.Zero.EntityFrameworkCore;
using PawnCloud.Authorization.Roles;
using PawnCloud.Authorization.Users;
using PawnCloud.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace PawnCloud.EntityFrameworkCore;

public class PawnCloudDbContext : AbpZeroDbContext<Tenant, Role, User, PawnCloudDbContext>
{
    /* Define a DbSet for each entity of the application */

    public PawnCloudDbContext(DbContextOptions<PawnCloudDbContext> options)
        : base(options)
    {
    }
}
