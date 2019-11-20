//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Extensions;

namespace Ntreev.CremaServer.Tests.Extensions
{
    public static class CremaServerTestFixtureExtensions
    {
        public static IDataBase InitDataBase(this CremaServerTestFixture fixture, string databaseName)
        {
            var cremaHost = fixture.CremaHost;

            return cremaHost.Dispatcher.Invoke(() =>
            {
                var database = GetOrCreateDataBase(cremaHost, databaseName);
                return database;
            });
        }

        public static (Authentication, IDataBase) InitDataBaseAndUser(this CremaServerTestFixture fixture, string databaseName, string userId)
        {
            return InitDataBaseAndUser(fixture, databaseName, userId, userId);
        }

        public static (Authentication, IDataBase) InitDataBaseAndUser(this CremaServerTestFixture fixture, string databaseName, string userId, string password)
        {
            var cremaHost = fixture.CremaHost;

            var database = InitDataBase(fixture, databaseName);

            return cremaHost.Dispatcher.Invoke(() =>
            {
                var user = GetOrCreateUser(cremaHost, userId, password);
                var authentication = cremaHost.Login(userId, password.ToSecureString());

                database.Enter(authentication);

                return (authentication, database);
            });
        }

        public static IDataBase GetOrCreateDataBase(this ICremaHost cremaHost, string databaseName)
        {
            if (cremaHost.DataBases.Contains(databaseName))
            {
                var database = cremaHost.DataBases[databaseName];
                if (!database.IsLoaded)
                {
                    database.Load(Authentication.System);
                }

                return database;
            }

            var adminAuthentication = cremaHost.GetAdminAuthentication();

            try
            {
                var database = cremaHost.DataBases.AddNewDataBase(adminAuthentication, databaseName, "create database");
                if (!database.IsLoaded)
                {
                    database.Load(Authentication.System);
                }

                return database;
            }
            finally
            {
                cremaHost.Logout(adminAuthentication);
            }
        }

        public static IUser GetOrCreateUser(this ICremaHost cremaHost, string userId, string password)
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

            var adminAuthentication = cremaHost.GetAdminAuthentication();

            try
            {
                return userContext.Dispatcher.Invoke(() => userContext.Categories.Root.AddNewUser(adminAuthentication, userId, password.ToSecureString(), userId, Authority.Member));
            }
            finally
            {
                cremaHost.Logout(adminAuthentication);
            }
        }
    }
}
