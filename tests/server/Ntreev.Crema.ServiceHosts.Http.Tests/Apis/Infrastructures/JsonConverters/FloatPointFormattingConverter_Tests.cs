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

using System.Threading.Tasks;
using Ntreev.Crema.Tests;
using Ntreev.Crema.Tests.Extensions;
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
