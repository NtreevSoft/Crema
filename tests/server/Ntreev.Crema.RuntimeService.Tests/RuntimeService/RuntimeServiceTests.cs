using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ntreev.Crema.Code.Reader;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Crema.Services;
using Ntreev.CremaServer.Tests;
using Ntreev.CremaServer.Tests.Extensions;
using Ntreev.Library;
using Ntreev.Library.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.RuntimeService.Tests.RuntimeService
{
    [Collection("Ntreev.Crema.RuntimeService.Tests")]
    public class RuntimeServiceTests
    {
        private readonly ITestOutputHelper output;
        private readonly CremaServerTestFixture fixture;

        public RuntimeServiceTests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("equals_types_value", "equals_types_value_user01", "Table01", "Type01")]
        public void EqualsTypeValueAfterExtractCremaDataAlways(string databaseName, string userId, string tableName, string typeName)
        {
            // given
            var (authentication, dataBase) = this.fixture.InitDataBaseAndUser(databaseName, userId);
            CreateType(authentication, dataBase, typeName);
            CreateTable(authentication, dataBase, tableName, typeName);

            // when
            var runtimeService = this.fixture.CremaHost.GetService<IRuntimeService>();
            var generationData = runtimeService.GetDataGenerationData(databaseName, TagInfo.All.ToString(), "", false, -1).Value;
            var dataSet = GetSerializedDataSet(generationData);
            PrintGenerationData(dataSet);
            
            EditType(authentication, dataBase, typeName);

            generationData = runtimeService.GetDataGenerationData(databaseName, TagInfo.All.ToString(), "", false, -1).Value;
            dataSet = GetSerializedDataSet(generationData);
            PrintGenerationData(dataSet);

            // then
            var row = dataSet.Tables[tableName].Rows.Find("data1");
            Assert.Equal(10, row["Column2"]);
        }

        private IDataSet GetSerializedDataSet(SerializationSet generationData)
        {
            var serializer = this.fixture.CremaHost.GetService<IEnumerable<IDataSerializer>>().First(o => o.Name == "bin");
            var ms = new MemoryStream();
            serializer.Serialize(ms, generationData);
            ms.Position = 0;
            var dataSet = CremaReader.Read(ms);
            return dataSet;
        }

        private void CreateType(Authentication authentication, IDataBase dataBase, string typeName)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var template = dataBase.TypeContext.Root.NewType(authentication);
                template.SetTypeName(authentication, typeName);
                template.CreateAddNew(authentication, "Member1", 0);
                template.CreateAddNew(authentication, "Member2", 1);
                template.CreateAddNew(authentication, "Member3", 2);
                template.EndEdit(authentication);
            });
        }

        private void CreateTable(Authentication authentication, IDataBase dataBase, string tableName, string typeName)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var template = dataBase.TableContext.Root.NewTable(authentication);
                template.SetTableName(authentication, tableName);
                template.CreateAddNew(authentication, "Column1", isKey: true);
                template.CreateAddNew(authentication, "Column2", dataType: typeName);
                template.EndEdit(authentication);
            });

            dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                table.Content.BeginEdit(authentication);
                table.Content.EnterEdit(authentication);

                var row = table.Content.AddNew(authentication, null);
                row.SetField(authentication, "Column1", "data1");
                row.SetField(authentication, "Column2", "Member1");
                table.Content.EndNew(authentication, row);

                table.Content.LeaveEdit(authentication);
                table.Content.EndEdit(authentication);
            });
        }

        private void EditType(Authentication authentication, IDataBase dataBase, string typeName)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var type = dataBase.TypeContext.Types[typeName];
                var template = type.Template;
                template.BeginEdit(authentication);
                template["Member1"].SetValue(authentication, 10);
                template.EndEdit(authentication);
            });
        }

        private void PrintGenerationData(IDataSet dataSet)
        {
            foreach (var table in dataSet.Tables)
            {
                output.WriteLine($"{table.Name}");

                foreach (var row in table.Rows)
                {
                    for (var i = 0; i < table.Columns.Count; i++)
                    {
                        output.WriteLine($"- {row[i]}");
                    }
                }
            }
        }
    }
}
