using System;
using System.Collections.Generic;
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
    public class TypeUpdateRevisionTests
    {
        private readonly ITestOutputHelper output;
        private readonly CremaServerTestFixture fixture;

        public TypeUpdateRevisionTests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("Default", "typetestuser_01", "Type1")]
        public void UpdateRevisionTest(string databaseName, string userId, string typeName)
        {
            // given
            var (authentication, database) = this.fixture.InitDataBaseAndUser(databaseName, userId);
            var firstRevision = 0L;
            var secondRevision = 0L;
            firstRevision = CreateTypeTemplate(typeName, database, authentication);

            // when
            secondRevision = AddTypeColumn(typeName, database, authentication);

            // then
            Assert.True(firstRevision+1 == secondRevision);
        }

        private long AddTypeColumn(string typeName, IDataBase database, Authentication authentication)
        {
            return database.Dispatcher.Invoke(() =>
            {
                var type = database.TypeContext.Types[typeName];
                type.Template.BeginEdit(authentication);
                type.Template.CreateAddNew(authentication, "Member2", 2);
                type.Template.EndEdit(authentication);

                return type.Dispatcher.Invoke(() =>
                {
                    this.output.WriteLine($"table.GetLog: {type.GetLog(authentication).First().ToJson()}");
                    this.output.WriteLine($"table.Revision: {type.Name} revision: {type.Revision}");

                    var typeInfo = type.TypeInfo;
                    Assert.Contains(typeInfo.Members, m => m.Name == "Member2");

                    return type.Revision;
                });
            });
        }

        private long CreateTypeTemplate(string typeName, IDataBase database, Authentication authentication)
        {
            return database.Dispatcher.Invoke(() =>
            {
                var template = database.TypeContext.Root.NewType(authentication);
                template.SetTypeName(authentication, typeName);
                template.CreateAddNew(authentication, "Member1", 1);
                template.EndEdit(authentication);

                Assert.Contains(database.TypeContext.Types, t => t.Dispatcher.Invoke(() => t.Name == typeName));

                var type = database.TypeContext.Types[typeName];
                return type.Dispatcher.Invoke(() =>
                {
                    this.output.WriteLine($"table.GetLog: {type.GetLog(authentication).First().ToJson()}");
                    this.output.WriteLine($"table.Revision: {type.Name} revision: {type.Revision}");

                    var typeInfo = type.TypeInfo;
                    Assert.Contains(typeInfo.Members, m => m.Name == "Member1");

                    return type.Revision;
                });
            });
        }
    }
}
