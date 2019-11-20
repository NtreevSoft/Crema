using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using Ntreev.CremaServer.Tests;
using Ntreev.CremaServer.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Ntreev.Crema.Services.Tests
{
    [Collection("Ntreev.Crema.Services.Tests")]
    public class TableUpdateRevisionTests
    {
        private readonly ITestOutputHelper output;
        private readonly CremaServerTestFixture fixture;

        public TableUpdateRevisionTests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;

            this.output.WriteLine($"RepositoryPath: {fixture.CremaHost.RepositoryPath}");
        }

        [Theory]
        [InlineData("default", "testuser_01", "Table1")]
        public void UpdateRevisionTest(string databaseName, string userId, string tableName)
        {
            // given
            var (authentication, database) = this.fixture.InitDataBaseAndUser(databaseName, userId);
            var firstRevision = CreateTableTemplate(tableName, database, authentication);

            // when
            var secondRevision = AddTableData(tableName, database, authentication);
            var thirdRevision = ModifyTableTemplate(tableName, database, authentication);

            // then
            Assert.True(firstRevision+1 == secondRevision);
            Assert.True(secondRevision == thirdRevision);
        }

        [Theory]
        [InlineData("default", "testuser_01", "Table2", "Table2_Copied")]
        public void CopyAndUpdateRevisionTest(string databaseName, string userId, string tableName, string newTableName)
        {
            // given
            var (authentication, dataBase) = this.fixture.InitDataBaseAndUser(databaseName, userId);
            var firstRevision = CreateTableTemplate(tableName, dataBase, authentication);

            // when
            var secondRevision = dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                table.Dispatcher.Invoke(() =>
                {
                    table.Copy(authentication, newTableName, "/", true); 
                });
                var copiedTable = dataBase.TableContext.Tables[newTableName];
                return copiedTable.Dispatcher.Invoke(() => copiedTable.Revision);
            });

            this.output.WriteLine(firstRevision.ToString());
            this.output.WriteLine(secondRevision.ToString());

            // then
            Assert.True(firstRevision+1 == secondRevision);
        }

        private long CreateTableTemplate(string tableName, IDataBase database, Authentication authentication)
        {
            return database.Dispatcher.Invoke(() =>
            {
                var template = database.TableContext.Root.NewTable(authentication);
                template.SetTableName(authentication, tableName);
                template.CreateAddNew(authentication, "Column1", isKey: true);
                template.EndEdit(authentication);

                Assert.Contains(database.TableContext.Tables, t => t.Dispatcher.Invoke(() => t.TableName == tableName));

                var table = database.TableContext.Tables[tableName];
                return table.Dispatcher.Invoke(() =>
                {
                    this.output.WriteLine($"table.GetLog: {table.GetLog(authentication).First().ToJson()}");
                    this.output.WriteLine($"table.Revision: {table.TableName} revision: {table.Revision}");
                    return table.Revision;
                });
            });
        }

        private long AddTableData(string tableName, IDataBase database, Authentication authentication)
        {
            return database.Dispatcher.Invoke(() =>
            {
                var table = database.TableContext.Tables[tableName];
                return table.Dispatcher.Invoke(() =>
                {
                    table.Content.BeginEdit(authentication);
                    table.Content.EnterEdit(authentication);

                    this.output.WriteLine($"DomainId: {table.Content.Domain.ID}");

                    var row = table.Content.AddNew(authentication, "a");
                    row.SetField(authentication, "Column1", "hello");
                    table.Content.EndNew(authentication, row);

                    table.Content.LeaveEdit(authentication);
                    table.Content.EndEdit(authentication);

                    this.output.WriteLine($"table.GetLog: {table.GetLog(authentication).First().ToJson()}");
                    this.output.WriteLine($"table.Revision: {table.TableName} revision: {table.Revision}");
                    return table.Revision;
                });
            });
        }

        private long ModifyTableTemplate(string tableName, IDataBase database, Authentication authentication)
        {
            return database.Dispatcher.Invoke(() =>
            {
                var table = database.TableContext.Tables[tableName];
                return table.Dispatcher.Invoke(() =>
                {
                    table.Template.BeginEdit(authentication);
                    table.Template.CreateAddNew(authentication, "Column2", allowNull: true);
                    table.Template.EndEdit(authentication);

                    this.output.WriteLine($"table.GetLog: {table.GetLog(authentication).First().ToJson()}");
                    this.output.WriteLine($"table.Revision: {table.TableName} revision: {table.Revision}");
                    return table.Revision;
                });
            });
        }
    }
}
