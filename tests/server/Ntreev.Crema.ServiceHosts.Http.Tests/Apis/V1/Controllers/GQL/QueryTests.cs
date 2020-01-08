using System.Net.Http;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Requests.GQL;
using Ntreev.Crema.Services;
using Ntreev.CremaServer.Tests;
using Ntreev.CremaServer.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.ServiceHosts.Http.Tests.Apis.V1.Controllers.GQL
{
    [Collection("Ntreev.Crema.ServiceHosts.Http.Tests")]
    public class QueryTests
    {
        private readonly ITestOutputHelper output;
        private readonly CremaServerTestFixture fixture;

        public QueryTests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("GQLDB", "gqluser_01", "GQLTable1")]
        public async Task QueryTest(string databaseName, string userId, string tableName)
        {
            // given
            var (authentication, dataBase) = this.fixture.InitDataBaseAndUser(databaseName, userId);
            CreateTestTemplate(tableName, dataBase, authentication);
            AddTableData(tableName, dataBase, authentication);

            // when
            var client = HttpClientTest.GetHttpClient(authentication);
            var response = await client.PostAsJsonAsync("api/v1/graphql", new GraphQLQueryRequest
            {
                Query = @"
query {
  Databases(name:""*"") {
  	ID
  	Name
  	IsEntered
  	Tags
  	Tables(name: ""GQLTable1"") {
  		Name
  		Tags
  		Columns {
  			Name
  			DefaultValue
  			Tags
  		}
  	}
  }
}
"});

            // then
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            this.output.WriteLine(json);
        }

        private void CreateTestTemplate(string tableName, IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var template = dataBase.TableContext.Root.NewTable(authentication);
                template.SetTableName(authentication, tableName);
                template.CreateAddNew(authentication, "KeyColumn1", isKey: true, dataType: "int");
                template.CreateAddNew(authentication, "StringColumn2", dataType: "string");
                template.EndEdit(authentication);

                Assert.Contains(dataBase.TableContext.Tables, table => table.Dispatcher.Invoke(() => table.TableName == tableName));
            });
        }

        private void AddTableData(string tableName, IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                table.Dispatcher.Invoke(() =>
                {
                    table.Content.BeginEdit(authentication);
                    table.Content.EnterEdit(authentication);

                    this.output.WriteLine($"DomainId: {table.Content.Domain.ID}");

                    for (var i = 0; i < 10; i++)
                    {
                        var row = table.Content.AddNew(authentication, i.ToString());
                        row.SetField(authentication, "KeyColumn1", i);
                        row.SetField(authentication, "StringColumn2", $"StringValue {i}");
                        table.Content.EndNew(authentication, row);
                    }

                    table.Content.LeaveEdit(authentication);
                    table.Content.EndEdit(authentication);
                });
            });
        }
    }
}
