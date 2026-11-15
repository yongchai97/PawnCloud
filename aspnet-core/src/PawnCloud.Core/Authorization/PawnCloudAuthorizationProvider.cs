using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace PawnCloud.Authorization;

public class PawnCloudAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        var users = context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
        users.CreateChildPermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
        context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
        context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

        var customers = context.CreatePermission(PermissionNames.Pages_Customers, L("Customers"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Create, L("CreateCustomer"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Edit, L("EditCustomer"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Delete, L("DeleteCustomer"));

        var pawnTickets = context.CreatePermission(PermissionNames.Pages_PawnTickets, L("PawnTickets"));
        pawnTickets.CreateChildPermission(PermissionNames.Pages_PawnTickets_Create, L("CreatePawnTicket"));
        pawnTickets.CreateChildPermission(PermissionNames.Pages_PawnTickets_Edit, L("EditPawnTicket"));
        pawnTickets.CreateChildPermission(PermissionNames.Pages_PawnTickets_Delete, L("DeletePawnTicket"));

        var pawnItems = context.CreatePermission(PermissionNames.Pages_PawnItems, L("PawnItems"));
        pawnItems.CreateChildPermission(PermissionNames.Pages_PawnItems_Create, L("CreatePawnItem"));
        pawnItems.CreateChildPermission(PermissionNames.Pages_PawnItems_Edit, L("EditPawnItem"));
        pawnItems.CreateChildPermission(PermissionNames.Pages_PawnItems_Delete, L("DeletePawnItem"));

        var loans = context.CreatePermission(PermissionNames.Pages_Loans, L("Loans"));
        loans.CreateChildPermission(PermissionNames.Pages_Loans_Create, L("CreateLoan"));
        loans.CreateChildPermission(PermissionNames.Pages_Loans_Edit, L("EditLoan"));
        loans.CreateChildPermission(PermissionNames.Pages_Loans_Delete, L("DeleteLoan"));

        var basicCodes = context.CreatePermission(PermissionNames.Pages_BasicCodes, L("BasicCodes"));
        basicCodes.CreateChildPermission(PermissionNames.Pages_BasicCodes_Create, L("CreateBasicCode"));
        basicCodes.CreateChildPermission(PermissionNames.Pages_BasicCodes_Edit, L("EditBasicCode"));
        basicCodes.CreateChildPermission(PermissionNames.Pages_BasicCodes_Delete, L("DeleteBasicCode"));

        context.CreatePermission(PermissionNames.Pages_MiscFunctions, L("MiscFunctions"));
        context.CreatePermission(PermissionNames.Pages_AuditLogs, L("AuditLogs"), multiTenancySides: MultiTenancySides.Host | MultiTenancySides.Tenant);

        var globalBlackList = context.CreatePermission(PermissionNames.Pages_GlobalBlackLists, L("GlobalBlackLists"), multiTenancySides: MultiTenancySides.Host);
        globalBlackList.CreateChildPermission(PermissionNames.Pages_GlobalBlackLists_Create, L("CreateGlobalBlackLists"), multiTenancySides: MultiTenancySides.Host);
        globalBlackList.CreateChildPermission(PermissionNames.Pages_GlobalBlackLists_Delete, L("DeleteGlobalBlackLists"), multiTenancySides: MultiTenancySides.Host);

        var globalBlackListRequests = context.CreatePermission(PermissionNames.Pages_GlobalBlackListRequests, L("GlobalBlackListRequests"));
        globalBlackListRequests.CreateChildPermission(PermissionNames.Pages_GlobalBlackListRequests_Create, L("CreateGlobalBlackListRequests"));
        globalBlackListRequests.CreateChildPermission(PermissionNames.Pages_GlobalBlackListRequests_Edit, L("EditGlobalBlackListRequests"));
        globalBlackListRequests.CreateChildPermission(PermissionNames.Pages_GlobalBlackListRequests_Delete, L("DeleteGlobalBlackListRequests"));

        var privateBlackLists = context.CreatePermission(PermissionNames.Pages_PrivateBlackLists, L("PrivateBlackLists"));
        privateBlackLists.CreateChildPermission(PermissionNames.Pages_PrivateBlackLists_Create, L("CreatePrivateBlackLists"));
        privateBlackLists.CreateChildPermission(PermissionNames.Pages_PrivateBlackLists_Edit, L("EditPrivateBlackLists"));
        privateBlackLists.CreateChildPermission(PermissionNames.Pages_PrivateBlackLists_Delete, L("DeletePrivateBlackLists"));

    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, PawnCloudConsts.LocalizationSourceName);
    }
}
