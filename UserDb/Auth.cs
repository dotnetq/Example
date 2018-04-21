using DotnetQ.QSchema.Attributes;

namespace Auth
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        [Unique]
        public string Login { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // We'll store the various roles when we deserialize
        [Ignore] public string[] PrincipalIds { get; set; }
    }

    public class LoginInfo
    {
        [Key]
        public string Id { get; set; }
        [Unique, ForeignKey(typeof(User))]
        public string User { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}