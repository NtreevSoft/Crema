using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Data;
using Ntreev.CremaServer.Tests;
using Ntreev.CremaServer.Tests.Extensions;
using Ntreev.Library;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.Services.Tests.Data
{
    [Collection("Ntreev.Crema.Services.Tests")]
    public class TableDetailInfoTests
    {
        private readonly ITestOutputHelper output;
        private readonly CremaServerTestFixture fixture;

        public TableDetailInfoTests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("default", "tabledetailuser_01", "TableDetailInfo1")]
        public void TableDetailInfoTest(string databaseName, string userId, string tableName)
        {
            // given
            var (authentication, dataBase) = this.fixture.InitDataBaseAndUser(databaseName, userId);
            CreateTableTemplate(authentication, dataBase, tableName);
            var table = dataBase.Dispatcher.Invoke(() => dataBase.TableContext.Tables[tableName]);

            // when
            AddTableData(authentication, dataBase, tableName, "a1", "a1", TagInfo.All);
            Assert.Equal(1, table.Dispatcher.Invoke(() => table.TableDetailInfo.TableRowsCount));

            // then
            AddTableData(authentication, dataBase, tableName, "a2", "a2", TagInfoUtility.Server);
            Assert.Equal(2, table.Dispatcher.Invoke(() => table.TableDetailInfo.TableRowsCount));
            Assert.Equal(1, table.Dispatcher.Invoke(() => table.TableDetailInfo.TableServerTagRowsCount));
        }

        private void CreateTableTemplate(Authentication authentication, IDataBase dataBase, string tableName)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var template = dataBase.TableContext.Root.NewTable(authentication);
                template.SetTableName(authentication, tableName);
                template.CreateAddNew(authentication, "Column1", isKey: true);
                template.CreateAddNew(authentication, "Column2");
                template.EndEdit(authentication);
            });
        }

        private void AddTableData(Authentication authentication, IDataBase dataBase, string tableName, string column1, string column2, TagInfo tag, bool enabled = true)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                table.Dispatcher.Invoke(() =>
                {
                    table.Content.BeginEdit(authentication);
                    table.Content.EnterEdit(authentication);

                    var row1 = table.Content.AddNew(authentication, null);
                    row1.SetField(authentication, "Column1", column1);
                    row1.SetField(authentication, "Column2", column2);
                    row1.SetTags(authentication, tag);
                    row1.SetIsEnabled(authentication, enabled);
                    table.Content.EndNew(authentication, row1);

                    table.Content.LeaveEdit(authentication);
                    table.Content.EndEdit(authentication);
                });
            });
        }
    }
}
