namespace UserDb
{
    public static class Operations
    {
        public static readonly Acl.Operation Enter = new Acl.Operation { Id = "operation::enter", Name = "Enter" };
        public static readonly Acl.Operation Leave = new Acl.Operation { Id = "operation::leave", Name = "Leave" };
    }
}
