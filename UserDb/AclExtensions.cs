using System.Collections.Generic;
using System.Linq;

namespace UserDb
{
    /// <summary>
    /// This is a fluent interface to progressively 'unpeel' our access control data structure.
    /// </summary>
    public static class AclExtensions
    {
        public static ILookup<string, string> OnResource(this Dictionary<string, ILookup<string, string>> acl, Acl.Resource resource)
        {
            ILookup<string, string> innerAcl;
            if (acl.TryGetValue(resource.Id, out innerAcl))
            {
                return innerAcl;
            }

            return null;
        }

        public static IEnumerable<string> PermissionTo(this ILookup<string, string> permissions, Acl.Operation operation)
        {
            if (permissions == null) return null;

            return permissions[operation.Id];
        }

        public static bool ExistsFor(this IEnumerable<string> principalIds, Auth.User user)
        {
            if (principalIds == null) return false;

            return principalIds.Any(
                aclPrincipal => user.PrincipalIds.Any(
                    userPrincipal => string.Equals(aclPrincipal, userPrincipal)));
        }
    }
}
