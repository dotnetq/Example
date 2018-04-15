using QTools.Schema;

namespace Acl
{
    public class Principal
    {
        [Key]
        public string Id { get; set; }
        [Unique]
        public string Name { get; set; }
    }

    public class UserPrincipal
    {
        [ForeignKey(typeof(Auth.User))]
        public string User { get; set; }
        [ForeignKey(typeof(Principal))]
        public string Principal { get; set; }
    }

    public class Operation
    {
        [Key]
        public string Id { get; set; }
        [Unique]
        public string Name { get; set; }
    }

    public class Resource
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ResourceAcl
    {
        [ForeignKey(typeof(Resource))]
        public string Resource { get; set; }
        [ForeignKey(typeof(Principal))]
        public string Principal { get; set; }
        [ForeignKey(typeof(Operation))]
        public string Operation { get; set; }
    }

    public class GrantResourceAcl : ResourceAcl
    { }

    public class DenyResourceAcl : ResourceAcl
    { }
}
