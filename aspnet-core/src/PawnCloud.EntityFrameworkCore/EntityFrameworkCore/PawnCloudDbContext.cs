using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PawnCloud.Authorization.Roles;
using PawnCloud.Authorization.Users;
using PawnCloud.BasicCodes;
using PawnCloud.Countries;
using PawnCloud.Customers;
using PawnCloud.DailyGoldPrices;
using PawnCloud.GeneralSetups;
using PawnCloud.GoldTypes;
using PawnCloud.ItemListings;
using PawnCloud.ItemStatuses;
using PawnCloud.MiscMasterConfigs;
using PawnCloud.MultiTenancy;
using PawnCloud.PawnItems;

namespace PawnCloud.EntityFrameworkCore;

public class PawnCloudDbContext : AbpZeroDbContext<Tenant, Role, User, PawnCloudDbContext>
{
    /* Define a DbSet for each entity of the application */
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<CustomerDocument> CustomerDocuments { get; set; }
    public virtual DbSet<PawnCloud.PawnTickets.PawnTicket> PawnTickets { get; set; }
    public virtual DbSet<PawnCloud.Loans.Loan> Loans { get; set; }
    public virtual DbSet<GoldType> GoldTypes { get; set; }
    public virtual DbSet<DailyGoldPrice> DailyGoldPrices { get; set; }
    public virtual DbSet<MiscMasterConfig> MiscMasterConfigs { get; set; }
    public virtual DbSet<BasicCode> BasicCodes { get; set; }
    public virtual DbSet<ItemStatus> ItemStatuses { get; set; }
    public virtual DbSet<ItemListing> ItemListings { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<PawnItem> PawnItems { get; set; }
    public virtual DbSet<GeneralSetup> GeneralSetups { get; set; }

    public PawnCloudDbContext(DbContextOptions<PawnCloudDbContext> options)
        : base(options)
    {
    }
}
