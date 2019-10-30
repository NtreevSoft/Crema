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
