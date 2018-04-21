using DotnetQ.Qapper;
using qSharp;
using System;
using System.Linq;

namespace UserDb
{
    class Program
    {
        void Run()
        {
            var builder = new SpringfieldAccessControlDbBuilder();
            builder.BuildDb();

            ReadSpringfieldAccessControlDb();
        }

        private void ReadSpringfieldAccessControlDb()
        {
            using (var connection = new QBasicConnection(port: 5010))
            {
                connection.Open();

                var userPrincipals = connection.QueryObjects<Acl.UserPrincipal>("select from .acl.userPrincipal")
                    .ToLookup(u => u.User);

                // Pull out the users and initialise the objects with all relevant principal Ids
                var users = connection.QueryObjects<Auth.User>("select from .auth.user")
                    .Select(u => 
                    {
                        u.PrincipalIds = userPrincipals[u.Id].Select(p => p.Principal).ToArray();
                        return u;
                    })
                    .ToDictionary(u => u.Id);

                // pull out the grant acl and restructure the data for rapid in-proc queries
                var grantResourceAcl = connection.QueryObjects<Acl.GrantResourceAcl>("select from .acl.grantResourceAcl")
                    .GroupBy(row => row.Resource)
                    .ToDictionary(group => group.Key,
                                  group => group.ToLookup(entry => entry.Operation,
                                                          entry => entry.Principal));

                var denyResourceAcl = connection.QueryObjects<Acl.DenyResourceAcl>("select from .acl.denyResourceAcl")
                    .GroupBy(row => row.Resource)
                    .ToDictionary(group => group.Key,
                                  group => group.ToLookup(entry => entry.Operation,
                                                          entry => entry.Principal));

                // Now we can test our ACL

                Auth.User dbUser = users[Users.Bart.Id];

                var denyOutcome = denyResourceAcl.OnResource(Resources.SimpsonHome).PermissionTo(Operations.Enter).ExistsFor(dbUser);
                var grantOutcome = grantResourceAcl.OnResource(Resources.SimpsonHome).PermissionTo(Operations.Enter).ExistsFor(dbUser);

                Console.WriteLine($"{dbUser.Name} is {IsPermitted(denyOutcome, grantOutcome)} to Enter the Simpson Home");

                // We gave Smithers specific access to enter the Simpsons Home
                dbUser = users[Users.Smithers.Id];

                denyOutcome = denyResourceAcl.OnResource(Resources.SimpsonHome).PermissionTo(Operations.Leave).ExistsFor(dbUser);
                grantOutcome = grantResourceAcl.OnResource(Resources.SimpsonHome).PermissionTo(Operations.Leave).ExistsFor(dbUser);

                Console.WriteLine($"{dbUser.Name} is {IsPermitted(denyOutcome, grantOutcome)} specifically to Enter the Simpson Home");

                dbUser = users[Users.MrBurns.Id];

                denyOutcome = denyResourceAcl.OnResource(Resources.HeadOffice).PermissionTo(Operations.Leave).ExistsFor(dbUser);
                grantOutcome = grantResourceAcl.OnResource(Resources.HeadOffice).PermissionTo(Operations.Leave).ExistsFor(dbUser);

                Console.WriteLine($"{dbUser.Name} is {IsPermitted(denyOutcome, grantOutcome)} to Leave the office");

                Console.ReadKey();
            }
        }

        public string IsPermitted(bool denyOutcome, bool grantOutcome)
        {
            return !denyOutcome && grantOutcome ? "permitted" : "not permitted";
        }

        static void Main(string[] args)
        {
            var theApp = new Program();
            theApp.Run();
        }
    }
}