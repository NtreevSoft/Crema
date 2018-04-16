using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServerService.Test
{
    static class Extensions
    {
        public static void AddMember(this ITypeTemplate template, Authentication authentication, string name, long value, string comment)
        {
            var member = template.AddNew(authentication);
            member.SetName(authentication, name);
            member.SetValue(authentication, value);
            member.SetComment(authentication, comment);
            template.EndNew(authentication, member);
        }

        public static void AddKey(this ITableTemplate template, Authentication authentication, string name, string typeName)
        {
            var column = template.AddNew(authentication);
            column.SetName(authentication, name);
            column.SetIsKey(authentication, true);
            column.SetDataType(authentication, typeName);
            column.SetComment(authentication, string.Format("Key : {0}", typeName));
            template.EndNew(authentication, column);
        }

        public static void AddColumn(this ITableTemplate template, Authentication authentication, string name, string typeName)
        {
            var column = template.AddNew(authentication);
            column.SetName(authentication, name);
            column.SetDataType(authentication, typeName);
            template.EndNew(authentication, column);
        }

        public static ITypeCategory AddNewCategory(this ITypeCategory category, Authentication authentication)
        {
            var newName = NameUtility.GenerateNewName("Folder", category.Categories.Select(item => item.Name));
            return category.AddNewCategory(authentication, newName);
        }

        public static ITableCategory AddNewCategory(this ITableCategory category, Authentication authentication)
        {
            var newName = NameUtility.GenerateNewName("Folder", category.Categories.Select(item => item.Name));
            return category.AddNewCategory(authentication, newName);
        }

        public static Authentication LoginAdmin(this ICremaHost cremaHost)
        {
            return cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService<IUserContext>();
                var user = userContext.Users.Random(item => item.Authority == Authority.Admin);
                return cremaHost.Login(user.ID, user.Authority.ToString().ToLower());
            });
        }
    }
}
