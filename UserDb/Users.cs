namespace UserDb
{
    public static class Users
    {
        public static readonly Auth.User Bart = new Auth.User { Id = "user::bart", Login = "bart", Name = "Bartholomew J. Simpson", Description = "Neighborhood troublemaker" };
        public static readonly Auth.User Homer = new Auth.User { Id = "user::homer", Login = "homer", Name = "Homer Jay Simpson", Description = "Everyday Joe..." };
        public static readonly Auth.User Marge = new Auth.User { Id = "user::marge", Login = "marge", Name = "Marjorie Jacqueline Simpson", Description = "Heroic home-maker" };
        public static readonly Auth.User MrBurns = new Auth.User { Id = "user::mrburns", Login = "mrburns", Name = "Charles Montgomery Burns", Description = "Ghoulish capitalist" };
        public static readonly Auth.User Smithers = new Auth.User { Id = "user::smithers", Login = "smithers", Name = "Waylon Smithers, Jr.", Description = "Personal Assistant" };
    }
}
