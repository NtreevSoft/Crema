using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Code;
using Ntreev.Crema.Code.Reader;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.Runtime.Generation.CSharpGenerationTests
{
    [Collection("Ntreev.Crema.Runtime.Generation")]
    public class CodeGenerationTests
    {
        private readonly ITestOutputHelper output;
        private readonly string DATAFILE_PATH = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), 
            "CSharpGenerationTests", "Code", "crema.dat");

        public CodeGenerationTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void CremaReaderTest()
        {
            var sourceList = new List<string>();
            var targetList = new List<string>();

            var reader = CremaReader.Read(DATAFILE_PATH);
            foreach (var table in reader.Tables)
            {
                foreach (var row in table.Rows)
                {
                    var sb = new StringBuilder();
                    foreach (var column in table.Columns)
                    {
                        if (!column.IsKey) continue;
                        var value = row[column.Name] + ", ";
                        sb.Append(value);
                    }
                    this.output.WriteLine(sb.ToString());
                    sourceList.Add(sb.ToString());
                }
            }

            this.output.WriteLine("----");

            var dataSet = new CremaDataSet(DATAFILE_PATH, false);
            foreach (var row in dataSet.Table1.Rows)
            {
                var value = $"{row.StringKey}, {row.IntKey}, {(int)row.TypeKey}, {row.FloatKey}, ";
                this.output.WriteLine(value);
                targetList.Add(value);
            }

            for (var i = 0; i < sourceList.Count; i++)
            {
                Assert.Equal(sourceList[i], targetList[i]);
            }
        }
    }
}
