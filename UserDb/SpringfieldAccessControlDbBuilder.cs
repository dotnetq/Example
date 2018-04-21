using DotnetQ.Qapper;
using DotnetQ.QSchema;
using qSharp;
using System;
using System.IO;

namespace UserDb
{
    public class SpringfieldAccessControlDbBuilder
    {
        public void BuildDb()
        {
            using (var connection = new QBasicConnection(port: 5010))
            {
                connection.Open();

                DefineSchema(connection);

                AddUsers(connection);
                AddPrincipals(connection);
                AddOperations(connection);
                AddResources(connection);
                AddUserPrincipals(connection);
                AddGrantAcl(connection);
                AddDenyAcl(connection);
            }
        }

        private void DefineSchema(QBasicConnection connection)
        {
            // input types can be added in any order. They are sorted by dependency later
            var types = new[]
            {
                typeof(Auth.User),
                typeof(Auth.LoginInfo),
                typeof(Acl.Principal),
                typeof(Acl.UserPrincipal),
                typeof(Acl.Operation),
                typeof(Acl.Resource),
                typeof(Acl.GrantResourceAcl),
                typeof(Acl.DenyResourceAcl),
            };
            DefineSchema(types, connection);
        }

        private void DefineSchema(Type[] types, QBasicConnection connection)
        {
            var fullAclSchema = SchemaBuilder.DeclareEmptySchema(types);
            using (var sr = new StringReader(fullAclSchema))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    connection.Sync(line);
                }
            }
        }

        private void AddEntities<T>(T[] entities, QBasicConnection connection)
        {
            var qTable = QMapper.ConvertToQTable<T>(entities);
            var tableName = SchemaBuilder.GetQTableName(typeof(T));
            connection.Sync("upsert", tableName, qTable);
        }

        #region Add Entities

        private void AddUsers(QBasicConnection connection)
        {
            var users = new[] { Users.Bart, Users.Homer, Users.Marge, Users.MrBurns, Users.Smithers, };
            AddEntities(users, connection);
            AddIdentityPrincipals(users, connection);
        }

        private void AddIdentityPrincipals(Auth.User[] users, QBasicConnection connection)
        {
            var identityPrincipals = new Acl.Principal[users.Length];
            var identityUserPrincipals = new Acl.UserPrincipal[users.Length];
            for (int i = 0; i < users.Length; ++i)
            {
                var login = users[i].Login;
                identityPrincipals[i] = new Acl.Principal { Id = "principal::" + login, Name = login };
                identityUserPrincipals[i] = new Acl.UserPrincipal { User = "user::" + login, Principal = "principal::" + login };
            }
            AddEntities(identityPrincipals, connection);
            AddEntities(identityUserPrincipals, connection);
        }

        private void AddPrincipals(QBasicConnection connection)
        {
            var principals = new[] { Principals.Everyone, Principals.Employee, Principals.Supervisor, Principals.Owner, Principals.Simpson };
            AddEntities(principals, connection);
        }

        private void AddOperations(QBasicConnection connection)
        {
            var operations = new[] { Operations.Enter, Operations.Leave };
            AddEntities(operations, connection);
        }

        private void AddResources(QBasicConnection connection)
        {
            var resources = new[] { Resources.SimpsonHome, Resources.Sector7G, Resources.HeadOffice };
            AddEntities(resources, connection);
        }

        private void AddUserPrincipals(QBasicConnection connection)
        {
            var userPrincipals = new[]
            {
                new Acl.UserPrincipal { User = Users.Bart.Id, Principal = Principals.Everyone.Id},
                new Acl.UserPrincipal { User = Users.Bart.Id, Principal = Principals.Simpson.Id},
                new Acl.UserPrincipal { User = Users.Marge.Id, Principal = Principals.Everyone.Id},
                new Acl.UserPrincipal { User = Users.Marge.Id, Principal = Principals.Simpson.Id},
                new Acl.UserPrincipal { User = Users.Homer.Id, Principal = Principals.Everyone.Id},
                new Acl.UserPrincipal { User = Users.Homer.Id, Principal = Principals.Simpson.Id},
                new Acl.UserPrincipal { User = Users.Homer.Id, Principal = Principals.Employee.Id},
                new Acl.UserPrincipal { User =  Users.Smithers.Id, Principal = Principals.Everyone.Id},
                new Acl.UserPrincipal { User =  Users.Smithers.Id, Principal = Principals.Employee.Id},
                new Acl.UserPrincipal { User =  Users.Smithers.Id, Principal = Principals.Supervisor.Id},
                new Acl.UserPrincipal { User = Users.MrBurns.Id, Principal = Principals.Everyone.Id},
                new Acl.UserPrincipal { User = Users.MrBurns.Id, Principal = Principals.Employee.Id},
                new Acl.UserPrincipal { User = Users.MrBurns.Id, Principal = Principals.Supervisor.Id},
                new Acl.UserPrincipal { User = Users.MrBurns.Id, Principal = Principals.Owner.Id},
            };
            AddEntities(userPrincipals, connection);
        }

        private void AddGrantAcl(QBasicConnection connection)
        {
            var grantAcl = new[]
            {
                // All Simpson family members can enter and leave the family home
                new Acl.GrantResourceAcl{ Resource = Resources.SimpsonHome.Id, Operation = Operations.Enter.Id, Principal = Principals.Simpson.Id},
                new Acl.GrantResourceAcl{ Resource = Resources.SimpsonHome.Id, Operation = Operations.Leave.Id, Principal = Principals.Simpson.Id},
                // We'll give Smithers specific access to enter and leave the family home
                new Acl.GrantResourceAcl{ Resource = Resources.SimpsonHome.Id, Operation = Operations.Enter.Id, Principal = "principal::smithers"},
                new Acl.GrantResourceAcl{ Resource = Resources.SimpsonHome.Id, Operation = Operations.Leave.Id, Principal = "principal::smithers"},
                // All employees can enter and leave the Homer's office
                new Acl.GrantResourceAcl{ Resource = Resources.Sector7G.Id, Operation = Operations.Enter.Id, Principal = Principals.Employee.Id},
                new Acl.GrantResourceAcl{ Resource = Resources.Sector7G.Id, Operation = Operations.Leave.Id, Principal = Principals.Employee.Id},
                // Supervisors can enter and leave the head office
                new Acl.GrantResourceAcl{ Resource = Resources.HeadOffice.Id, Operation = Operations.Enter.Id, Principal = Principals.Supervisor.Id},
                new Acl.GrantResourceAcl{ Resource = Resources.HeadOffice.Id, Operation = Operations.Leave.Id, Principal = Principals.Supervisor.Id},
            };
            AddEntities(grantAcl, connection);
        }

        private void AddDenyAcl(QBasicConnection connection)
        {
            // Owners are explicitly prohibited from leaving the office
            var denyAcl = new[]
            {
                new Acl.DenyResourceAcl{ Resource = Resources.HeadOffice.Id, Operation = Operations.Leave.Id, Principal = Principals.Owner.Id},
            };
            AddEntities(denyAcl, connection);
        }

        #endregion
    }
}