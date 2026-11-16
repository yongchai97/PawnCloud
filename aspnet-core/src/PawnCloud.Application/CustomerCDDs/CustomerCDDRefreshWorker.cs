using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PawnCloud.Customers;
using PawnCloud.GlobalBlackLists;
using PawnCloud.MultiTenancy;
using PawnCloud.PrivateBlackLists;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PawnCloud.CustomerCDDs;

public class CustomerCDDRefreshWorker : AsyncPeriodicBackgroundWorkerBase
{
    private const int RefreshPeriodMilliseconds = 30 * 60 * 1000;

    private readonly IRepository<Tenant, int> _tenantRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<CustomerCDD> _customerCDDRepository;
    private readonly IRepository<PrivateBlackList> _privateBlackListRepository;
    private readonly IRepository<GlobalBlackList> _globalBlackListRepository;
    private readonly IConfiguration _configuration;

    public CustomerCDDRefreshWorker(
        AbpAsyncTimer timer,
        IRepository<Tenant, int> tenantRepository,
        IRepository<Customer> customerRepository,
        IRepository<CustomerCDD> customerCDDRepository,
        IRepository<PrivateBlackList> privateBlackListRepository,
        IRepository<GlobalBlackList> globalBlackListRepository,
        IConfiguration configuration)
        : base(timer)
    {
        _tenantRepository = tenantRepository;
        _customerRepository = customerRepository;
        _customerCDDRepository = customerCDDRepository;
        _privateBlackListRepository = privateBlackListRepository;
        _globalBlackListRepository = globalBlackListRepository;
        _configuration = configuration;
        Timer.Period = RefreshPeriodMilliseconds;
        Timer.RunOnStart = false;
    }

    protected override async Task DoWorkAsync()
    {
        try
        {
            var snapshot = LoadSanctionSnapshot();
            List<int> tenantIds;
            List<string> globalBlacklistNames;

            using (var discoveryUnitOfWork = UnitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    tenantIds = await _tenantRepository.GetAll()
                        .Select(tenant => tenant.Id)
                        .ToListAsync();

                    globalBlacklistNames = (await _globalBlackListRepository.GetAllListAsync(x => x.TenantId == null))
                        .Select(x => NormalizeName(x.FullName))
                        .Where(x => x.Length > 0)
                        .Distinct()
                        .ToList();
                }

                await discoveryUnitOfWork.CompleteAsync();
            }

            await RefreshScopeAsync(null, globalBlacklistNames, snapshot);
            foreach (var tenantId in tenantIds)
            {
                try
                {
                    await RefreshScopeAsync(tenantId, globalBlacklistNames, snapshot);
                }
                catch (Exception exception)
                {
                    Logger.Error($"Customer CDD refresh failed for tenant {tenantId}.", exception);
                }
            }
        }
        catch (Exception exception)
        {
            Logger.Error("Customer CDD refresh failed.", exception);
        }
    }

    private async Task RefreshScopeAsync(int? tenantId, IReadOnlyCollection<string> globalBlacklistNames, SanctionSnapshot snapshot)
    {
        using (var unitOfWork = UnitOfWorkManager.Begin())
        using (CurrentUnitOfWork.SetTenantId(tenantId))
        {
            var customers = await _customerRepository.GetAllListAsync();
            var customersById = customers.ToDictionary(customer => customer.Id);
            var customerCDDs = await _customerCDDRepository.GetAllListAsync();
            var privateBlacklistNames = (await _privateBlackListRepository.GetAllListAsync())
                .Select(x => NormalizeName(x.FullName))
                .Where(x => x.Length > 0)
                .ToHashSet();
            var globalBlacklistNameSet = globalBlacklistNames.ToHashSet();

            foreach (var customerCDD in customerCDDs)
            {
                if (!customerCDD.Customer.HasValue || !customersById.TryGetValue(customerCDD.Customer.Value, out var customer))
                {
                    continue;
                }

                var changed = false;
                var customerName = customer.FullName ?? string.Empty;
                var normalizedCustomerName = NormalizeName(customerName);
                var appearsInPrivateBlacklist = privateBlacklistNames.Contains(normalizedCustomerName);
                var appearsInPublicBlacklist = globalBlacklistNameSet.Contains(normalizedCustomerName);

                if (customerCDD.appearInPrivateBlackList != appearsInPrivateBlacklist)
                {
                    customerCDD.appearInPrivateBlackList = appearsInPrivateBlacklist;
                    changed = true;
                }

                if (customerCDD.appearInPublicBlackList != appearsInPublicBlacklist)
                {
                    customerCDD.appearInPublicBlackList = appearsInPublicBlacklist;
                    changed = true;
                }

                if (snapshot.IsAvailable && !HasExistingSanctionMatch(customerCDD))
                {
                    var sanctionMatch = FindSanctionMatch(customerName, snapshot);
                    if (customerCDD.MOHAMatching != sanctionMatch.MOHAMatching ||
                        customerCDD.UNSCRMatching != sanctionMatch.UNSCRMatching ||
                        customerCDD.otherMatching != sanctionMatch.OtherMatching ||
                        !string.Equals(customerCDD.matchingID ?? string.Empty, sanctionMatch.MatchingId, StringComparison.Ordinal) ||
                        !string.Equals(customerCDD.matchingDescription ?? string.Empty, sanctionMatch.MatchingDescription, StringComparison.Ordinal))
                    {
                        customerCDD.MOHAMatching = sanctionMatch.MOHAMatching;
                        customerCDD.UNSCRMatching = sanctionMatch.UNSCRMatching;
                        customerCDD.otherMatching = sanctionMatch.OtherMatching;
                        customerCDD.matchingID = sanctionMatch.MatchingId;
                        customerCDD.matchingDescription = sanctionMatch.MatchingDescription;
                        changed = true;
                    }
                }

                if (changed)
                {
                    await _customerCDDRepository.UpdateAsync(customerCDD);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            await unitOfWork.CompleteAsync();
        }
    }

    private SanctionSnapshot LoadSanctionSnapshot()
    {
        var rootPath = _configuration["ExternalXml:FolderPath"];
        var folderPath = FindRecentFolder(rootPath);
        if (folderPath == null)
        {
            return new SanctionSnapshot();
        }

        return new SanctionSnapshot
        {
            Moha = LoadDocument(folderPath, "SENARAI_KDN.xml"),
            Unscr = LoadDocument(folderPath, "consolidatedLegacyByNAME.xml"),
            Sanction1988 = LoadDocument(folderPath, "1988sanction.xml"),
            Isil = LoadDocument(folderPath, "ISILsanction.xml"),
            Sanction1718 = LoadDocument(folderPath, "1718sanction.xml"),
        };
    }

    private static string? FindRecentFolder(string? rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            return null;
        }

        for (var daysBack = 0; daysBack < 10; daysBack++)
        {
            var folderPath = Path.Combine(rootPath, DateTime.Now.AddDays(-daysBack).ToString("yyyyMMdd"));
            if (Directory.Exists(folderPath))
            {
                return folderPath;
            }
        }

        return null;
    }

    private static XDocument? LoadDocument(string folderPath, string fileName)
    {
        var filePath = Path.Combine(folderPath, fileName);
        return File.Exists(filePath) ? XDocument.Load(filePath) : null;
    }

    private static bool HasExistingSanctionMatch(CustomerCDD customerCDD)
    {
        return customerCDD.MOHAMatching || customerCDD.UNSCRMatching || customerCDD.otherMatching;
    }

    private static SanctionMatch FindSanctionMatch(string customerName, SanctionSnapshot snapshot)
    {
        var match = new SanctionMatch();
        if (string.IsNullOrWhiteSpace(customerName))
        {
            return match;
        }

        if (snapshot.Moha != null)
        {
            foreach (var entry in snapshot.Moha.Descendants("entry"))
            {
                var listedName = entry.Elements("field")
                    .FirstOrDefault(field => NormalizeFieldName(field.Attribute("name")?.Value) == "(3)Name")
                    ?.Value
                    .Trim();
                if (!IsNameMatch(listedName, customerName))
                {
                    continue;
                }

                match.MOHAMatching = true;
                match.MatchingId = "MOHA:" + (entry.Attribute("id")?.Value ?? string.Empty);
                match.MatchingDescription = "MOHA REFERENCE:" + (entry.Elements("field")
                    .FirstOrDefault(field => NormalizeFieldName(field.Attribute("name")?.Value) == "(2)Reference")
                    ?.Value ?? string.Empty);
                break;
            }
        }

        AppendSanctionMatch(snapshot.Unscr, customerName, "UNSCR2231", "UNSCR 2231", match);
        if (AppendSanctionMatch(snapshot.Sanction1988, customerName, "UNSCR1988", "UNSCR 1988", match))
        {
            match.OtherMatching = true;
        }
        if (AppendSanctionMatch(snapshot.Isil, customerName, "UNSCR1267", "UNSCR 1267", match))
        {
            match.OtherMatching = true;
        }
        if (AppendSanctionMatch(snapshot.Sanction1718, customerName, "UNSCR1718", "UNSCR 1718", match))
        {
            match.OtherMatching = true;
        }

        return match;
    }

    private static bool AppendSanctionMatch(XDocument? document, string customerName, string idPrefix, string descriptionPrefix, SanctionMatch match)
    {
        if (document == null)
        {
            return false;
        }

        foreach (var individual in document.Descendants("INDIVIDUAL"))
        {
            var dataId = individual.Element("DATAID")?.Value.Trim() ?? string.Empty;
            var reference = individual.Element("REFERENCE_NUMBER")?.Value.Trim() ?? string.Empty;
            var fullName = string.Join(" ", new[]
            {
                individual.Element("FIRST_NAME")?.Value.Trim(),
                individual.Element("SECOND_NAME")?.Value.Trim(),
                individual.Element("THIRD_NAME")?.Value.Trim(),
            }.Where(value => !string.IsNullOrWhiteSpace(value)));

            if (IsNameMatch(fullName, customerName) || individual.Elements("INDIVIDUAL_ALIAS")
                .Elements("ALIAS_NAME")
                .Any(alias => IsNameMatch(alias.Value.Trim(), customerName)))
            {
                AppendMatch(match, idPrefix, descriptionPrefix, dataId, reference, false);
                return true;
            }
        }

        foreach (var entity in document.Descendants("ENTITY"))
        {
            var dataId = entity.Element("DATAID")?.Value.Trim() ?? string.Empty;
            var reference = entity.Element("REFERENCE_NUMBER")?.Value.Trim() ?? string.Empty;
            var entityName = entity.Element("FIRST_NAME")?.Value.Trim() ?? string.Empty;

            if (IsNameMatch(entityName, customerName) || entity.Elements("ENTITY_ALIAS")
                .Elements("ALIAS_NAME")
                .Any(alias => IsNameMatch(alias.Value.Trim(), customerName)))
            {
                AppendMatch(match, idPrefix, descriptionPrefix, dataId, reference, true);
                return true;
            }
        }

        return false;
    }

    private static void AppendMatch(SanctionMatch match, string idPrefix, string descriptionPrefix, string dataId, string reference, bool entity)
    {
        match.MatchingId += $" {idPrefix}:{dataId}";
        match.MatchingDescription += entity
            ? $" {descriptionPrefix} ENTITY REFERENCE:{reference}"
            : $" {descriptionPrefix} REFERENCE:{reference}";
        if (idPrefix == "UNSCR2231")
        {
            match.UNSCRMatching = true;
        }
    }

    private static string NormalizeName(string? value)
    {
        return string.Concat(value?.Where(char.IsLetterOrDigit) ?? Enumerable.Empty<char>()).ToUpperInvariant();
    }

    private static bool IsNameMatch(string? listedName, string customerName)
    {
        var normalizedListedName = NormalizeName(listedName);
        var normalizedCustomerName = NormalizeName(customerName);
        return normalizedListedName.Length > 0 && normalizedCustomerName.Length > 0 && normalizedListedName.Contains(normalizedCustomerName);
    }

    private static string NormalizeFieldName(string? value)
    {
        return string.Concat(value?.Where(character => !char.IsWhiteSpace(character)) ?? Enumerable.Empty<char>());
    }

    private sealed class SanctionSnapshot
    {
        public XDocument? Moha { get; init; }
        public XDocument? Unscr { get; init; }
        public XDocument? Sanction1988 { get; init; }
        public XDocument? Isil { get; init; }
        public XDocument? Sanction1718 { get; init; }
        public bool IsAvailable => Moha != null || Unscr != null || Sanction1988 != null || Isil != null || Sanction1718 != null;
    }

    private sealed class SanctionMatch
    {
        public bool MOHAMatching { get; set; }
        public bool UNSCRMatching { get; set; }
        public bool OtherMatching { get; set; }
        public string MatchingId { get; set; } = string.Empty;
        public string MatchingDescription { get; set; } = string.Empty;
    }
}
