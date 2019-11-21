using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ntreev.Crema.Services.Data;
using Ntreev.CremaServer.Tests;
using Ntreev.CremaServer.Tests.Extensions;
using Ntreev.Library;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.SvnModule.Shell.Tests
{
    [Collection("Ntreev.Crema.SvnModule.Shell.Tests")]
    public class SvnRevisionInfoEventArgsTests
    {
        private readonly ITestOutputHelper output;
        private readonly CremaServerTestFixture fixture;

        public SvnRevisionInfoEventArgsTests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Fact]
        public void RunTest()
        {
            // given
            var path = this.fixture.CremaHost.RepositoryPath;
            output.WriteLine(path);

            var infos = SvnRevisionInfoEventArgs.Run(path);
            output.WriteLine(infos.ToJson());

            Assert.NotEmpty(infos);
            Assert.Contains(infos, args => args.Name == "users.xml");
        }

        [Theory]
        [InlineData("testdb01", "testuser_01")]
        public void TableRevisionTest(string databaseName, string userName)
        {
            // given
            var tableName = "Table1";
            var (authentication, database) = this.fixture.InitDataBaseAndUser(databaseName, userName, userName);
            var path = this.fixture.CremaHost.RepositoryPath;
            this.output.WriteLine($"RepositoryPath: {path}");

            // when
            database.Dispatcher.Invoke(() =>
            {
                var template = database.TableContext.Root.NewTable(authentication);
                template.SetTableName(authentication, tableName);
                var column = template.AddNew(authentication);
                column.SetIsKey(authentication, true);
                column.SetName(authentication, "Column1");
                template.EndNew(authentication, column);
                template.EndEdit(authentication);
            });

            // then
            database.Dispatcher.Invoke(() =>
            {
                var tableContext = (TableContext) database.TableContext;
                this.output.WriteLine($"tableContext.BasePath: {tableContext.BasePath}");
                Assert.Contains(database.TableContext.Tables, table =>
                {
                    this.output.WriteLine($"XmlPath: {tableContext.GenerateTableXmlPath(table.Category.Path, table.Name)}");
                    this.output.WriteLine($"XsdPath: {tableContext.GenerateCategoryPath(table.Category.Path)}");
                    return table.Dispatcher.Invoke(() => table.TableName == tableName);
                });
            });
            var infos = SvnRevisionInfoEventArgs.Run(path);
            this.output.WriteLine(infos.ToJson());
            Assert.Contains(infos, info => info.Name == "branches/testdb01/tables/Table1.xml" && info.Revision > 1);
            Assert.Contains(infos, info => info.Name == "branches/testdb01/tables/Table1.xsd" && info.Revision > 1);
        }
    }
}
