using Parakeet.Net.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 供应商权限
    /// </summary>
    public class SupplierPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(SupplierPermissions.GroupName,L("Suppliers"));

            myGroup.AddPermission(SupplierPermissions.Supplier.Default, L("Suppliers.Default"));
            myGroup.AddPermission(SupplierPermissions.Supplier.Create, L("Suppliers.Create"));
            myGroup.AddPermission(SupplierPermissions.Supplier.Update, L("Suppliers.Update"));
            myGroup.AddPermission(SupplierPermissions.Supplier.Delete, L("Suppliers.Delete"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<NetResource>(name);
        }
    }
}