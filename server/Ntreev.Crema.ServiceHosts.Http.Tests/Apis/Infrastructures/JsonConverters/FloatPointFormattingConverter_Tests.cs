using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ntreev.Crema.Services;
using Ntreev.Crema.Tests;
using Ntreev.Crema.Tests.Extensions;
using Ntreev.Library;
using Ntreev.Library.IO;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.ServiceHosts.Http.Tests.Apis.Infrastructures.JsonConverters
{
    public class FloatPointFormattingConverter_Tests : CremaServerTest, IClassFixture<CremaServerTestFixture>
    {
        private readonly CremaServerTestFixture fixture;
        private readonly ITestOutputHelper output;

        public FloatPointFormattingConverter_Tests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }


        [Theory]
        [InlineData("FloatPointFormatting_DB", "float_point_formatting_test_table", "floattestuser01")]
        public async Task FloatPointFormattingConvert_Test(string databaseName, string tableName, string userId)
        {
            var (authentication, database) = this.fixture.InitDataBaseAndUser(databaseName, userId, userId);

            try
            {
                // given
                await database.Dispatcher.InvokeAsync(() =>
                {
                    var template = database.TableContext.Root.NewTable(authentication);
                    template.Dispatcher.Invoke(() =>
                    {
                        template.SetTableName(authentication, tableName);

                        var column1 = template.AddNew(authentication);
                        column1.SetName(authentication, "Column1");
                        column1.SetIsKey(authentication, true);
                        column1.SetDefaultValue(authentication, "1.0");
                        column1.SetDataType(authentication, "float");
                        template.EndNew(authentication, column1);
                        template.EndEdit(authentication);
                    });

                    Assert.Contains(database.TableContext.Tables, table => table.Dispatcher.Invoke(() => table.TableName == tableName));
                });

                // when
                var http = GetHttpClient(authentication);
                var message = await http.GetAsync($"/api/v1/commands/databases/{databaseName}/tables/{tableName}/info");
                message.EnsureSuccessStatusCode();

                var json = await message.Content.ReadAsStringAsync();
                output.WriteLine(json);

                // then
                Assert.Contains("\"DefaultValue\":1", json);
                Assert.DoesNotContain("\"DefaultValue\":1.0", json);
            }
            finally
            {
                this.fixture.CremaHost.Logout(authentication);
            }
        }
    }
}
