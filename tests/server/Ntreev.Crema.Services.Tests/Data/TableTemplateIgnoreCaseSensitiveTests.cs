using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.CremaServer.Tests;
using Ntreev.CremaServer.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.Services.Tests.Data
{
    [Collection("Ntreev.Crema.Services.Tests")]
    public class TableTemplateIgnoreCaseSensitiveTests
    {
        private readonly ITestOutputHelper output;
        private readonly CremaServerTestFixture fixture;

        public TableTemplateIgnoreCaseSensitiveTests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("default", "ignoreuser_01", "NoIgnoreTable1")]
        public void IgnoreCaseSensitiveIsFalseTest(string databaseName, string userId, string tableName)
        {
            // given
            var (authentication, dataBase) = this.fixture.InitDataBaseAndUser(databaseName, userId);

            // when
            CreateTableTemplate(dataBase, tableName, authentication, false);
            CreateTableData(dataBase, tableName, authentication);

            // then
            dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                var dataSet = table.GetDataSet(authentication, -1);
                Assert.Equal(2, dataSet.Tables[0].Rows.Count);
            });
        }

        [Theory]
        [InlineData("default", "ignoreuser_01", "IgnoreTable1")]
        public void IgnoreCaseSensitiveIsTrueTest(string databaseName, string userId, string tableName)
        {
            // given
            var (authentication, dataBase) = this.fixture.InitDataBaseAndUser(databaseName, userId);

            // when
            CreateTableTemplate(dataBase, tableName, authentication, true);

            // then
            Assert.Throws<ConstraintException>(() => CreateTableData(dataBase, tableName, authentication));
        }

        private void CreateTableTemplate(IDataBase dataBase, string tableName, Authentication authentication, bool ignoreCaseSensitive)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var template = dataBase.TableContext.Root.NewTable(authentication);
                template.SetTableName(authentication, tableName);
                template.SetIgnoreCaseSensitive(authentication, ignoreCaseSensitive);
                template.CreateAddNew(authentication, "Column1", isKey: true);
                template.CreateAddNew(authentication, "Column2");
                template.EndEdit(authentication);
            });
        }

        private void CreateTableData(IDataBase dataBase, string tableName, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                table.Dispatcher.Invoke(() =>
                {
                    table.Content.BeginEdit(authentication);
                    table.Content.EnterEdit(authentication);

                    var row1 = table.Content.AddNew(authentication, "a");
                    row1.SetField(authentication, "Column1", "crema");
                    row1.SetField(authentication, "Column2", "crema data");
                    table.Content.EndNew(authentication, row1);

                    var row2 = table.Content.AddNew(authentication, "b");
                    row2.SetField(authentication, "Column1", "CREMA");
                    row2.SetField(authentication, "Column2", "CREMA DATA");
                    table.Content.EndNew(authentication, row2);

                    table.Content.LeaveEdit(authentication);
                    table.Content.EndEdit(authentication);
                });
            });
        }
    }
}
