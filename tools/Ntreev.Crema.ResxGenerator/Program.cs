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

using Microsoft.CSharp;
using Ntreev.Crema.Data;
using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Crema.RuntimeService;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Resources.Tools;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ResxGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();
            var parser = new CommandLineParser(settings);

            try
            {
                parser.Parse(Environment.CommandLine);
                Write(settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        static void Write(Settings settings)
        {
            var runtimeService = Container.Get<IRuntimeService>();
            var data = runtimeService.GetDataGenerationData(settings.Address, settings.DataBaseName, "all", null, false, null);
            var dataSet = SerializationUtility.Create(data);

            var projectInfoTable = dataSet.Tables[settings.ExportName];
            foreach (var item in projectInfoTable.Rows)
            {
                Write(settings.OutputPath, item);
            }
        }

        static void Write(string outputPath, CremaDataRow dataRow)
        {
            var localPath = dataRow.Field<string>("ProjectPath").Replace(PathUtility.SeparatorChar, Path.DirectorySeparatorChar);
            var projectPath = Path.Combine(outputPath, localPath);

            var projectXml = File.ReadAllText(projectPath);
            var projectInfo = new ProjectInfo(projectXml);

            foreach (var item in projectInfo.ResxInfos)
            {
                var dataTable = FindDataTable(item);
                if (dataTable == null)
                    continue;

                Write(Path.GetDirectoryName(projectPath), item, dataTable);

                if (item.ResgenFileName != string.Empty)
                {
                    WriteDesigner(Path.GetDirectoryName(projectPath), projectInfo, item);
                }
            }

            CremaDataTable FindDataTable(ResxInfo resxInfo)
            {
                foreach (var item in dataRow.GetChildRows("ExportInfo"))
                {
                    if (item.Field<string>("FileName") == resxInfo.Name)
                    {
                        var tableName = item.Field<string>("TableName");
                        return dataRow.Table.DataSet.Tables[tableName];
                    }
                }
                return null;
            }
        }

        static void Write(string outputPath, ResxInfo resxInfo, CremaDataTable dataTable)
        {
            var cultureInfo = resxInfo.CultureInfo;
            var valueName = cultureInfo == null ? "Value" : cultureInfo.Name.Replace('-', '_');
            var path = FileUtility.Prepare(outputPath, resxInfo.FileName);
            var writer = new ResXResourceWriter(path);

            foreach (var item in dataTable.Rows)
            {
                var name = $"{item["Type"]}" == "None" ? $"{item["Name"]}" : $"{item["Type"]}_{item["Name"]}";
                var node = new ResXDataNode(name, item[valueName])
                {
                    Comment = $"{item["Comment"]}",
                };
                writer.AddResource(node);
            }
            writer.Close();
            Console.WriteLine(Path.Combine(outputPath, resxInfo.FileName));
        }

        static void WriteDesigner(string outputPath, ProjectInfo projectInfo, ResxInfo resxInfo)
        {
            var resxFileName = Path.Combine(outputPath, resxInfo.FileName);
            var designerFileName = Path.Combine(outputPath, resxInfo.ResgenFileName);
            var ss = Ntreev.Library.StringUtility.SplitPath(Path.GetDirectoryName(resxInfo.FileName));
            var codeNamespace = $"{projectInfo.RootNamespace}.{string.Join(".", ss)}";
            var baseName = Path.GetFileNameWithoutExtension(resxInfo.FileName);

            using (var sw = new StreamWriter(designerFileName))
            {
                var errors = null as string[];
                var provider = new CSharpCodeProvider();
                var code = StronglyTypedResourceBuilder.Create(resxFileName, baseName, codeNamespace, provider, resxInfo.IsPublic == false, out errors);
                if (errors.Length > 0)
                {
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error);
                    }
                    return;
                }

                provider.GenerateCodeFromCompileUnit(code, sw, new CodeGeneratorOptions());
                Console.WriteLine(designerFileName);
            }
        }
    }
}

