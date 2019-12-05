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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Services;
using Ntreev.Crema.Spreadsheet.Csv;
using Ntreev.Crema.Spreadsheet.Excel;
using Ntreev.CremaServer.Tests;
using Ntreev.CremaServer.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.Spreadsheet.Tests.Csv
{
    [Collection("Ntreev.Crema.Spreadsheet.Tests")]
    public class SpreadsheetCsvWriterTests
    {
        private readonly ITestOutputHelper output;
        private readonly CremaServerTestFixture fixture;

        public SpreadsheetCsvWriterTests(ITestOutputHelper output,
            CremaServerTestFixture fixture)
        {
            this.output = output;
            this.fixture = fixture;

            this.output.WriteLine($"RepositoryPath: {fixture.CremaHost.RepositoryPath}");
        }

        [Theory]
        [InlineData("default", "testuser_01", "Table1", "Child1")]
        public void WriteCsvTest(string databaseName, string userId, string tableName, string childTableName)
        {
            // given
            var (authentication, dataBase) = this.fixture.InitDataBaseAndUser(databaseName, userId);
            CreateTableTemplate(tableName, dataBase, authentication);
            AddChildTableData(tableName, childTableName, dataBase, authentication);
            AddTableData(tableName, childTableName, dataBase, authentication);

            // when
            var dataSet = dataBase.Dispatcher.Invoke(() => dataBase.TableContext.Tables[tableName].GetDataSet(authentication, -1));
            var writer = new SpreadsheetCsvWriter(dataSet, new SpreadsheetCsvWriterSettings
            {
                OmitAttribute = false,
            });
            writer.Write("d:\\tmp\\t-{categoryName}{name}{extension}");
        }

        private static void CreateTableTemplate(string tableName, IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var template = dataBase.TableContext.Root.NewTable(authentication);
                template.SetTableName(authentication, tableName);
                template.CreateAddNew(authentication, "ColumnKey", dataType: "string", isKey: true);
                template.CreateAddNew(authentication, "ColumnString", dataType: "string");
                template.CreateAddNew(authentication, "ColumnInt", dataType: "int");
                template.CreateAddNew(authentication, "ColumnDateTime", dataType: "dateTime");
                template.EndEdit(authentication);

                Assert.Contains(dataBase.TableContext.Tables, t => t.Dispatcher.Invoke(() => template.TableName == tableName));
            });
        }

        private void AddTableData(string tableName, string childTableName, IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                table.Dispatcher.Invoke(() =>
                {
                    table.Content.BeginEdit(authentication);
                    table.Content.EnterEdit(authentication);

                    var row1 = table.Content.AddNew(authentication, null);
                    row1.SetField(authentication, "ColumnKey", "row1");
                    row1.SetField(authentication, "ColumnString", "Hello, World");
                    row1.SetField(authentication, "ColumnInt", int.MaxValue);
                    row1.SetField(authentication, "ColumnDateTime", DateTime.Now);
                    table.Content.EndNew(authentication, row1);

                    var row2 = table.Content.AddNew(authentication, null);
                    row2.SetField(authentication, "ColumnKey", "row2");
                    row2.SetField(authentication, "ColumnString", "안 녕 하 세 요");
                    row2.SetField(authentication, "ColumnInt", int.MinValue);
                    row2.SetField(authentication, "ColumnDateTime", DateTime.MinValue);
                    table.Content.EndNew(authentication, row2);

                    var childTable1 = table.Childs[childTableName];
                    var childRow1 = childTable1.Content.AddNew(authentication, row1.RelationID);
                    childRow1.SetField(authentication, "ColumnKey", "row1-1");
                    childRow1.SetField(authentication, "ColumnString", "Hello,World");
                    childRow1.SetField(authentication, "ColumnInt", int.MaxValue);
                    childRow1.SetField(authentication, "ColumnDateTime", DateTime.Now);
                    childTable1.Content.EndNew(authentication, childRow1);

                    table.Content.LeaveEdit(authentication);
                    table.Content.EndEdit(authentication);
                });
            });
        }



        private void AddChildTableData(string tableName, string childTableName, IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                table.Dispatcher.Invoke(() =>
                {
                    var template = table.NewTable(authentication);
                    template.SetTableName(authentication, childTableName);
                    template.CreateAddNew(authentication, "ColumnKey", dataType: "string", isKey: true);
                    template.CreateAddNew(authentication, "ColumnString", dataType: "string");
                    template.CreateAddNew(authentication, "ColumnInt", dataType: "int");
                    template.CreateAddNew(authentication, "ColumnDateTime", dataType: "dateTime");
                    template.EndEdit(authentication);
                });
            });
        }
    }
}
