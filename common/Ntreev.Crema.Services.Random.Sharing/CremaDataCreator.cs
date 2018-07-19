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

using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Text;
using Ntreev.Library;
using Ntreev.Crema.Data;
using System.Linq;

namespace Ntreev.Crema.Services.Random
{
    public static class CremaDataCreator
    {
        public static void CreateStandard(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                tableContext.Root.AddNewCategory(authentication, "All");
                tableContext.Root.AddNewCategory(authentication, "Client");
                tableContext.Root.AddNewCategory(authentication, "Server");
                tableContext.Root.AddNewCategory(authentication, "None");

                //CreateTable(authentication, allCategory, "table_all", TagInfo.All);
            });
        }

        private static void CreateTable(Authentication authentication, ITableCategory category, string name, TagInfo tags)
        {
            var template = category.NewTable(authentication);

            template.SetTableName(authentication, name);
            template.SetTags(authentication, tags);
            template.SetComment(authentication, $"table-{tags}");

            var key = template.AddNew(authentication);
            key.SetName(authentication, "key_column");
            key.SetIsKey(authentication, true);
            template.EndNew(authentication, key);

            var all = template.AddNew(authentication);
            all.SetName(authentication, "all_column");
            template.EndNew(authentication, all);

            var server = template.AddNew(authentication);
            server.SetName(authentication, "server_column");
            template.EndNew(authentication, server);

            var client = template.AddNew(authentication);
            client.SetName(authentication, "client_column");
            template.EndNew(authentication, client);

            var none = template.AddNew(authentication);
            none.SetName(authentication, "none_column");
            template.EndNew(authentication, none);

            template.EndEdit(authentication);

            if (template.Target is ITable[] tables)
            {
                var table = tables.First();

                CreateTable(authentication, table, "child_all", TagInfoUtility.All);
                CreateTable(authentication, table, "child_server", TagInfoUtility.Server);
                CreateTable(authentication, table, "child_client", TagInfoUtility.Client);
                CreateTable(authentication, table, "child_none", TagInfoUtility.Unused);
            }
        }

        private static void CreateTable(Authentication authentication, ITable table, string name, TagInfo tags)
        {
            var template = table.NewTable(authentication);

            template.SetTableName(authentication, name);
            template.SetTags(authentication, tags);
            template.SetComment(authentication, $"table-{tags}");

            var key = template.AddNew(authentication);
            key.SetName(authentication, "key_column");
            key.SetIsKey(authentication, true);
            template.EndNew(authentication, key);

            var all = template.AddNew(authentication);
            all.SetName(authentication, "all_column");
            template.EndNew(authentication, all);

            var server = template.AddNew(authentication);
            server.SetName(authentication, "server_column");
            template.EndNew(authentication, server);

            var client = template.AddNew(authentication);
            client.SetName(authentication, "client_column");
            template.EndNew(authentication, client);

            var none = template.AddNew(authentication);
            none.SetName(authentication, "none_column");
            template.EndNew(authentication, none);

            template.EndEdit(authentication);
        }
    }
}
