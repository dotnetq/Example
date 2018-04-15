namespace UserDb
{
    public static class Principals
    {
        public static readonly Acl.Principal Everyone = new Acl.Principal { Id = "principal::everyone", Name = "Everyone" };
        public static readonly Acl.Principal Employee = new Acl.Principal { Id = "principal::employee", Name = "Employee" };
        public static readonly Acl.Principal Supervisor = new Acl.Principal { Id = "principal::supervisor", Name = "Supervisor" };
        public static readonly Acl.Principal Owner = new Acl.Principal { Id = "principal::owner", Name = "Owner" };
        public static readonly Acl.Principal Simpson = new Acl.Principal { Id = "principal::simpson", Name = "Simpson" };
    }
}
