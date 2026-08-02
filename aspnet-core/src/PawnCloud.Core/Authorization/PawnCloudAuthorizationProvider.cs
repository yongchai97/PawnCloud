using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace PawnCloud.Authorization;

public class PawnCloudAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
        context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
        context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
        context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

        var customers = context.CreatePermission(PermissionNames.Pages_Customers, L("Customers"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Create, L("CreateCustomer"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Edit, L("EditCustomer"));
        customers.CreateChildPermission(PermissionNames.Pages_Customers_Delete, L("DeleteCustomer"));

        var docs = context.CreatePermission(PermissionNames.Pages_CustomerDocuments, L("CustomerDocuments"));
        docs.CreateChildPermission(PermissionNames.Pages_CustomerDocuments_Create, L("CreateCustomerDocument"));
        docs.CreateChildPermission(PermissionNames.Pages_CustomerDocuments_Edit, L("EditCustomerDocument"));
        docs.CreateChildPermission(PermissionNames.Pages_CustomerDocuments_Delete, L("DeleteCustomerDocument"));

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

        context.CreatePermission(PermissionNames.Pages_MiscFunctions, L("MiscFunctions"));
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, PawnCloudConsts.LocalizationSourceName);
    }
}
