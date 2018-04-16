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
using Ntreev.Crema.Runtime.Generation.TypeScript.TypeScript;
using Ntreev.Crema.Runtime.Generation.TypeScript.Properties;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.ComponentModel.Composition;
using Ntreev.Library;
using Ntreev.Library.IO;
using System.Reflection;

namespace Ntreev.Crema.Runtime.Generation.TypeScript
{
    [Export(typeof(ICodeGenerator))]
    [Export(typeof(ICodeCompiler))]
    class CremaCodeGenerator : ICodeGenerator, ICodeCompiler, ICodePropertyProvider
    {
        public const string CreatedDateTime = "createdDateTime";
        public const string Creator = "creator";
        public const string ModifiedDateTime = "modifiedDateTime";
        public const string Modifier = "modifier";
        public const string ContentsModifiedDateTime = "contentsModifiedDateTime";
        public const string ContentsModifier = "contentsModifier";

        private readonly CodeGeneratorOptions options = new CodeGeneratorOptions();

        public CremaCodeGenerator()
        {
            this.options.BlankLinesBetweenMembers = true;
        }

        public void Generate(string outputPath, GenerationSet metaData, CodeGenerationSettings settings)
        {
            var generationInfo = new CodeGenerationInfo(outputPath, metaData, settings);

            if (settings.Options.HasFlag(CodeGenerationOptions.OmitCode) == false)
            {
                var codes = this.GenerateCodes(generationInfo);

                foreach (var item in codes)
                {
                    var codePath = FileUtility.WriteAllText(item.Value, Encoding.UTF8, outputPath, item.Key);
                    this.PrintResult(codePath);
                }
            }

            if (settings.Options.HasFlag(CodeGenerationOptions.OmitBaseCode) == false)
            {
                var codes = this.GenerateBaseCodes(generationInfo);

                foreach (var item in codes)
                {
                    var codePath = FileUtility.WriteAllText(item.Value, Encoding.UTF8, outputPath, item.Key);
                    this.PrintResult(codePath);
                }
            }
        }

        public void Compile(string outputPath, GenerationSet metaData, CodeGenerationSettings settings, string target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            var generationInfo = new CodeGenerationInfo(outputPath, metaData, settings);

            var codes1 = this.GenerateCodes(generationInfo);
            var codes2 = this.GenerateBaseCodes(generationInfo);
            var codes3 = codes1.Concat(codes2).ToDictionary(item => item.Key, item => item.Value);

            var codes = TypeScriptCompiler.Compile(codes3, target == string.Empty ? "ES5" : target);

            if (Directory.Exists(outputPath) == false)
                Directory.CreateDirectory(outputPath);

            foreach (var item in codes)
            {
                string codePath = Path.Combine(outputPath, item.Key);

                using (StreamWriter sw = new StreamWriter(codePath, false, Encoding.UTF8))
                {
                    sw.WriteLine(item.Value);
                    this.PrintResult(codePath);
                }
            }
        }

        public string[] SupportedTargets
        {
            get { return new string[] { "ES5", }; }
        }

        public virtual string Name
        {
            get { return "ts"; }
        }

        public string Lint
        {
            get; set;
        }

        public bool OmitDeclaration
        {
            get; set;
        }

        private string GenerateBase(CodeGenerationInfo generationInfo)
        {
            var sb = new StringBuilder();
            this.AppendLint(sb, generationInfo);
            var text = this.GetResourceString("Reader.crema-base.ts");
            var text2 = text.Replace("./crema-reader", string.Format("./{0}-reader", generationInfo.Namespace));
            sb.Append(text2);
            return sb.ToString();
        }

        private string GenerateReader(CodeGenerationInfo generationInfo)
        {
            var sb = new StringBuilder();
            this.AppendLint(sb, generationInfo);
            var text = this.GetResourceString("Reader.crema-reader.ts");
            sb.Append(text);
            return sb.ToString();
        }

        private string GenerateTables(CodeDomProvider codeDomProvider, CodeGeneratorOptions options, CodeGenerationInfo generationInfo)
        {
            var sb = new StringBuilder();
            ColumnInfoExtensions.TypeNamespace = "types";
            using (var writer = new StringWriter(sb))
            {
                var codeGenerator = codeDomProvider.CreateGenerator(writer);
                var compileUnit = new CodeCompileUnit();
                var codeNamespace = new CodeNamespace();

                this.AppendLint(sb, generationInfo);

                codeNamespace.Imports.Add(new CodeNamespaceImport($"reader = require(\"./{generationInfo.RelativePath}{generationInfo.Namespace}-reader\")"));
                codeNamespace.Imports.Add(new CodeNamespaceImport($"base = require(\"./{generationInfo.RelativePath}{generationInfo.Namespace}-base\")"));
                if (generationInfo.Types.Any() == true)
                    codeNamespace.Imports.Add(new CodeNamespaceImport($"types = require(\"./{generationInfo.Namespace}-types\")"));

                CremaDataClassCreator.Create(codeNamespace, generationInfo);
                compileUnit.Namespaces.Add(codeNamespace);
                codeGenerator.GenerateCodeFromCompileUnit(compileUnit, writer, options);
            }

            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), "^\\s+$", string.Empty, System.Text.RegularExpressions.RegexOptions.Multiline);
        }

        private string GenerateTypes(CodeDomProvider codeDomProvider, CodeGeneratorOptions options, CodeGenerationInfo generationInfo)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var codeGenerator = codeDomProvider.CreateGenerator(sw);
                var compileUnit = new CodeCompileUnit();
                var codeNamespace = new CodeNamespace();

                this.AppendLint(sb, generationInfo);

                CremaTypeEnumCreator.CreateTypes(codeNamespace, generationInfo);

                compileUnit.Namespaces.Add(codeNamespace);
                codeGenerator.GenerateCodeFromCompileUnit(compileUnit, sw, options);
            }
            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), "^\\s+$", string.Empty, System.Text.RegularExpressions.RegexOptions.Multiline);
        }

        private string GenerateDeclaration(CodeDomProvider codeDomProvider, CodeGeneratorOptions options, CodeGenerationInfo generationInfo)
        {
            ColumnInfoExtensions.TypeNamespace = string.Empty;
            var sb = new StringBuilder();
            this.AppendLint(sb, generationInfo);
            using (StringWriter sw = new StringWriter(sb))
            {
                var codeGenerator = codeDomProvider.CreateGenerator(sw);
                var compileUnit = new CodeCompileUnit();
                var codeNamespace = new CodeNamespace("'" + generationInfo.Namespace + "'");
                codeNamespace.UserData["decl"] = true;

                CremaDeclCreator.Create(codeNamespace, generationInfo);

                compileUnit.Namespaces.Add(codeNamespace);
                codeGenerator.GenerateCodeFromCompileUnit(compileUnit, sw, options);
            }

            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), "^\\s+$", string.Empty, System.Text.RegularExpressions.RegexOptions.Multiline);
        }

        private void PrintResult(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            Console.WriteLine("generated : {0}", fileInfo.FullName);
        }

        private string GetResourceString(string name)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fullname = string.Join(".", assembly.GetName().Name, name);
            using (var stream = new StreamReader(assembly.GetManifestResourceStream(fullname)))
            {
                return stream.ReadToEnd();
            }
        }

        private IDictionary<string, string> GenerateCodes(CodeGenerationInfo generationInfo)
        {
            var codes = new Dictionary<string, string>();
            var codeDomProvider = new CodeDom.TypeScriptCodeProvider();
            var options = new CodeGeneratorOptions() { BlankLinesBetweenMembers = true, };

            var cremaTypes = GenerateTypes(codeDomProvider, options, generationInfo);
            var cremaTables = GenerateTables(codeDomProvider, options, generationInfo);
            var cremaDecl = GenerateDeclaration(codeDomProvider, options, generationInfo);

            codes.Add($"{generationInfo.Namespace}-{generationInfo.Prefix}types{generationInfo.Postfix}.ts", cremaTypes);
            codes.Add($"{generationInfo.Namespace}-{generationInfo.Prefix}tables{generationInfo.Postfix}.ts", cremaTables);
            if (generationInfo.OmitDeclaration == false)
                codes.Add($"{generationInfo.Namespace}.d.ts", cremaDecl);

            return codes;
        }

        private IDictionary<string, string> GenerateBaseCodes(CodeGenerationInfo generationInfo)
        {
            var codes = new Dictionary<string, string>();
            var baseCode = GenerateBase(generationInfo);
            var readerCode = GenerateReader(generationInfo);

            codes.Add($"{generationInfo.RelativePath}{generationInfo.Namespace}-base.ts", baseCode);
            codes.Add($"{generationInfo.RelativePath}{generationInfo.Namespace}-reader.ts", readerCode);

            return codes;
        }

        private void AppendLint(StringBuilder sb, CodeGenerationInfo generationInfo)
        {
            if (generationInfo.TSLintDisable != null)
            {
                if (generationInfo.TSLintDisable == string.Empty)
                    sb.AppendLine($"// tslint:disable");
                else
                    sb.AppendLine($"// tslint:disable: {generationInfo.TSLintDisable}");
                sb.AppendLine();
            }
        }

        #region ICodePropertyProvider

        PropertyInfo[] ICodePropertyProvider.Properties
        {
            get
            {
                return new PropertyInfo[]
                {
                    typeof(CremaCodeGenerator).GetProperty(nameof(Lint)),
                };
            }
        }

        object ICodePropertyProvider.Target => this;

        #endregion
    }
}
