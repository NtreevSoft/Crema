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
using Ntreev.Library.IO;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ntreev.Library;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.Runtime.Generation.CSharp
{
    [Export(typeof(ICodeGenerator))]
    [Export(typeof(ICodeCompiler))]
    sealed class CremaCodeGenerator : ICodeGenerator, ICodeCompiler
    {
        private readonly CodeGeneratorOptions options;

        public CremaCodeGenerator()
        {
            this.options = new CodeGeneratorOptions()
            {
                BlankLinesBetweenMembers = true,
                BracingStyle = "C",
                ElseOnClosing = false,
                IndentString = IndentedTextWriter.DefaultTabString
            };
            this.options["CustomGeneratorOptionStringExampleID"] = "BuildFlags: /A /B /C /D /E";
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
            var generationInfo = new CodeGenerationInfo(outputPath, metaData, settings);

            if (settings.Options.HasFlag(CodeGenerationOptions.OmitCode) == false && settings.Options.HasFlag(CodeGenerationOptions.OmitBaseCode) == false)
            {
                var filename = this.CompileAll(outputPath, generationInfo, target);
                this.PrintResult(filename);
            }
            else if (settings.Options.HasFlag(CodeGenerationOptions.OmitCode) == true)
            {
                var filename = this.CompileBase(outputPath, generationInfo, target);
                this.PrintResult(filename);
            }
            else if (settings.Options.HasFlag(CodeGenerationOptions.OmitBaseCode) == true)
            {
                var tempPath = PathUtility.GetTempPath(true);
                try
                {
                    var readerPath = this.CompileBase(tempPath, generationInfo, target);
                    var filename = this.Compile(outputPath, readerPath, generationInfo, target);
                    this.PrintResult(filename);
                }
                finally
                {
                    DirectoryUtility.Delete(tempPath);
                }
            }
        }

        public string[] SupportedTargets
        {
            get { return null; }
        }

        public string Name
        {
            get { return "c#"; }
        }

        private IDictionary<string, string> GenerateCodes(CodeGenerationInfo generationInfo)
        {
            var codes = new Dictionary<string, string>();

            var cremaTypes = this.GenerateTypeCode(generationInfo);
            var cremaTables = this.GenerateTableCode(generationInfo);

            codes.Add($"{generationInfo.Prefix}Types{generationInfo.Postfix}.cs", cremaTypes);
            codes.Add($"{generationInfo.Prefix}Tables{generationInfo.Postfix}.cs", cremaTables);

            foreach (var item in codes.ToArray())
            {
                codes[item.Key] = item.Value.Replace("Ntreev.Crema.Reader", generationInfo.ReaderNamespace);
            }

            return codes;
        }

        private IDictionary<string, string> GenerateBaseCodes(CodeGenerationInfo generationInfo)
        {
            var codes = new Dictionary<string, string>();

            var cremaBase = this.GenerateBase(generationInfo);
            codes.Add($"{generationInfo.RelativePath}CremaBase.cs", cremaBase);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var ns = string.Join(".", this.GetType().Namespace, "Reader");
            var files = assembly.GetManifestResourceNames().Where(item => item.StartsWith(ns));

            foreach (var item in files)
            {
                var value = this.GetResourceString(item);
                var key = Regex.Replace(item, "(^" + this.GetType().Namespace + @"[.])(\S+)[.]cs", "$2").Replace('.', Path.DirectorySeparatorChar);
                value = value.Replace("Ntreev.Crema.Reader", generationInfo.ReaderNamespace);
                codes.Add($"{generationInfo.RelativePath}{key}.cs", value);
            }

            return codes;
        }

        private string GenerateBase(CodeGenerationInfo generationInfo)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = string.Join(".", this.GetType().Namespace, "Code", "CremaBase.cs");
            var code = this.GetResourceString(resourceName);
            code = code.Replace("namespace Ntreev.Crema.Code", "namespace " + generationInfo.BaseNamespace);
            code = code.Replace("Ntreev.Crema.Reader", generationInfo.ReaderNamespace);
            return code;
        }

        private string GenerateTableCode(CodeGenerationInfo generationInfo)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var codeDomProvider = new CSharpCodeProvider();
                var codeGenerator = codeDomProvider.CreateGenerator(sw);
                var compileUnit = new CodeCompileUnit();
                var codeNamespace = new CodeNamespace(generationInfo.Namespace);
                codeNamespace.Imports.Add(new CodeNamespaceImport(generationInfo.ReaderNamespace));
                CremaDataClassCreator.Create(codeNamespace, generationInfo);
                compileUnit.Namespaces.Add(codeNamespace);
                codeGenerator.GenerateCodeFromCompileUnit(compileUnit, sw, this.options);
            }

            return sb.ToString();
        }

        private string GenerateTypeCode(CodeGenerationInfo generationInfo)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var codeDomProvider = new CSharpCodeProvider();
                var codeGenerator = codeDomProvider.CreateGenerator(sw);
                var compileUnit = new CodeCompileUnit();
                var codeNamespace = new CodeNamespace(generationInfo.Namespace);
                codeNamespace.Imports.Add(new CodeNamespaceImport(generationInfo.ReaderNamespace));
                CremaTypeEnumCreator.Create(codeNamespace, generationInfo);
                compileUnit.Namespaces.Add(codeNamespace);
                codeGenerator.GenerateCodeFromCompileUnit(compileUnit, sw, this.options);
            }

            return sb.ToString();
        }

        private string GenerateAssemblyInfo(string assemblyName)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var codeDomProvider = new CSharpCodeProvider();
                var codeGenerator = codeDomProvider.CreateGenerator(sw);
                var compileUnit = new CodeCompileUnit();
                var time = DateTime.Now;
                var build = string.Format("{0:yy}{1}", time, time.DayOfYear);
                var revision = string.Format("{0:HHmm}", time);

                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyTitleAttribute), assemblyName);
                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyDescriptionAttribute), "");
                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyConfigurationAttribute), "");
                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyCompanyAttribute), "NTREEV SOFT");
                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyProductAttribute), assemblyName);
                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyCopyrightAttribute), $"Copyright © NTREEV SOFT {DateTime.Now.Year}");
                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyTrademarkAttribute), "");
                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyCultureAttribute), "");

                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyVersionAttribute), $"{CremaSchema.MajorVersion}.{CremaSchema.MinorVersion}.0.0");
                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyFileVersionAttribute), $"{CremaSchema.MajorVersion}.{CremaSchema.MinorVersion}.{build}.{revision}");
                compileUnit.AssemblyCustomAttributes.Add(typeof(System.Reflection.AssemblyInformationalVersionAttribute), $"{CremaSchema.MajorVersion}.{CremaSchema.MinorVersion}.{build}.{revision}");

                codeGenerator.GenerateCodeFromCompileUnit(compileUnit, sw, this.options);
            }

            return sb.ToString();
        }

        private string CompileBase(string outputPath, CodeGenerationInfo generationInfo, string target)
        {
            var sourceList = new List<string>();

            sourceList.AddRange(this.GenerateBaseCodes(generationInfo).Values);
            sourceList.Add(this.GenerateAssemblyInfo(generationInfo.BaseNamespace + ".Base"));

            var providerOptions = new Dictionary<string, string>
            {
                { "CompilerVersion", target == string.Empty ? "v3.5" : target }
            };
            var codeDomProvider = new CSharpCodeProvider(providerOptions);

            var cp = new CompilerParameters()
            {
                OutputAssembly = Path.Combine(outputPath, generationInfo.BaseNamespace + ".Base.dll"),
                CompilerOptions = "/optimize",
                GenerateExecutable = false,
                GenerateInMemory = false
            };
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");

            FileUtility.Prepare(cp.OutputAssembly);

            var result = codeDomProvider.CompileAssemblyFromSource(cp, sourceList.ToArray());

            if (result.Errors.HasErrors)
            {
                foreach (var item in result.Errors)
                {
                    Trace.WriteLine(item);
                }

                throw new Exception("에러가 발생했습니다.");
            }

            //this.PrintResult(cp.OutputAssembly);
            return cp.OutputAssembly;
        }

        private string Compile(string outputPath, string readerDllPath, CodeGenerationInfo generationInfo, string target)
        {
            var sourceList = new List<string>();

            sourceList.AddRange(this.GenerateCodes(generationInfo).Values);
            sourceList.Add(this.GenerateAssemblyInfo(generationInfo.Namespace));

            var providerOptions = new Dictionary<string, string>
            {
                { "CompilerVersion", target == string.Empty ? "v3.5" : target }
            };
            var codeDomProvider = new CSharpCodeProvider(providerOptions);

            var cp = new CompilerParameters()
            {
                OutputAssembly = Path.Combine(outputPath, generationInfo.Namespace + ".dll"),
                CompilerOptions = "/optimize",
                GenerateExecutable = false,
                GenerateInMemory = false
            };
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add(readerDllPath);

            FileUtility.Prepare(cp.OutputAssembly);

            var result = codeDomProvider.CompileAssemblyFromSource(cp, sourceList.ToArray());

            if (result.Errors.HasErrors)
            {
                foreach (var item in result.Errors)
                {
                    Trace.WriteLine(item);
                }

                throw new Exception("에러가 발생했습니다.");
            }

            //this.PrintResult(cp.OutputAssembly);
            return cp.OutputAssembly;
        }

        private string CompileAll(string outputPath, CodeGenerationInfo generationInfo, string target)
        {
            var sourceList = new List<string>();

            sourceList.AddRange(this.GenerateCodes(generationInfo).Values);
            sourceList.AddRange(this.GenerateBaseCodes(generationInfo).Values);

            sourceList.Add(this.GenerateAssemblyInfo(generationInfo.Namespace));

            var providerOptions = new Dictionary<string, string>
            {
                { "CompilerVersion", target == string.Empty ? "v3.5" : target }
            };
            var codeDomProvider = new CSharpCodeProvider(providerOptions);

            var cp = new CompilerParameters()
            {
                OutputAssembly = Path.Combine(outputPath, generationInfo.Namespace + ".dll"),
                CompilerOptions = "/optimize",
                GenerateExecutable = false,
                GenerateInMemory = false
            };
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");

            FileUtility.Prepare(cp.OutputAssembly);

            var result = codeDomProvider.CompileAssemblyFromSource(cp, sourceList.ToArray());

            if (result.Errors.HasErrors)
            {
                foreach (var item in result.Errors)
                {
                    Trace.WriteLine(item);
                }

                throw new Exception("에러가 발생했습니다.");
            }

            return cp.OutputAssembly;
        }

        private string GetResourceString(string resourceName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var stream = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
            {
                return stream.ReadToEnd();
            }
        }

        private void PrintResult(string path)
        {
            var fileInfo = new FileInfo(path);
            Trace.WriteLine($"generated : {fileInfo.FullName}");
        }
    }
}
