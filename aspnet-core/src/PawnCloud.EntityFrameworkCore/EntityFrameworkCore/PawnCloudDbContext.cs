using Abp.Zero.EntityFrameworkCore;
using PawnCloud.Authorization.Roles;
using PawnCloud.Authorization.Users;
using PawnCloud.MultiTenancy;
using PawnCloud.Customers;
using Microsoft.EntityFrameworkCore;

namespace PawnCloud.EntityFrameworkCore;

public class PawnCloudDbContext : AbpZeroDbContext<Tenant, Role, User, PawnCloudDbContext>
{
    /* Define a DbSet for each entity of the application */
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<CustomerDocument> CustomerDocuments { get; set; }
    public virtual DbSet<PawnCloud.PawnTickets.PawnTicket> PawnTickets { get; set; }
    public virtual DbSet<PawnCloud.PawnItems.PawnItem> PawnItems { get; set; }
    public virtual DbSet<PawnCloud.Loans.Loan> Loans { get; set; }

    public PawnCloudDbContext(DbContextOptions<PawnCloudDbContext> options)
        : base(options)
    {
    }
}
