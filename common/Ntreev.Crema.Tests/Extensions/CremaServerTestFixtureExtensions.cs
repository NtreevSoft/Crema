using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Extensions;

namespace Ntreev.Crema.Tests.Extensions
{
    public static class CremaServerTestFixtureExtensions
    {
        public static void Login(string databaseName, string userId, string password)
        {
        }

        public static (Authentication, IDataBase) InitDataBaseAndUser(this CremaServerTestFixture fixture, string databaseName, string userId, string password)
        {
            var cremaHost = fixture.CremaHost;

            return cremaHost.Dispatcher.Invoke(() =>
            {
                var database = GetOrCreateDataBase(cremaHost, databaseName);
                var user = GetOrCreateUser(cremaHost, userId, password);
                var authentication = cremaHost.Login(userId, password.ToSecureString());

                database.Enter(authentication);

                return (authentication, database);
            });
        }

        public static IDataBase GetOrCreateDataBase(ICremaHost cremaHost, string databaseName)
        {
            if (cremaHost.DataBases.Contains(databaseName))
            {
                return cremaHost.DataBases[databaseName];
            }

            var authentication = GetAdminAuthentication(cremaHost);
            try
            {
                var database = cremaHost.DataBases.AddNewDataBase(authentication, databaseName, "create database");
                if (!database.IsLoaded)
                {
                    database.Load(authentication);
                }

                return database;
            }
            finally
            {
                cremaHost.Logout(authentication);
            }
        }

        private static IUser GetOrCreateUser(ICremaHost cremaHost, string userId, string password)
        {
            var userContext = cremaHost.GetService<IUserContext>();
            var user = userContext.Dispatcher.Invoke(() =>
            {
                if (userContext.Users.Contains(userId))
                {
                    return userContext.Users[userId];
                }

                return null;
            });

            if (user != null) return user;

            var authentication = GetAdminAuthentication(cremaHost);

            try
            {

                return userContext.Dispatcher.Invoke(() =>
                {
                    return userContext.Categories.Root.AddNewUser(authentication, userId, password.ToSecureString(), userId, Authority.Member, true);
                });
            }
            finally
            {
                cremaHost.Logout(authentication);
            }
        }

        private static Authentication GetAdminAuthentication(ICremaHost cremaHost)
        {
            return cremaHost.Login("admin", "admin".ToSecureString());
        }
    }
}
