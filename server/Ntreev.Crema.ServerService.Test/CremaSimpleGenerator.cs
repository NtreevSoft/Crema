using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.Random;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ServerService.Test
{
    static class CremaSimpleGenerator
    {
        public static void Generate(this ICremaHost cremaHost, int tryCount)
        {
            var userContext = cremaHost.GetService<IUserContext>();
            var user = userContext.Users.Random(item => item.Authority == Authority.Admin);
            var authentication = cremaHost.Login(user.ID, "admin");

            CremaHostUtility.UserCreateTest(cremaHost, authentication, Authority.Member);
            CremaHostUtility.UserCreateTest(cremaHost, authentication, Authority.Guest);

            for (int i = 0; i < tryCount; i++)
            {
                CremaHostUtility.UserCreateTest(cremaHost, authentication);
            }

            var transaction = cremaHost.PrimaryDataBase.BeginTransaction(authentication);

            for (int i = 0; i < tryCount; i++)
            {
                CremaHostUtility.TypeCategoryCreateTest(cremaHost, authentication);
            }

            for (int i = 0; i < tryCount; i++)
            {
                CremaHostUtility.TypeCreateTest(cremaHost, authentication);
            }

            for (int i = 0; i < tryCount; i++)
            {
                CremaHostUtility.TableCategoryCreateTest(cremaHost, authentication);
            }

            for (int i = 0; i < tryCount; i++)
            {
                CremaHostUtility.TableCreateTest(cremaHost, authentication);
            }

            for (int i = 0; i < tryCount; i++)
            {
                CremaHostUtility.ChildTableCreateTest(cremaHost, authentication);
            }

            for (int i = 0; i < tryCount; i++)
            {
                CremaHostUtility.TableInheritTest(cremaHost, authentication);
            }

            for (int i = 0; i < tryCount; i++)
            {
                CremaHostUtility.ChildTableCreateTest(cremaHost, authentication);
            }

            transaction.Commit();

            
                //var userContext = cremaHost.GetService<IUserContext>();
                userContext.Logout(authentication, authentication.UserID);
            
        }

        //private static void GenerateUsers(this ICremaHost cremaHost, Authentication authentication)
        //{
        //    cremaHost.Dispatcher.Invoke(() =>
        //    {
        //        GenerateUser(cremaHost, authentication, Authority.Member);
        //        GenerateUser(cremaHost, authentication, Authority.Guest);
        //    });
        //}

        //private static void GenerateUser(this ICremaHost cremaHost, Authentication authentication, Authority authority)
        //{
        //    var userContext = cremaHost.GetService<IUserContext>();
        //    var category = userContext.Categories.Random();
        //    var identifier = RandomUtility.NextIdentifier();

        //    if (authority == Authority.Admin)
        //    {
        //        var newID = string.Format("Admin_{0}", identifier);
        //        var newName = string.Format("관리자_{0}", identifier);

        //        category.AddNewUser(authentication, newID, "admin", newName, Authority.Admin);
        //    }
        //    else if (authority == Authority.Member)
        //    {
        //        var newID = string.Format("Member_{0}", identifier);
        //        var newName = string.Format("구성원_{0}", identifier);

        //        category.AddNewUser(authentication, newID, "member", newName, Authority.Member);
        //    }
        //    else if (authority == Authority.Guest)
        //    {
        //        var newID = string.Format("Guest_{0}", identifier);
        //        var newName = string.Format("손님_{0}", identifier);

        //        category.AddNewUser(authentication, newID, "guest", newName, Authority.Guest);
        //    }
        //}
    }
}
