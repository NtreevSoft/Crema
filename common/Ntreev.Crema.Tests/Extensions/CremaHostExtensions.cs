using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;
using Ntreev.Library;

namespace Ntreev.Crema.Tests.Extensions
{
    public static class CremaHostExtensions
    {
        public static (Authentication, IDataBase) LoginAndEnter(this ICremaHost cremaHost, string databaseName, string userID, string password)
        {
            var authentication = cremaHost.Login(userID, password.ToSecureString());
            return cremaHost.Dispatcher.Invoke(() =>
            {
                var database = cremaHost.DataBases[databaseName];
                return database.Dispatcher.Invoke(() =>
                {
                    if (!database.IsLoaded)
                    {
                        database.Load(authentication);
                    }

                    database.Enter(authentication);

                    return (authentication, database);
                });
            });
        }
    }
}
