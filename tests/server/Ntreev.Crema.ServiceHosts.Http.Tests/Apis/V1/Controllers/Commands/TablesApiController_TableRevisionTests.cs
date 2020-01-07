using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.Commands;
using Ntreev.Crema.Services;
using Ntreev.CremaServer.Tests;
using Ntreev.CremaServer.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.ServiceHosts.Http.Tests.Apis.V1.Controllers.Commands
{
    [Collection("Ntreev.Crema.ServiceHosts.Http.Tests")]
    public class TablesApiController_TableRevisionTests
    {
        private readonly ITestOutputHelper output;
        private readonly CremaServerTestFixture fixture;

        public TablesApiController_TableRevisionTests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("HttpDefault", "testuser_01", "Table1")]
        public async Task TableRevisionTest(string databaseName, string userId, string tableName)
        {
            // given
            var (authentication, database) = this.fixture.InitDataBaseAndUser(databaseName, userId);
            CreateTableTemplate(tableName, database, authentication);
            var client = HttpClientTest.GetHttpClient(authentication);
            var json = await client.GetStringAsync($"api/v1/commands/databases/{databaseName}/tables/{tableName}/info");
            var response = json.FromJson<GetTableInfoResponse>();
            var firstRevision = response.Revision;

            // when
            AddTableData(tableName, database, authentication);
            json = await client.GetStringAsync($"api/v1/commands/databases/{databaseName}/tables/{tableName}/info");
            response = json.FromJson<GetTableInfoResponse>();
            var secondRevision = response.Revision;

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
    }
}
