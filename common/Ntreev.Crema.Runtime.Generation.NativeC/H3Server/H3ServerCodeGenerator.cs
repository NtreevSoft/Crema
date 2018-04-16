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

using Ntreev.Crema.ServiceModel;
using Ntreev.Library.IO;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Generation.NativeC.H3Server
{
    [Export(typeof(ICodeGenerator))]
    class H3ServerCodeGenerator : ICodeGenerator
    {
        public void Generate(string outputPath, GenerationSet metaData, CodeGenerationSettings settings)
        {
            var generationInfo = new CodeGenerationInfo(metaData, settings);

            var codes = this.Generate(generationInfo);

            foreach (var item in codes)
            {
                string codePath = FileUtility.Prepare(outputPath, item.Key);
                
                using (StreamWriter sw = new StreamWriter(codePath, false, Encoding.UTF8))
                {
                    sw.WriteLine(item.Value);
                    this.PrintResult(codePath);
                }
            }
        }

        public IDictionary<string, string> Generate(CodeGenerationInfo generationInfo)
        {
            Dictionary<string, string> codes = new Dictionary<string, string>();


            CodeDomProvider codeDomProvider = new CodeDom.NativeCCodeProvider();

            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = generationInfo.BlankLinesBetweenMembers;
            options.BracingStyle = "C";
            options.ElseOnClosing = false;
            options.IndentString = IndentedTextWriter.DefaultTabString;
            options["CustomGeneratorOptionStringExampleID"] = "BuildFlags: /A /B /C /D /E";

            ColumnInfoExtensions.TypeNamespace = generationInfo.Namespace;

            var cremaTypes = GenerateTypes(codeDomProvider, options, generationInfo);
            var cremaTables = GenerateTables(codeDomProvider, options, generationInfo);

            string cremaTypesHeader, cremaTypesCpp;
            string cremaTablesHeader, cremaTablesCpp;

            SplitCode(cremaTypes, "crema_datatypes.h", out cremaTypesHeader, out cremaTypesCpp);
            SplitCode(cremaTables, "crema_tables.h", out cremaTablesHeader, out cremaTablesCpp);

            codes.Add("crema_datatypes.h", cremaTypesHeader);
            codes.Add("crema_datatypes.cpp", cremaTypesCpp);
            codes.Add("crema_tables.h", cremaTablesHeader);
            codes.Add("crema_tables.cpp", cremaTablesCpp);

            return codes;
        }

        public string Name
        {
            get { return "h3-server"; }
        }

        private void PrintResult(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            Console.WriteLine("generated : {0}", fileInfo.FullName);
        }

        private static void SplitCode(string code, string filename, out string header, out string cpp)
        {
            int index = code.IndexOf("/// cpp start");
            if (index >= 0)
            {
                header = code.Substring(0, index);
                cpp = code.Substring(index);

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("#include \"{0}\"", Path.GetFileName(filename));
                sb.AppendLine();
                sb.Append(cpp);
                cpp = sb.ToString();
                cpp = cpp.Replace("/// cpp start", string.Empty);
                cpp = cpp.Replace("/// cpp end", string.Empty);
            }
            else
            {
                header = code;
                cpp = null;
            }
        }

        private string GenerateTables(CodeDomProvider codeDomProvider, CodeGeneratorOptions options, CodeGenerationInfo generationInfo)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                var codeGenerator = codeDomProvider.CreateGenerator(sw);
                CodeCompileUnit compileUnit = new CodeCompileUnit();

                //compileUnit.AddCustomInclude("reader/include/crema/inidata");
                compileUnit.AddInclude("crema/inidata");
                compileUnit.AddCustomInclude("crema_datatypes");
                //compileUnit.AddCustomInclude("crema_base");

                CodeNamespace codeNamespace = new CodeNamespace(generationInfo.Namespace);
                codeNamespace.Imports.Add(new CodeNamespaceImport(generationInfo.ReaderNamespace));

                foreach (var item in generationInfo.GetTables())
                {
                    H3RowClassCreator.Create(codeNamespace, item, generationInfo);
                }

                compileUnit.Namespaces.Add(codeNamespace);

                codeGenerator.GenerateCodeFromCompileUnit(compileUnit, sw, options);
            }

            return sb.ToString();
        }

        private string GenerateTypes(CodeDomProvider codeDomProvider, CodeGeneratorOptions options, CodeGenerationInfo generationInfo)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                var codeGenerator = codeDomProvider.CreateGenerator(sw);
                CodeCompileUnit compileUnit = new CodeCompileUnit();

                CodeNamespace codeNamespace = new CodeNamespace(generationInfo.Namespace);
                codeNamespace.Imports.Add(new CodeNamespaceImport(generationInfo.ReaderNamespace));

                CremaTypeEnumCreator.NoCpp = true;
                CremaTypeEnumCreator.Create(codeNamespace, generationInfo);

                compileUnit.Namespaces.Add(codeNamespace);

                codeGenerator.GenerateCodeFromCompileUnit(compileUnit, sw, options);
            }

            return sb.ToString();
        }
    }
}
