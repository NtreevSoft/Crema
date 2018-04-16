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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Generation.TypeScript.CodeDom
{
    static class wow
    {
        public static void InternalOutputTabs(this IndentedTextWriter writer)
        {
            for (int i = 0; i < writer.Indent; i++)
            {
                writer.Write(IndentedTextWriter.DefaultTabString);
            }
        }
    }

    class TypeScriptCodeGenerator : System.CodeDom.Compiler.ICodeCompiler, System.CodeDom.Compiler.ICodeGenerator
    {
        //private string regexPattern = "(^[a-zA-Z])|(^_[a-zA-Z])[^A-Z]";
        private string regexPattern = "(^[A-Z])[^A-Z]";

        private IndentedTextWriter output;

        private CodeGeneratorOptions options;

        private CodeTypeDeclaration currentClass;

        private CodeTypeMember currentMember;

        private bool inNestedBinary;

        private IDictionary<string, string> provOptions;

        private const int ParameterMultilineThreshold = 15;

        private const int MaxLineLength = 80;

        private const GeneratorSupport LanguageSupport = GeneratorSupport.ArraysOfArrays | GeneratorSupport.EntryPointMethod | GeneratorSupport.GotoStatements | GeneratorSupport.MultidimensionalArrays | GeneratorSupport.StaticConstructors | GeneratorSupport.TryCatchStatements | GeneratorSupport.ReturnTypeAttributes | GeneratorSupport.DeclareValueTypes | GeneratorSupport.DeclareEnums | GeneratorSupport.DeclareDelegates | GeneratorSupport.DeclareInterfaces | GeneratorSupport.DeclareEvents | GeneratorSupport.AssemblyAttributes | GeneratorSupport.ParameterAttributes | GeneratorSupport.ReferenceParameters | GeneratorSupport.ChainedConstructorArguments | GeneratorSupport.NestedTypes | GeneratorSupport.MultipleInterfaceMembers | GeneratorSupport.PublicStaticMembers | GeneratorSupport.ComplexExpressions | GeneratorSupport.Win32Resources | GeneratorSupport.Resources | GeneratorSupport.PartialTypes | GeneratorSupport.GenericTypeReference | GeneratorSupport.GenericTypeDeclaration | GeneratorSupport.DeclareIndexerProperties;

        private static volatile Regex outputRegWithFileAndLine;

        private static volatile Regex outputRegSimple;

        private static readonly string[] keywords = new string[]
        {
                "as",
                "do",
                "if",
                "in",
                "is",
                "for",
                "int",
                "new",
                "out",
                "ref",
                "try",
                "bool",
                "byte",
                "case",
                "char",
                "else",
                "enum",
                "goto",
                "lock",
                "long",
                "null",
                "this",
                "true",
                "uint",
                "void",
                "break",
                "catch",
                "class",
                "const",
                "event",
                "false",
                "fixed",
                "throw",
                "while",
                "object",
                "params",
                "public",
                "return",
                "sealed",
                "sizeof",
                "static",
                "string",
                "struct",
                "switch",
                "typeof",
                "unsafe",
                "ushort",
                "checked",
                "decimal",
                "default",
                "finally",
                "foreach",
                "private",
                "virtual",
                "abstract",
                "continue",
                "delegate",
                "explicit",
                "implicit",
                "internal",
                "operator",
                "override",
                "readonly",
                "volatile",
                "__arglist",
                "__makeref",
                "__reftype",
                "interface",
                "protected",
                "unchecked",
                "__refvalue",
                "stackalloc",
        };

        private bool generatingForLoop;

        private string FileExtension
        {
            get
            {
                return ".ts";
            }
        }

        private string CompilerName
        {
            get
            {
                return "tsc.exe";
            }
        }

        private string CurrentTypeName
        {
            get
            {
                if (this.currentClass != null)
                {
                    return this.currentClass.Name;
                }
                return "<% unknown %>";
            }
        }

        private int Indent
        {
            get
            {
                return this.output.Indent;
            }
            set
            {
                this.output.Indent = value;
            }
        }

        private bool IsCurrentInterface
        {
            get
            {
                return this.currentClass != null && !(this.currentClass is CodeTypeDelegate) && this.currentClass.IsInterface;
            }
        }

        private bool IsCurrentClass
        {
            get
            {
                return this.currentClass != null && !(this.currentClass is CodeTypeDelegate) && this.currentClass.IsClass;
            }
        }

        private bool IsCurrentStruct
        {
            get
            {
                return this.currentClass != null && !(this.currentClass is CodeTypeDelegate) && this.currentClass.IsStruct;
            }
        }

        private bool IsCurrentEnum
        {
            get
            {
                return this.currentClass != null && !(this.currentClass is CodeTypeDelegate) && this.currentClass.IsEnum;
            }
        }

        private bool IsCurrentDelegate
        {
            get
            {
                return this.currentClass != null && this.currentClass is CodeTypeDelegate;
            }
        }

        private string NullToken
        {
            get
            {
                return "null";
            }
        }

        private CodeGeneratorOptions Options
        {
            get
            {
                return this.options;
            }
        }

        private TextWriter Output
        {
            get
            {
                return this.output;
            }
        }

        internal TypeScriptCodeGenerator()
        {
        }

        internal TypeScriptCodeGenerator(IDictionary<string, string> providerOptions)
        {
            this.provOptions = providerOptions;
        }

        private string QuoteSnippetStringCStyle(string value)
        {
            StringBuilder stringBuilder = new StringBuilder(value.Length + 5);
            Indentation indentation = new Indentation((IndentedTextWriter)this.Output, this.Indent + 1, IndentedTextWriter.DefaultTabString);
            stringBuilder.Append("\"");
            int i = 0;
            while (i < value.Length)
            {
                char c = value[i];
                if (c <= '"')
                {
                    if (c != '\0')
                    {
                        switch (c)
                        {
                            case '\t':
                                stringBuilder.Append("\\t");
                                break;
                            case '\n':
                                stringBuilder.Append("\\n");
                                break;
                            case '\v':
                            case '\f':
                                goto IL_10C;
                            case '\r':
                                stringBuilder.Append("\\r");
                                break;
                            default:
                                if (c != '"')
                                {
                                    goto IL_10C;
                                }
                                stringBuilder.Append("\\\"");
                                break;
                        }
                    }
                    else
                    {
                        stringBuilder.Append("\\0");
                    }
                }
                else if (c <= '\\')
                {
                    if (c != '\'')
                    {
                        if (c != '\\')
                        {
                            goto IL_10C;
                        }
                        stringBuilder.Append("\\\\");
                    }
                    else
                    {
                        stringBuilder.Append("\\'");
                    }
                }
                else
                {
                    if (c != '\u2028' && c != '\u2029')
                    {
                        goto IL_10C;
                    }
                    this.AppendEscapedChar(stringBuilder, value[i]);
                }
            IL_11A:
                if (i > 0 && i % 80 == 0)
                {
                    if (char.IsHighSurrogate(value[i]) && i < value.Length - 1 && char.IsLowSurrogate(value[i + 1]))
                    {
                        stringBuilder.Append(value[++i]);
                    }
                    stringBuilder.Append("\" +");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append(indentation.IndentationString);
                    stringBuilder.Append('"');
                }
                i++;
                continue;
            IL_10C:
                stringBuilder.Append(value[i]);
                goto IL_11A;
            }
            stringBuilder.Append("\"");
            return stringBuilder.ToString();
        }

        private string QuoteSnippetStringVerbatimStyle(string value)
        {
            StringBuilder stringBuilder = new StringBuilder(value.Length + 5);
            stringBuilder.Append("@\"");
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '"')
                {
                    stringBuilder.Append("\"\"");
                }
                else
                {
                    stringBuilder.Append(value[i]);
                }
            }
            stringBuilder.Append("\"");
            return stringBuilder.ToString();
        }

        private string QuoteSnippetString(string value)
        {
            if (value.Length < 256 || value.Length > 1500 || value.IndexOf('\0') != -1)
            {
                return this.QuoteSnippetStringCStyle(value);
            }
            return this.QuoteSnippetStringVerbatimStyle(value);
        }

        private void ProcessCompilerOutputLine(CompilerResults results, string line)
        {
            if (TypeScriptCodeGenerator.outputRegSimple == null)
            {
                TypeScriptCodeGenerator.outputRegWithFileAndLine = new Regex("(^(.*)(\\(([0-9]+),([0-9]+)\\)): )(error|warning) ([A-Z]+[0-9]+) ?: (.*)");
                TypeScriptCodeGenerator.outputRegSimple = new Regex("(error|warning) ([A-Z]+[0-9]+) ?: (.*)");
            }
            Match match = TypeScriptCodeGenerator.outputRegWithFileAndLine.Match(line);
            bool flag;
            if (match.Success)
            {
                flag = true;
            }
            else
            {
                match = TypeScriptCodeGenerator.outputRegSimple.Match(line);
                flag = false;
            }
            if (match.Success)
            {
                CompilerError compilerError = new CompilerError();
                if (flag)
                {
                    compilerError.FileName = match.Groups[2].Value;
                    compilerError.Line = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
                    compilerError.Column = int.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture);
                }
                if (string.Compare(match.Groups[flag ? 6 : 1].Value, "warning", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    compilerError.IsWarning = true;
                }
                compilerError.ErrorNumber = match.Groups[flag ? 7 : 2].Value;
                compilerError.ErrorText = match.Groups[flag ? 8 : 3].Value;
                results.Errors.Add(compilerError);
            }
        }

        //private string CmdArgsFromParameters(CompilerParameters options)
        //{
        //    StringBuilder stringBuilder = new StringBuilder(128);
        //    if (options.GenerateExecutable)
        //    {
        //        stringBuilder.Append("/t:exe ");
        //        if (options.MainClass != null && options.MainClass.Length > 0)
        //        {
        //            stringBuilder.Append("/main:");
        //            stringBuilder.Append(options.MainClass);
        //            stringBuilder.Append(" ");
        //        }
        //    }
        //    else
        //    {
        //        stringBuilder.Append("/t:library ");
        //    }
        //    stringBuilder.Append("/utf8output ");
        //    string text = options.CoreAssemblyFileName;
        //    string text2;
        //    if (string.IsNullOrWhiteSpace(options.CoreAssemblyFileName) && CodeDomProvider.TryGetProbableCoreAssemblyFilePath(options, out text2))
        //    {
        //        text = text2;
        //    }
        //    if (!string.IsNullOrWhiteSpace(text))
        //    {
        //        stringBuilder.Append("/nostdlib+ ");
        //        stringBuilder.Append("/R:\"").Append(text.Trim()).Append("\" ");
        //    }
        //    foreach (string current in options.ReferencedAssemblies)
        //    {
        //        stringBuilder.Append("/R:");
        //        stringBuilder.Append("\"");
        //        stringBuilder.Append(current);
        //        stringBuilder.Append("\"");
        //        stringBuilder.Append(" ");
        //    }
        //    stringBuilder.Append("/out:");
        //    stringBuilder.Append("\"");
        //    stringBuilder.Append(options.OutputAssembly);
        //    stringBuilder.Append("\"");
        //    stringBuilder.Append(" ");
        //    if (options.IncludeDebugInformation)
        //    {
        //        stringBuilder.Append("/D:DEBUG ");
        //        stringBuilder.Append("/debug+ ");
        //        stringBuilder.Append("/optimize- ");
        //    }
        //    else
        //    {
        //        stringBuilder.Append("/debug- ");
        //        stringBuilder.Append("/optimize+ ");
        //    }
        //    if (options.Win32Resource != null)
        //    {
        //        stringBuilder.Append("/win32res:\"" + options.Win32Resource + "\" ");
        //    }
        //    foreach (string current2 in options.EmbeddedResources)
        //    {
        //        stringBuilder.Append("/res:\"");
        //        stringBuilder.Append(current2);
        //        stringBuilder.Append("\" ");
        //    }
        //    foreach (string current3 in options.LinkedResources)
        //    {
        //        stringBuilder.Append("/linkres:\"");
        //        stringBuilder.Append(current3);
        //        stringBuilder.Append("\" ");
        //    }
        //    if (options.TreatWarningsAsErrors)
        //    {
        //        stringBuilder.Append("/warnaserror ");
        //    }
        //    if (options.WarningLevel >= 0)
        //    {
        //        stringBuilder.Append("/w:" + options.WarningLevel + " ");
        //    }
        //    if (options.CompilerOptions != null)
        //    {
        //        stringBuilder.Append(options.CompilerOptions + " ");
        //    }
        //    return stringBuilder.ToString();
        //}

        private void ContinueOnNewLine(string st)
        {
            this.Output.WriteLine(st);
        }

        private string GetResponseFileCmdArgs(CompilerParameters options, string cmdArgs)
        {
            string text = options.TempFiles.AddExtension("cmdline");
            Stream stream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.Read);
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    streamWriter.Write(cmdArgs);
                    streamWriter.Flush();
                }
            }
            finally
            {
                stream.Close();
            }
            return "/noconfig /fullpaths @\"" + text + "\"";
        }

        private void OutputIdentifier(string ident)
        {
            this.Output.Write(this.CreateEscapedIdentifier(ident));
        }

        private void OutputIdentifierAsCamelCase(string ident)
        {
            ident = Regex.Replace(ident, this.regexPattern, i => i.Value.ToLower());
            this.Output.Write(this.CreateEscapedIdentifier(ident));
        }

        private void OutputType(CodeTypeReference typeRef)
        {
            this.Output.Write(this.GetTypeOutput(typeRef));
        }

        private void GenerateArrayCreateExpression(CodeArrayCreateExpression e)
        {
            this.Output.Write("new ");
            CodeExpressionCollection initializers = e.Initializers;
            if (initializers.Count > 0)
            {
                this.OutputType(e.CreateType);
                if (e.CreateType.ArrayRank == 0)
                {
                    this.Output.Write("[]");
                }
                this.Output.WriteLine(" {");
                int indent = this.Indent;
                this.Indent = indent + 1;
                this.OutputExpressionList(initializers, true);
                indent = this.Indent;
                this.Indent = indent - 1;
                this.Output.Write("}");
                return;
            }
            this.Output.Write(this.GetBaseTypeOutput(e.CreateType));
            this.Output.Write("[");
            if (e.SizeExpression != null)
            {
                this.GenerateExpression(e.SizeExpression);
            }
            else
            {
                this.Output.Write(e.Size);
            }
            this.Output.Write("]");
            int nestedArrayDepth = 1;// e.CreateType.NestedArrayDepth;
            for (int i = 0; i < nestedArrayDepth - 1; i++)
            {
                this.Output.Write("[]");
            }
        }

        private void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e)
        {
            this.Output.Write("super");
        }

        private void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e)
        {
            bool flag = false;
            this.Output.Write("(");
            this.GenerateExpression(e.Left);
            this.Output.Write(" ");
            if (e.Left is CodeBinaryOperatorExpression || e.Right is CodeBinaryOperatorExpression)
            {
                if (!this.inNestedBinary)
                {
                    flag = true;
                    this.inNestedBinary = true;
                    this.Indent += 3;
                }
                //this.ContinueOnNewLine("");
            }
            this.OutputOperator(e.Operator);
            this.Output.Write(" ");
            this.GenerateExpression(e.Right);
            this.Output.Write(")");
            if (flag)
            {
                this.Indent -= 3;
                this.inNestedBinary = false;
            }
        }

        private void GenerateCastExpression(CodeCastExpression e)
        {
            this.Output.Write("(<");
            this.OutputType(e.TargetType);
            this.Output.Write(">(");
            this.GenerateExpression(e.Expression);
            this.Output.Write("))");
        }

        public void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
        {
            if (this.output != null)
            {
                throw new InvalidOperationException("CodeGenReentrance");
            }
            this.options = ((options == null) ? new CodeGeneratorOptions() : options);
            this.output = new IndentedTextWriter(writer, this.options.IndentString);
            try
            {
                CodeTypeDeclaration declaredType = new CodeTypeDeclaration();
                this.currentClass = declaredType;
                this.GenerateTypeMember(member, declaredType);
            }
            finally
            {
                this.currentClass = null;
                this.output = null;
                this.options = null;
            }
        }

        private void GenerateDefaultValueExpression(CodeDefaultValueExpression e)
        {
            this.Output.Write("default(");
            this.OutputType(e.Type);
            this.Output.Write(")");
        }

        private void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e)
        {
            this.Output.Write("new ");
            this.OutputType(e.DelegateType);
            this.Output.Write("(");
            this.GenerateExpression(e.TargetObject);
            this.Output.Write(".");
            this.OutputIdentifier(e.MethodName);
            this.Output.Write(")");
        }

        private void GenerateEvents(CodeTypeDeclaration e)
        {
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeMemberEvent)
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeMemberEvent codeMemberEvent = (CodeMemberEvent)enumerator.Current;
                    if (codeMemberEvent.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(codeMemberEvent.LinePragma);
                    }
                    this.GenerateEvent(codeMemberEvent, e);
                    if (codeMemberEvent.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(codeMemberEvent.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                }
            }
        }

        private void GenerateFields(CodeTypeDeclaration e)
        {
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeMemberField)
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeMemberField codeMemberField = (CodeMemberField)enumerator.Current;
                    if (codeMemberField.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(codeMemberField.LinePragma);
                    }
                    this.GenerateField(codeMemberField);
                    if (codeMemberField.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(codeMemberField.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                }
            }
        }

        private void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                this.GenerateExpression(e.TargetObject);
                this.Output.Write(".");
            }
            this.OutputIdentifierAsCamelCase(e.FieldName);
        }

        private void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e)
        {
            this.OutputIdentifierAsCamelCase(e.ParameterName);
        }

        private void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e)
        {
            this.OutputIdentifierAsCamelCase(e.VariableName);
        }

        private void GenerateIndexerExpression(CodeIndexerExpression e)
        {
            this.GenerateExpression(e.TargetObject);
            this.Output.Write("[");
            bool flag = true;
            foreach (CodeExpression e2 in e.Indices)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    this.Output.Write(", ");
                }
                this.GenerateExpression(e2);
            }
            this.Output.Write("]");
        }

        private void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e)
        {
            this.GenerateExpression(e.TargetObject);
            this.Output.Write("[");
            bool flag = true;
            foreach (CodeExpression e2 in e.Indices)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    this.Output.Write(", ");
                }
                this.GenerateExpression(e2);
            }
            this.Output.Write("]");
        }

        private void GenerateSnippetCompileUnit(CodeSnippetCompileUnit e)
        {
            this.GenerateDirectives(e.StartDirectives);
            if (e.LinePragma != null)
            {
                this.GenerateLinePragmaStart(e.LinePragma);
            }
            this.Output.WriteLine(e.Value);
            if (e.LinePragma != null)
            {
                this.GenerateLinePragmaEnd(e.LinePragma);
            }
            if (e.EndDirectives.Count > 0)
            {
                this.GenerateDirectives(e.EndDirectives);
            }
        }

        private void GenerateSnippetExpression(CodeSnippetExpression e)
        {
            this.Output.Write(e.Value);
        }

        private void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e)
        {
            this.GenerateMethodReferenceExpression(e.Method);
            this.Output.Write("(");
            this.OutputExpressionList(e.Parameters);
            this.Output.Write(")");
        }

        private void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                if (e.TargetObject is CodeBinaryOperatorExpression)
                {
                    this.Output.Write("(");
                    this.GenerateExpression(e.TargetObject);
                    this.Output.Write(")");
                }
                else
                {
                    this.GenerateExpression(e.TargetObject);
                }
                this.Output.Write(".");
            }
            this.OutputIdentifierAsCamelCase(e.MethodName);
            if (e.TypeArguments.Count > 0)
            {
                this.Output.Write(this.GetTypeArgumentsOutput(e.TypeArguments));
            }
        }

        private bool GetUserData(CodeObject e, string property, bool defaultValue)
        {
            object obj = e.UserData[property];
            if (obj != null && obj is bool)
            {
                return (bool)obj;
            }
            return defaultValue;
        }

        private void GenerateNamespace(CodeNamespace e)
        {
            this.GenerateCommentStatements(e.Comments);
            this.GenerateNamespaceStart(e);
            if (this.GetUserData(e, "GenerateImports", true))
            {
                this.GenerateNamespaceImports(e);
            }
            this.Output.WriteLine("");
            this.GenerateTypes(e);
            this.GenerateNamespaceEnd(e);
        }

        private void GenerateStatement(CodeStatement e)
        {
            if (e.StartDirectives.Count > 0)
            {
                this.GenerateDirectives(e.StartDirectives);
            }
            if (e.LinePragma != null)
            {
                this.GenerateLinePragmaStart(e.LinePragma);
            }
            if (e is CodeCommentStatement)
            {
                this.GenerateCommentStatement((CodeCommentStatement)e);
            }
            else if (e is CodeMethodReturnStatement)
            {
                this.GenerateMethodReturnStatement((CodeMethodReturnStatement)e);
            }
            else if (e is CodeConditionStatement)
            {
                this.GenerateConditionStatement((CodeConditionStatement)e);
            }
            else if (e is CodeTryCatchFinallyStatement)
            {
                this.GenerateTryCatchFinallyStatement((CodeTryCatchFinallyStatement)e);
            }
            else if (e is CodeAssignStatement)
            {
                this.GenerateAssignStatement((CodeAssignStatement)e);
            }
            else if (e is CodeExpressionStatement)
            {
                this.GenerateExpressionStatement((CodeExpressionStatement)e);
            }
            else if (e is CodeIterationStatement)
            {
                this.GenerateIterationStatement((CodeIterationStatement)e);
            }
            else if (e is CodeThrowExceptionStatement)
            {
                this.GenerateThrowExceptionStatement((CodeThrowExceptionStatement)e);
            }
            else if (e is CodeSnippetStatement)
            {
                int indent = this.Indent;
                this.Indent = 0;
                this.GenerateSnippetStatement((CodeSnippetStatement)e);
                this.Indent = indent;
            }
            else if (e is CodeVariableDeclarationStatement)
            {
                this.GenerateVariableDeclarationStatement((CodeVariableDeclarationStatement)e);
            }
            else if (e is CodeAttachEventStatement)
            {
                this.GenerateAttachEventStatement((CodeAttachEventStatement)e);
            }
            else if (e is CodeRemoveEventStatement)
            {
                this.GenerateRemoveEventStatement((CodeRemoveEventStatement)e);
            }
            else if (e is CodeGotoStatement)
            {
                this.GenerateGotoStatement((CodeGotoStatement)e);
            }
            else
            {
                if (!(e is CodeLabeledStatement))
                {
                    throw new ArgumentException("InvalidElementType");
                }
                this.GenerateLabeledStatement((CodeLabeledStatement)e);
            }
            if (e.LinePragma != null)
            {
                this.GenerateLinePragmaEnd(e.LinePragma);
            }
            if (e.EndDirectives.Count > 0)
            {
                this.GenerateDirectives(e.EndDirectives);
            }
        }

        private void GenerateStatements(CodeStatementCollection stms)
        {
            IEnumerator enumerator = stms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromStatement((CodeStatement)enumerator.Current, this.output.InnerWriter, this.options);
            }
        }

        private void GenerateNamespaceImports(CodeNamespace e)
        {
            IEnumerator enumerator = e.Imports.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)enumerator.Current;
                if (codeNamespaceImport.LinePragma != null)
                {
                    this.GenerateLinePragmaStart(codeNamespaceImport.LinePragma);
                }
                this.GenerateNamespaceImport(codeNamespaceImport);
                if (codeNamespaceImport.LinePragma != null)
                {
                    this.GenerateLinePragmaEnd(codeNamespaceImport.LinePragma);
                }
            }
        }

        private void GenerateEventReferenceExpression(CodeEventReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                this.GenerateExpression(e.TargetObject);
                this.Output.Write(".");
            }
            this.OutputIdentifier(e.EventName);
        }

        private void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e)
        {
            if (e.TargetObject != null)
            {
                this.GenerateExpression(e.TargetObject);
            }
            this.Output.Write("(");
            this.OutputExpressionList(e.Parameters);
            this.Output.Write(")");
        }

        private void GenerateObjectCreateExpression(CodeObjectCreateExpression e)
        {
            this.Output.Write("new ");
            this.OutputType(e.CreateType);
            this.Output.Write("(");
            this.OutputExpressionList(e.Parameters);
            this.Output.Write(")");
        }

        private void GeneratePrimitiveExpression(CodePrimitiveExpression e)
        {
            if (e.Value is char)
            {
                this.GeneratePrimitiveChar((char)e.Value);
                return;
            }
            if (e.Value is sbyte)
            {
                this.Output.Write(((sbyte)e.Value).ToString(CultureInfo.InvariantCulture));
                return;
            }
            if (e.Value is ushort)
            {
                this.Output.Write(((ushort)e.Value).ToString(CultureInfo.InvariantCulture));
                return;
            }
            if (e.Value is uint)
            {
                this.Output.Write(((uint)e.Value).ToString(CultureInfo.InvariantCulture));
                this.Output.Write("u");
                return;
            }
            if (e.Value is ulong)
            {
                this.Output.Write(((ulong)e.Value).ToString(CultureInfo.InvariantCulture));
                this.Output.Write("ul");
                return;
            }
            this.GeneratePrimitiveExpressionBase(e);
        }

        private void GeneratePrimitiveExpressionBase(CodePrimitiveExpression e)
        {
            if (e.Value == null)
            {
                this.Output.Write(this.NullToken);
                return;
            }
            if (e.Value is string)
            {
                this.Output.Write(this.QuoteSnippetString((string)e.Value));
                return;
            }
            if (e.Value is char)
            {
                this.Output.Write("'" + e.Value.ToString() + "'");
                return;
            }
            if (e.Value is byte)
            {
                this.Output.Write(((byte)e.Value).ToString(CultureInfo.InvariantCulture));
                return;
            }
            if (e.Value is short)
            {
                this.Output.Write(((short)e.Value).ToString(CultureInfo.InvariantCulture));
                return;
            }
            if (e.Value is int)
            {
                this.Output.Write(((int)e.Value).ToString(CultureInfo.InvariantCulture));
                return;
            }
            if (e.Value is long)
            {
                this.Output.Write(((long)e.Value).ToString(CultureInfo.InvariantCulture));
                return;
            }
            if (e.Value is float)
            {
                this.GenerateSingleFloatValue((float)e.Value);
                return;
            }
            if (e.Value is double)
            {
                this.GenerateDoubleValue((double)e.Value);
                return;
            }
            if (e.Value is decimal)
            {
                this.GenerateDecimalValue((decimal)e.Value);
                return;
            }
            if (!(e.Value is bool))
            {
                throw new ArgumentException("InvalidPrimitiveType");
            }
            if ((bool)e.Value)
            {
                this.Output.Write("true");
                return;
            }
            this.Output.Write("false");
        }

        private void GeneratePrimitiveChar(char c)
        {
            this.Output.Write('\'');
            if (c > '\'')
            {
                if (c <= '\u0084')
                {
                    if (c == '\\')
                    {
                        this.Output.Write("\\\\");
                        goto IL_143;
                    }
                    if (c != '\u0084')
                    {
                        goto IL_125;
                    }
                }
                else if (c != '\u0085' && c != '\u2028' && c != '\u2029')
                {
                    goto IL_125;
                }
                this.AppendEscapedChar(null, c);
                goto IL_143;
            }
            if (c <= '\r')
            {
                if (c == '\0')
                {
                    this.Output.Write("\\0");
                    goto IL_143;
                }
                switch (c)
                {
                    case '\t':
                        this.Output.Write("\\t");
                        goto IL_143;
                    case '\n':
                        this.Output.Write("\\n");
                        goto IL_143;
                    case '\r':
                        this.Output.Write("\\r");
                        goto IL_143;
                }
            }
            else
            {
                if (c == '"')
                {
                    this.Output.Write("\\\"");
                    goto IL_143;
                }
                if (c == '\'')
                {
                    this.Output.Write("\\'");
                    goto IL_143;
                }
            }
        IL_125:
            if (char.IsSurrogate(c))
            {
                this.AppendEscapedChar(null, c);
            }
            else
            {
                this.Output.Write(c);
            }
        IL_143:
            this.Output.Write('\'');
        }

        private void AppendEscapedChar(StringBuilder b, char value)
        {
            int num;
            if (b == null)
            {
                this.Output.Write("\\u");
                TextWriter arg_2C_0 = this.Output;
                num = (int)value;
                arg_2C_0.Write(num.ToString("X4", CultureInfo.InvariantCulture));
                return;
            }
            b.Append("\\u");
            num = (int)value;
            b.Append(num.ToString("X4", CultureInfo.InvariantCulture));
        }

        private void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e)
        {
            this.Output.Write("value");
        }

        private void GenerateThisReferenceExpression(CodeThisReferenceExpression e)
        {
            this.Output.Write("this");
        }

        private void GenerateExpressionStatement(CodeExpressionStatement e)
        {
            this.GenerateExpression(e.Expression);
            if (!this.generatingForLoop)
            {
                this.Output.WriteLine(";");
            }
        }

        private void GenerateIterationStatement(CodeIterationStatement e)
        {
            this.generatingForLoop = true;
            this.Output.Write("for (");
            this.GenerateStatement(e.InitStatement);
            this.Output.Write("; ");
            this.GenerateExpression(e.TestExpression);
            this.Output.Write("; ");
            this.GenerateStatement(e.IncrementStatement);
            this.Output.Write(")");
            this.OutputStartingBrace();
            this.generatingForLoop = false;
            int indent = this.Indent;
            this.Indent = indent + 1;
            this.GenerateStatements(e.Statements);
            indent = this.Indent;
            this.Indent = indent - 1;
            this.Output.WriteLine("}");
        }

        private void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e)
        {
            this.Output.Write("throw");
            if (e.ToThrow != null)
            {
                this.Output.Write(" ");
                this.GenerateExpression(e.ToThrow);
            }
            this.Output.WriteLine(";");
        }

        private void GenerateComment(CodeComment e)
        {
            string value = e.DocComment ? "///" : "//";
            this.Output.Write(value);
            this.Output.Write(" ");
            string text = e.Text;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '\0')
                {
                    this.Output.Write(text[i]);
                    if (text[i] == '\r')
                    {
                        if (i < text.Length - 1 && text[i + 1] == '\n')
                        {
                            this.Output.Write('\n');
                            i++;
                        }
                        ((IndentedTextWriter)this.Output).InternalOutputTabs();
                        this.Output.Write(value);
                    }
                    else if (text[i] == '\n')
                    {
                        ((IndentedTextWriter)this.Output).InternalOutputTabs();
                        this.Output.Write(value);
                    }
                    else if (text[i] == '\u2028' || text[i] == '\u2029' || text[i] == '\u0085')
                    {
                        this.Output.Write(value);
                    }
                }
            }
            this.Output.WriteLine();
        }

        private void GenerateCommentStatement(CodeCommentStatement e)
        {
            if (e.Comment == null)
            {
                throw new ArgumentException("Argument_NullComment");
            }
            this.GenerateComment(e.Comment);
        }

        private void GenerateCommentStatements(CodeCommentStatementCollection e)
        {
            foreach (CodeCommentStatement e2 in e)
            {
                this.GenerateCommentStatement(e2);
            }
        }

        private void GenerateMethodReturnStatement(CodeMethodReturnStatement e)
        {
            this.Output.Write("return");
            if (e.Expression != null)
            {
                this.Output.Write(" ");
                this.GenerateExpression(e.Expression);
            }
            this.Output.WriteLine(";");
        }

        private void GenerateConditionStatement(CodeConditionStatement e)
        {
            this.Output.Write("if (");
            this.GenerateExpression(e.Condition);
            this.Output.Write(")");
            this.OutputStartingBrace();
            int indent = this.Indent;
            this.Indent = indent + 1;
            this.GenerateStatements(e.TrueStatements);
            indent = this.Indent;
            this.Indent = indent - 1;
            if (e.FalseStatements.Count > 0)
            {
                this.Output.Write("}");
                if (this.Options.ElseOnClosing)
                {
                    this.Output.Write(" ");
                }
                else
                {
                    this.Output.WriteLine("");
                }
                this.Output.Write("else");
                this.OutputStartingBrace();
                indent = this.Indent;
                this.Indent = indent + 1;
                this.GenerateStatements(e.FalseStatements);
                indent = this.Indent;
                this.Indent = indent - 1;
            }
            this.Output.WriteLine("}");
        }

        private void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e)
        {
            this.Output.Write("try");
            this.OutputStartingBrace();
            int indent = this.Indent;
            this.Indent = indent + 1;
            this.GenerateStatements(e.TryStatements);
            indent = this.Indent;
            this.Indent = indent - 1;
            CodeCatchClauseCollection catchClauses = e.CatchClauses;
            if (catchClauses.Count > 0)
            {
                IEnumerator enumerator = catchClauses.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    this.Output.Write("}");
                    if (this.Options.ElseOnClosing)
                    {
                        this.Output.Write(" ");
                    }
                    else
                    {
                        this.Output.WriteLine("");
                    }
                    CodeCatchClause codeCatchClause = (CodeCatchClause)enumerator.Current;
                    this.Output.Write("catch (");
                    //this.OutputType(codeCatchClause.CatchExceptionType);
                    //this.Output.Write(" ");
                    this.OutputIdentifier(codeCatchClause.LocalName);
                    this.Output.Write(")");
                    this.OutputStartingBrace();
                    indent = this.Indent;
                    this.Indent = indent + 1;
                    this.GenerateStatements(codeCatchClause.Statements);
                    indent = this.Indent;
                    this.Indent = indent - 1;
                }
            }
            CodeStatementCollection finallyStatements = e.FinallyStatements;
            if (finallyStatements.Count > 0)
            {
                this.Output.Write("}");
                if (this.Options.ElseOnClosing)
                {
                    this.Output.Write(" ");
                }
                else
                {
                    this.Output.WriteLine("");
                }
                this.Output.Write("finally");
                this.OutputStartingBrace();
                indent = this.Indent;
                this.Indent = indent + 1;
                this.GenerateStatements(finallyStatements);
                indent = this.Indent;
                this.Indent = indent - 1;
            }
            this.Output.WriteLine("}");
        }

        private void GenerateAssignStatement(CodeAssignStatement e)
        {
            this.GenerateExpression(e.Left);
            this.Output.Write(" = ");
            this.GenerateExpression(e.Right);
            if (!this.generatingForLoop)
            {
                this.Output.WriteLine(";");
            }
        }

        private void GenerateAttachEventStatement(CodeAttachEventStatement e)
        {
            this.GenerateEventReferenceExpression(e.Event);
            this.Output.Write(" += ");
            this.GenerateExpression(e.Listener);
            this.Output.WriteLine(";");
        }

        private void GenerateRemoveEventStatement(CodeRemoveEventStatement e)
        {
            this.GenerateEventReferenceExpression(e.Event);
            this.Output.Write(" -= ");
            this.GenerateExpression(e.Listener);
            this.Output.WriteLine(";");
        }

        private void GenerateSnippetStatement(CodeSnippetStatement e)
        {
            this.Output.WriteLine(e.Value);
        }

        private void GenerateGotoStatement(CodeGotoStatement e)
        {
            this.Output.Write("goto ");
            this.Output.Write(e.Label);
            this.Output.WriteLine(";");
        }

        private void GenerateLabeledStatement(CodeLabeledStatement e)
        {
            int indent = this.Indent;
            this.Indent = indent - 1;
            this.Output.Write(e.Label);
            this.Output.WriteLine(":");
            indent = this.Indent;
            this.Indent = indent + 1;
            if (e.Statement != null)
            {
                this.GenerateStatement(e.Statement);
            }
        }

        private void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e)
        {
            //this.OutputTypeNamePair(e.Type, e.Name);
            this.Output.Write("let ");
            this.OutputIdentifierAsCamelCase(e.Name);
            this.Output.Write(": ");
            this.OutputType(e.Type);
            if (e.InitExpression != null)
            {
                this.Output.Write(" = ");
                this.GenerateExpression(e.InitExpression);
            }
            if (!this.generatingForLoop)
            {
                this.Output.WriteLine(";");
            }
        }

        private void GenerateLinePragmaStart(CodeLinePragma e)
        {
            this.Output.WriteLine("");
            this.Output.Write("#line ");
            this.Output.Write(e.LineNumber);
            this.Output.Write(" \"");
            this.Output.Write(e.FileName);
            this.Output.Write("\"");
            this.Output.WriteLine("");
        }

        private void GenerateLinePragmaEnd(CodeLinePragma e)
        {
            this.Output.WriteLine();
            this.Output.WriteLine("#line default");
            this.Output.WriteLine("#line hidden");
        }

        private void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c)
        {
            if (this.IsCurrentDelegate || this.IsCurrentEnum)
            {
                return;
            }
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes);
            }
            if (e.PrivateImplementationType == null)
            {
                this.OutputMemberAccessModifier(e.Attributes);
            }
            this.Output.Write("event ");
            string text = e.Name;
            if (e.PrivateImplementationType != null)
            {
                text = this.GetBaseTypeOutput(e.PrivateImplementationType) + "." + text;
            }
            this.OutputTypeNamePair(e.Type, text);
            this.Output.WriteLine(";");
        }

        private void GenerateExpression(CodeExpression e)
        {
            if (e is CodeArrayCreateExpression)
            {
                this.GenerateArrayCreateExpression((CodeArrayCreateExpression)e);
                return;
            }
            if (e is CodeBaseReferenceExpression)
            {
                this.GenerateBaseReferenceExpression((CodeBaseReferenceExpression)e);
                return;
            }
            if (e is CodeBinaryOperatorExpression)
            {
                this.GenerateBinaryOperatorExpression((CodeBinaryOperatorExpression)e);
                return;
            }
            if (e is CodeCastExpression)
            {
                this.GenerateCastExpression((CodeCastExpression)e);
                return;
            }
            if (e is CodeDelegateCreateExpression)
            {
                this.GenerateDelegateCreateExpression((CodeDelegateCreateExpression)e);
                return;
            }
            if (e is CodeFieldReferenceExpression)
            {
                this.GenerateFieldReferenceExpression((CodeFieldReferenceExpression)e);
                return;
            }
            if (e is CodeArgumentReferenceExpression)
            {
                this.GenerateArgumentReferenceExpression((CodeArgumentReferenceExpression)e);
                return;
            }
            if (e is CodeVariableReferenceExpression)
            {
                this.GenerateVariableReferenceExpression((CodeVariableReferenceExpression)e);
                return;
            }
            if (e is CodeIndexerExpression)
            {
                this.GenerateIndexerExpression((CodeIndexerExpression)e);
                return;
            }
            if (e is CodeArrayIndexerExpression)
            {
                this.GenerateArrayIndexerExpression((CodeArrayIndexerExpression)e);
                return;
            }
            if (e is CodeSnippetExpression)
            {
                this.GenerateSnippetExpression((CodeSnippetExpression)e);
                return;
            }
            if (e is CodeMethodInvokeExpression)
            {
                this.GenerateMethodInvokeExpression((CodeMethodInvokeExpression)e);
                return;
            }
            if (e is CodeMethodReferenceExpression)
            {
                this.GenerateMethodReferenceExpression((CodeMethodReferenceExpression)e);
                return;
            }
            if (e is CodeEventReferenceExpression)
            {
                this.GenerateEventReferenceExpression((CodeEventReferenceExpression)e);
                return;
            }
            if (e is CodeDelegateInvokeExpression)
            {
                this.GenerateDelegateInvokeExpression((CodeDelegateInvokeExpression)e);
                return;
            }
            if (e is CodeObjectCreateExpression)
            {
                this.GenerateObjectCreateExpression((CodeObjectCreateExpression)e);
                return;
            }
            if (e is CodeParameterDeclarationExpression)
            {
                this.GenerateParameterDeclarationExpression((CodeParameterDeclarationExpression)e);
                return;
            }
            if (e is CodeDirectionExpression)
            {
                this.GenerateDirectionExpression((CodeDirectionExpression)e);
                return;
            }
            if (e is CodePrimitiveExpression)
            {
                this.GeneratePrimitiveExpression((CodePrimitiveExpression)e);
                return;
            }
            if (e is CodePropertyReferenceExpression)
            {
                this.GeneratePropertyReferenceExpression((CodePropertyReferenceExpression)e);
                return;
            }
            if (e is CodePropertySetValueReferenceExpression)
            {
                this.GeneratePropertySetValueReferenceExpression((CodePropertySetValueReferenceExpression)e);
                return;
            }
            if (e is CodeThisReferenceExpression)
            {
                this.GenerateThisReferenceExpression((CodeThisReferenceExpression)e);
                return;
            }
            if (e is CodeTypeReferenceExpression)
            {
                this.GenerateTypeReferenceExpression((CodeTypeReferenceExpression)e);
                return;
            }
            if (e is CodeTypeOfExpression)
            {
                this.GenerateTypeOfExpression((CodeTypeOfExpression)e);
                return;
            }
            if (e is CodeDefaultValueExpression)
            {
                this.GenerateDefaultValueExpression((CodeDefaultValueExpression)e);
                return;
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            throw new ArgumentException("InvalidElementType");
        }

        private void GenerateField(CodeMemberField e)
        {
            if (this.IsCurrentDelegate || this.IsCurrentInterface)
            {
                return;
            }
            if (this.IsCurrentEnum)
            {
                if (e.CustomAttributes.Count > 0)
                {
                    this.GenerateAttributes(e.CustomAttributes);
                }
                this.OutputIdentifier(e.Name);
                if (e.InitExpression != null)
                {
                    this.Output.Write(" = ");
                    this.GenerateExpression(e.InitExpression);
                }
                this.Output.WriteLine(",");
                return;
            }
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes);
            }
            this.OutputMemberAccessModifier(e.Attributes);
            this.OutputVTableModifier(e.Attributes);
            this.OutputFieldScopeModifier(e.Attributes);
            this.OutputTypeNamePair(e.Type, e.Name);
            if (e.InitExpression != null)
            {
                this.Output.Write(" = ");
                this.GenerateExpression(e.InitExpression);
            }
            this.Output.WriteLine(";");
        }

        private void GenerateSnippetMember(CodeSnippetTypeMember e)
        {
            this.Output.Write(e.Text);
        }

        private void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e)
        {
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes, null, true);
            }
            this.OutputDirection(e.Direction);
            this.OutputTypeNamePair(e.Type, e.Name);
        }

        private void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c)
        {
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes);
            }
            this.Output.Write("public static ");
            this.OutputType(e.ReturnType);
            this.Output.Write(" Main()");
            this.OutputStartingBrace();
            int indent = this.Indent;
            this.Indent = indent + 1;
            this.GenerateStatements(e.Statements);
            indent = this.Indent;
            this.Indent = indent - 1;
            this.Output.WriteLine("}");
        }

        private void GenerateMethods(CodeTypeDeclaration e)
        {
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeMemberMethod && !(enumerator.Current is CodeTypeConstructor) && !(enumerator.Current is CodeConstructor))
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeMemberMethod codeMemberMethod = (CodeMemberMethod)enumerator.Current;
                    if (codeMemberMethod.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(codeMemberMethod.LinePragma);
                    }
                    if (enumerator.Current is CodeEntryPointMethod)
                    {
                        this.GenerateEntryPointMethod((CodeEntryPointMethod)enumerator.Current, e);
                    }
                    else
                    {
                        this.GenerateMethod(codeMemberMethod, e);
                    }
                    if (codeMemberMethod.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(codeMemberMethod.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                }
            }
        }

        private void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c)
        {
            if (!this.IsCurrentClass && !this.IsCurrentStruct && !this.IsCurrentInterface)
            {
                return;
            }
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes);
            }
            if (e.ReturnTypeCustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.ReturnTypeCustomAttributes, "return: ");
            }
            if (!this.IsCurrentInterface)
            {
                if (e.PrivateImplementationType == null)
                {
                    this.OutputMemberAccessModifier(e.Attributes);
                    this.OutputVTableModifier(e.Attributes);
                    this.OutputMemberScopeModifier(e.Attributes);
                }
            }
            else
            {
                this.OutputVTableModifier(e.Attributes);
            }

            //this.Output.Write(" ");
            if (e.PrivateImplementationType != null)
            {
                this.Output.Write(this.GetBaseTypeOutput(e.PrivateImplementationType));
                this.Output.Write(".");
            }

            this.OutputIdentifierAsCamelCase(e.Name);
            this.OutputTypeParameters(e.TypeParameters);
            this.Output.Write("(");
            this.OutputParameters(e.Parameters);
            this.Output.Write("): ");
            this.OutputType(e.ReturnType);
            this.OutputTypeParameterConstraints(e.TypeParameters);
            if (!this.IsCurrentInterface && (e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract)
            {
                if (e.Statements.Count > 0)
                {
                    this.OutputStartingBrace();
                    int indent = this.Indent;
                    this.Indent = indent + 1;
                    this.GenerateStatements(e.Statements);
                    indent = this.Indent;
                    this.Indent = indent - 1;
                    this.Output.WriteLine("}");
                    return;
                }
            }
            this.Output.WriteLine(";");
        }

        private void GenerateProperties(CodeTypeDeclaration e)
        {
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeMemberProperty)
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeMemberProperty codeMemberProperty = (CodeMemberProperty)enumerator.Current;
                    if (codeMemberProperty.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(codeMemberProperty.LinePragma);
                    }
                    this.GenerateProperty(codeMemberProperty, e);
                    if (codeMemberProperty.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(codeMemberProperty.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                }
            }
        }

        private void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c)
        {
            if (!this.IsCurrentClass && !this.IsCurrentStruct && !this.IsCurrentInterface)
            {
                return;
            }
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes);
            }

            if (!this.IsCurrentInterface)
            {
                if (e.PrivateImplementationType == null)
                {
                    this.OutputMemberAccessModifier(e.Attributes);
                    this.OutputVTableModifier(e.Attributes);
                    this.OutputMemberScopeModifier(e.Attributes);
                }
            }
            else
            {
                this.OutputVTableModifier(e.Attributes);
            }
            this.Output.Write("get");
            this.Output.Write(" ");
            if (e.PrivateImplementationType != null && !this.IsCurrentInterface)
            {
                this.Output.Write(this.GetBaseTypeOutput(e.PrivateImplementationType));
                this.Output.Write(".");
            }
            if (e.Parameters.Count > 0 && string.Compare(e.Name, "Item", StringComparison.OrdinalIgnoreCase) == 0)
            {
                this.Output.Write("this[");
                this.OutputParameters(e.Parameters);
                this.Output.Write("]");
            }
            else
            {
                this.OutputIdentifierAsCamelCase(e.Name);
            }

            this.Output.Write("(): ");
            this.OutputType(e.Type);
            this.OutputStartingBrace();
            int indent = this.Indent;
            this.Indent = indent + 1;
            if (e.HasGet)
            {
                if (this.IsCurrentInterface || (e.Attributes & MemberAttributes.ScopeMask) == MemberAttributes.Abstract)
                {
                    this.Output.WriteLine("get;");
                }
                else
                {
                    //this.Output.Write("get");
                    //this.OutputStartingBrace();
                    //indent = this.Indent;
                    //this.Indent = indent + 1;
                    this.GenerateStatements(e.GetStatements);
                    //indent = this.Indent;
                    //this.Indent = indent - 1;
                    //this.Output.WriteLine("}");
                }
            }
            if (e.HasSet)
            {
                if (this.IsCurrentInterface || (e.Attributes & MemberAttributes.ScopeMask) == MemberAttributes.Abstract)
                {
                    this.Output.WriteLine("set;");
                }
                else
                {
                    this.Output.Write("set");
                    this.OutputStartingBrace();
                    indent = this.Indent;
                    this.Indent = indent + 1;
                    this.GenerateStatements(e.SetStatements);
                    indent = this.Indent;
                    this.Indent = indent - 1;
                    this.Output.WriteLine("}");
                }
            }
            indent = this.Indent;
            this.Indent = indent - 1;
            this.Output.WriteLine("}");
        }

        private void GenerateSingleFloatValue(float s)
        {
            if (float.IsNaN(s))
            {
                this.Output.Write("float.NaN");
                return;
            }
            if (float.IsNegativeInfinity(s))
            {
                this.Output.Write("float.NegativeInfinity");
                return;
            }
            if (float.IsPositiveInfinity(s))
            {
                this.Output.Write("float.PositiveInfinity");
                return;
            }
            this.Output.Write(s.ToString(CultureInfo.InvariantCulture));
            this.Output.Write('F');
        }

        private void GenerateDoubleValue(double d)
        {
            if (double.IsNaN(d))
            {
                this.Output.Write("double.NaN");
                return;
            }
            if (double.IsNegativeInfinity(d))
            {
                this.Output.Write("double.NegativeInfinity");
                return;
            }
            if (double.IsPositiveInfinity(d))
            {
                this.Output.Write("double.PositiveInfinity");
                return;
            }
            this.Output.Write(d.ToString("R", CultureInfo.InvariantCulture));
            this.Output.Write("D");
        }

        private void GenerateDecimalValue(decimal d)
        {
            this.Output.Write(d.ToString(CultureInfo.InvariantCulture));
            this.Output.Write('m');
        }

        private void OutputVTableModifier(MemberAttributes attributes)
        {
            MemberAttributes memberAttributes = attributes & MemberAttributes.VTableMask;
            if (memberAttributes == MemberAttributes.New)
            {
                this.Output.Write("new ");
            }
        }

        private void OutputMemberAccessModifier(MemberAttributes attributes)
        {
            MemberAttributes memberAttributes = attributes & MemberAttributes.AccessMask;
            if (memberAttributes <= MemberAttributes.Family)
            {
                if (memberAttributes == MemberAttributes.Assembly)
                {
                    //this.Output.Write("internal ");
                    return;
                }
                if (memberAttributes == MemberAttributes.FamilyAndAssembly)
                {
                    //this.Output.Write("internal ");
                    return;
                }
                if (memberAttributes != MemberAttributes.Family)
                {
                    return;
                }
                this.Output.Write("protected ");
                return;
            }
            else
            {
                if (memberAttributes == MemberAttributes.FamilyOrAssembly)
                {
                    this.Output.Write("protected internal ");
                    return;
                }
                if (memberAttributes == MemberAttributes.Private)
                {
                    this.Output.Write("private ");
                    return;
                }
                if (memberAttributes != MemberAttributes.Public)
                {
                    return;
                }
                this.Output.Write("public ");
                return;
            }
        }

        private void OutputMemberScopeModifier(MemberAttributes attributes)
        {
            switch (attributes & MemberAttributes.ScopeMask)
            {
                case MemberAttributes.Abstract:
                    this.Output.Write("abstract ");
                    return;
                case MemberAttributes.Final:
                    this.Output.Write("");
                    return;
                case MemberAttributes.Static:
                    this.Output.Write("static ");
                    return;
                case MemberAttributes.Override:
                    //this.Output.Write("override ");
                    return;
                default:
                    {
                        MemberAttributes memberAttributes = attributes & MemberAttributes.AccessMask;
                        if (memberAttributes == MemberAttributes.Assembly || memberAttributes == MemberAttributes.Family || memberAttributes == MemberAttributes.Public)
                        {
                            //this.Output.Write("virtual ");
                        }
                        return;
                    }
            }
        }

        private void OutputOperator(CodeBinaryOperatorType op)
        {
            switch (op)
            {
                case CodeBinaryOperatorType.Add:
                    this.Output.Write("+");
                    return;
                case CodeBinaryOperatorType.Subtract:
                    this.Output.Write("-");
                    return;
                case CodeBinaryOperatorType.Multiply:
                    this.Output.Write("*");
                    return;
                case CodeBinaryOperatorType.Divide:
                    this.Output.Write("/");
                    return;
                case CodeBinaryOperatorType.Modulus:
                    this.Output.Write("%");
                    return;
                case CodeBinaryOperatorType.Assign:
                    this.Output.Write("=");
                    return;
                case CodeBinaryOperatorType.IdentityInequality:
                    this.Output.Write("!==");
                    return;
                case CodeBinaryOperatorType.IdentityEquality:
                    this.Output.Write("===");
                    return;
                case CodeBinaryOperatorType.ValueEquality:
                    this.Output.Write("==");
                    return;
                case CodeBinaryOperatorType.BitwiseOr:
                    this.Output.Write("|");
                    return;
                case CodeBinaryOperatorType.BitwiseAnd:
                    this.Output.Write("&");
                    return;
                case CodeBinaryOperatorType.BooleanOr:
                    this.Output.Write("||");
                    return;
                case CodeBinaryOperatorType.BooleanAnd:
                    this.Output.Write("&&");
                    return;
                case CodeBinaryOperatorType.LessThan:
                    this.Output.Write("<");
                    return;
                case CodeBinaryOperatorType.LessThanOrEqual:
                    this.Output.Write("<=");
                    return;
                case CodeBinaryOperatorType.GreaterThan:
                    this.Output.Write(">");
                    return;
                case CodeBinaryOperatorType.GreaterThanOrEqual:
                    this.Output.Write(">=");
                    return;
                default:
                    return;
            }
        }

        private void OutputFieldScopeModifier(MemberAttributes attributes)
        {
            switch (attributes & MemberAttributes.ScopeMask)
            {
                case MemberAttributes.Final:
                case MemberAttributes.Override:
                    break;
                case MemberAttributes.Static:
                    this.Output.Write("static ");
                    return;
                case MemberAttributes.Const:
                    this.Output.Write("const ");
                    break;
                default:
                    return;
            }
        }

        private void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                this.GenerateExpression(e.TargetObject);
                this.Output.Write(".");
            }
            this.OutputIdentifier(e.PropertyName);
        }

        private void GenerateConstructors(CodeTypeDeclaration e)
        {
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeConstructor)
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeConstructor codeConstructor = (CodeConstructor)enumerator.Current;
                    if (codeConstructor.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(codeConstructor.LinePragma);
                    }
                    this.GenerateConstructor(codeConstructor, e);
                    if (codeConstructor.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(codeConstructor.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                }
            }
        }

        private void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c)
        {
            if (!this.IsCurrentClass && !this.IsCurrentStruct)
            {
                return;
            }
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes);
            }
            this.OutputMemberAccessModifier(e.Attributes);
            this.OutputIdentifier("constructor");
            this.Output.Write("(");
            this.OutputParameters(e.Parameters);
            this.Output.Write(")");
            CodeExpressionCollection baseConstructorArgs = e.BaseConstructorArgs;
            CodeExpressionCollection chainedConstructorArgs = e.ChainedConstructorArgs;
            int indent;
            this.OutputStartingBrace();
            //if (baseConstructorArgs.Count > 0)
            if (c.BaseTypes.Count > 0)
            {
                indent = this.Indent;
                this.Indent = indent + 1;
                this.Output.Write("super(");
                this.OutputExpressionList(baseConstructorArgs);
                this.Output.WriteLine(");");
                indent = this.Indent;
                this.Indent = indent - 1;
                //indent = this.Indent;
                //this.Indent = indent - 1;
            }
            if (chainedConstructorArgs.Count > 0)
            {
                this.Output.WriteLine(" : ");
                indent = this.Indent;
                this.Indent = indent + 1;
                indent = this.Indent;
                this.Indent = indent + 1;
                this.Output.Write("this(");
                this.OutputExpressionList(chainedConstructorArgs);
                this.Output.Write(")");
                indent = this.Indent;
                this.Indent = indent - 1;
                indent = this.Indent;
                this.Indent = indent - 1;
            }
            indent = this.Indent;
            this.Indent = indent + 1;
            this.GenerateStatements(e.Statements);
            indent = this.Indent;
            this.Indent = indent - 1;
            this.Output.WriteLine("}");
        }

        private void GenerateTypeConstructor(CodeTypeConstructor e)
        {
            if (!this.IsCurrentClass && !this.IsCurrentStruct)
            {
                return;
            }
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes);
            }
            this.Output.Write("static ");
            this.Output.Write(this.CurrentTypeName);
            this.Output.Write("()");
            this.OutputStartingBrace();
            int indent = this.Indent;
            this.Indent = indent + 1;
            this.GenerateStatements(e.Statements);
            indent = this.Indent;
            this.Indent = indent - 1;
            this.Output.WriteLine("}");
        }

        private void GenerateTypeReferenceExpression(CodeTypeReferenceExpression e)
        {
            this.OutputType(e.Type);
        }

        private void GenerateTypeOfExpression(CodeTypeOfExpression e)
        {
            this.Output.Write("typeof(");
            this.OutputType(e.Type);
            this.Output.Write(")");
        }

        private void GenerateType(CodeTypeDeclaration e)
        {
            this.currentClass = e;
            if (e.StartDirectives.Count > 0)
            {
                this.GenerateDirectives(e.StartDirectives);
            }
            this.GenerateCommentStatements(e.Comments);
            if (e.LinePragma != null)
            {
                this.GenerateLinePragmaStart(e.LinePragma);
            }
            this.GenerateTypeStart(e);
            if (this.Options.VerbatimOrder)
            {
                IEnumerator enumerator = e.Members.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        CodeTypeMember member = (CodeTypeMember)enumerator.Current;
                        this.GenerateTypeMember(member, e);
                    }
                    goto IL_CA;
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
            this.GenerateFields(e);
            this.GenerateSnippetMembers(e);
            this.GenerateTypeConstructors(e);
            this.GenerateConstructors(e);
            this.GenerateProperties(e);
            this.GenerateEvents(e);
            this.GenerateMethods(e);
            this.GenerateNestedTypes(e);
        IL_CA:
            this.currentClass = e;
            this.GenerateTypeEnd(e);
            if (e.LinePragma != null)
            {
                this.GenerateLinePragmaEnd(e.LinePragma);
            }
            if (e.EndDirectives.Count > 0)
            {
                this.GenerateDirectives(e.EndDirectives);
            }
        }

        private void GenerateTypes(CodeNamespace e)
        {
            foreach (CodeTypeDeclaration e2 in e.Types)
            {
                if (this.options.BlankLinesBetweenMembers)
                {
                    this.Output.WriteLine();
                }
                ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromType(e2, this.output.InnerWriter, this.options);
            }
        }

        private void GenerateTypeStart(CodeTypeDeclaration e)
        {
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes);
            }
            if (this.IsCurrentDelegate)
            {
                TypeAttributes typeAttributes = e.TypeAttributes & TypeAttributes.VisibilityMask;
                if (typeAttributes != TypeAttributes.NotPublic && typeAttributes == TypeAttributes.Public)
                {
                    this.Output.Write("public ");
                }
                CodeTypeDelegate codeTypeDelegate = (CodeTypeDelegate)e;
                this.Output.Write("delegate ");
                this.OutputType(codeTypeDelegate.ReturnType);
                this.Output.Write(" ");
                this.OutputIdentifier(e.Name);
                this.Output.Write("(");
                this.OutputParameters(codeTypeDelegate.Parameters);
                this.Output.WriteLine(");");
                return;
            }
            this.OutputTypeAttributes(e);
            this.OutputIdentifier(e.Name);
            this.OutputTypeParameters(e.TypeParameters);
            bool flag = true;
            foreach (CodeTypeReference typeRef in e.BaseTypes)
            {
                if (flag)
                {
                    this.Output.Write(" extends ");
                    flag = false;
                }
                else
                {
                    this.Output.Write(", ");
                }
                this.OutputType(typeRef);
            }
            this.OutputTypeParameterConstraints(e.TypeParameters);
            this.OutputStartingBrace();
            int indent = this.Indent;
            this.Indent = indent + 1;
        }

        private void GenerateTypeMember(CodeTypeMember member, CodeTypeDeclaration declaredType)
        {
            if (this.options.BlankLinesBetweenMembers)
            {
                this.Output.WriteLine();
            }
            if (member is CodeTypeDeclaration)
            {
                ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromType((CodeTypeDeclaration)member, this.output.InnerWriter, this.options);
                this.currentClass = declaredType;
                return;
            }
            if (member.StartDirectives.Count > 0)
            {
                this.GenerateDirectives(member.StartDirectives);
            }
            this.GenerateCommentStatements(member.Comments);
            if (member.LinePragma != null)
            {
                this.GenerateLinePragmaStart(member.LinePragma);
            }
            if (member is CodeMemberField)
            {
                this.GenerateField((CodeMemberField)member);
            }
            else if (member is CodeMemberProperty)
            {
                this.GenerateProperty((CodeMemberProperty)member, declaredType);
            }
            else if (member is CodeMemberMethod)
            {
                if (member is CodeConstructor)
                {
                    this.GenerateConstructor((CodeConstructor)member, declaredType);
                }
                else if (member is CodeTypeConstructor)
                {
                    this.GenerateTypeConstructor((CodeTypeConstructor)member);
                }
                else if (member is CodeEntryPointMethod)
                {
                    this.GenerateEntryPointMethod((CodeEntryPointMethod)member, declaredType);
                }
                else
                {
                    this.GenerateMethod((CodeMemberMethod)member, declaredType);
                }
            }
            else if (member is CodeMemberEvent)
            {
                this.GenerateEvent((CodeMemberEvent)member, declaredType);
            }
            else if (member is CodeSnippetTypeMember)
            {
                int indent = this.Indent;
                this.Indent = 0;
                this.GenerateSnippetMember((CodeSnippetTypeMember)member);
                this.Indent = indent;
                this.Output.WriteLine();
            }
            if (member.LinePragma != null)
            {
                this.GenerateLinePragmaEnd(member.LinePragma);
            }
            if (member.EndDirectives.Count > 0)
            {
                this.GenerateDirectives(member.EndDirectives);
            }
        }

        private void GenerateTypeConstructors(CodeTypeDeclaration e)
        {
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeTypeConstructor)
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeTypeConstructor codeTypeConstructor = (CodeTypeConstructor)enumerator.Current;
                    if (codeTypeConstructor.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(codeTypeConstructor.LinePragma);
                    }
                    this.GenerateTypeConstructor(codeTypeConstructor);
                    if (codeTypeConstructor.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(codeTypeConstructor.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                }
            }
        }

        private void GenerateSnippetMembers(CodeTypeDeclaration e)
        {
            IEnumerator enumerator = e.Members.GetEnumerator();
            bool flag = false;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeSnippetTypeMember)
                {
                    flag = true;
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeSnippetTypeMember codeSnippetTypeMember = (CodeSnippetTypeMember)enumerator.Current;
                    if (codeSnippetTypeMember.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(codeSnippetTypeMember.LinePragma);
                    }
                    int indent = this.Indent;
                    this.Indent = 0;
                    this.GenerateSnippetMember(codeSnippetTypeMember);
                    this.Indent = indent;
                    if (codeSnippetTypeMember.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(codeSnippetTypeMember.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                }
            }
            if (flag)
            {
                this.Output.WriteLine();
            }
        }

        private void GenerateNestedTypes(CodeTypeDeclaration e)
        {
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeTypeDeclaration)
                {
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                    CodeTypeDeclaration e2 = (CodeTypeDeclaration)enumerator.Current;
                    ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromType(e2, this.output.InnerWriter, this.options);
                }
            }
        }

        private void GenerateNamespaces(CodeCompileUnit e)
        {
            foreach (CodeNamespace e2 in e.Namespaces)
            {
                ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromNamespace(e2, this.output.InnerWriter, this.options);
            }
        }

        private void OutputAttributeArgument(CodeAttributeArgument arg)
        {
            if (arg.Name != null && arg.Name.Length > 0)
            {
                this.OutputIdentifier(arg.Name);
                this.Output.Write("=");
            }
            ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromExpression(arg.Value, this.output.InnerWriter, this.options);
        }

        private void OutputDirection(FieldDirection dir)
        {
            switch (dir)
            {
                case FieldDirection.In:
                    break;
                case FieldDirection.Out:
                    this.Output.Write("out ");
                    return;
                case FieldDirection.Ref:
                    this.Output.Write("ref ");
                    break;
                default:
                    return;
            }
        }

        private void OutputExpressionList(CodeExpressionCollection expressions)
        {
            this.OutputExpressionList(expressions, false);
        }

        private void OutputExpressionList(CodeExpressionCollection expressions, bool newlineBetweenItems)
        {
            bool flag = true;
            IEnumerator enumerator = expressions.GetEnumerator();
            int indent = this.Indent;
            this.Indent = indent + 1;
            while (enumerator.MoveNext())
            {
                if (flag)
                {
                    flag = false;
                }
                else if (newlineBetweenItems)
                {
                    this.ContinueOnNewLine(",");
                }
                else
                {
                    this.Output.Write(", ");
                }
                ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromExpression((CodeExpression)enumerator.Current, this.output.InnerWriter, this.options);
            }
            indent = this.Indent;
            this.Indent = indent - 1;
        }

        private void OutputParameters(CodeParameterDeclarationExpressionCollection parameters)
        {
            bool flag = true;
            bool flag2 = parameters.Count > 15;
            if (flag2)
            {
                this.Indent += 3;
            }
            IEnumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CodeParameterDeclarationExpression e = (CodeParameterDeclarationExpression)enumerator.Current;
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    this.Output.Write(", ");
                }
                if (flag2)
                {
                    this.ContinueOnNewLine("");
                }
                this.GenerateExpression(e);
            }
            if (flag2)
            {
                this.Indent -= 3;
            }
        }

        private void OutputTypeNamePair(CodeTypeReference typeRef, string name)
        {
            this.OutputIdentifierAsCamelCase(name);
            this.Output.Write(": ");
            this.OutputType(typeRef);
        }

        private void OutputTypeParameters(CodeTypeParameterCollection typeParameters)
        {
            if (typeParameters.Count == 0)
            {
                return;
            }
            this.Output.Write('<');
            bool flag = true;
            for (int i = 0; i < typeParameters.Count; i++)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    this.Output.Write(", ");
                }
                if (typeParameters[i].CustomAttributes.Count > 0)
                {
                    this.GenerateAttributes(typeParameters[i].CustomAttributes, null, true);
                    this.Output.Write(' ');
                }
                this.Output.Write(typeParameters[i].Name);
            }
            this.Output.Write('>');
        }

        private void OutputTypeParameterConstraints(CodeTypeParameterCollection typeParameters)
        {
            if (typeParameters.Count == 0)
            {
                return;
            }
            for (int i = 0; i < typeParameters.Count; i++)
            {
                //this.Output.WriteLine();
                int indent = this.Indent;
                this.Indent = indent + 1;
                bool flag = true;
                if (typeParameters[i].Constraints.Count > 0)
                {
                    foreach (CodeTypeReference typeRef in typeParameters[i].Constraints)
                    {
                        if (flag)
                        {
                            this.Output.Write("where ");
                            this.Output.Write(typeParameters[i].Name);
                            this.Output.Write(" : ");
                            flag = false;
                        }
                        else
                        {
                            this.Output.Write(", ");
                        }
                        this.OutputType(typeRef);
                    }
                }
                if (typeParameters[i].HasConstructorConstraint)
                {
                    if (flag)
                    {
                        this.Output.Write("where ");
                        this.Output.Write(typeParameters[i].Name);
                        this.Output.Write(" : new()");
                    }
                    else
                    {
                        this.Output.Write(", new ()");
                    }
                }
                indent = this.Indent;
                this.Indent = indent - 1;
            }
        }

        private void OutputTypeAttributes(CodeTypeDeclaration e)
        {
            if ((e.Attributes & MemberAttributes.New) != (MemberAttributes)0)
            {
                this.Output.Write("new ");
            }
            TypeAttributes typeAttributes = e.TypeAttributes;
            switch (typeAttributes & TypeAttributes.VisibilityMask)
            {
                case TypeAttributes.NotPublic:
                case TypeAttributes.NestedAssembly:
                case TypeAttributes.NestedFamANDAssem:
                    //this.Output.Write("internal ");
                    break;
                case TypeAttributes.Public:
                case TypeAttributes.NestedPublic:
                    this.Output.Write("export ");
                    break;
                case TypeAttributes.NestedPrivate:
                    this.Output.Write("private ");
                    break;
                case TypeAttributes.NestedFamily:
                    this.Output.Write("protected ");
                    break;
                case TypeAttributes.VisibilityMask:
                    this.Output.Write("protected internal ");
                    break;
            }
            if (e.IsStruct)
            {
                if (e.IsPartial)
                {
                    this.Output.Write("partial ");
                }
                this.Output.Write("struct ");
                return;
            }
            if (e.IsEnum)
            {
                this.Output.Write("enum ");
                return;
            }
            TypeAttributes typeAttributes2 = typeAttributes & TypeAttributes.ClassSemanticsMask;
            if (typeAttributes2 == TypeAttributes.NotPublic)
            {
                if ((typeAttributes & TypeAttributes.Sealed) == TypeAttributes.Sealed)
                {
                    //this.Output.Write("sealed ");
                }
                if ((typeAttributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)
                {
                    this.Output.Write("abstract ");
                }
                if (e.IsPartial)
                {
                    this.Output.Write("partial ");
                }
                this.Output.Write("class ");
                return;
            }
            if (typeAttributes2 != TypeAttributes.ClassSemanticsMask)
            {
                return;
            }
            if (e.IsPartial)
            {
                this.Output.Write("partial ");
            }
            this.Output.Write("interface ");
        }

        private void GenerateTypeEnd(CodeTypeDeclaration e)
        {
            if (!this.IsCurrentDelegate)
            {
                int indent = this.Indent;
                this.Indent = indent - 1;
                this.Output.WriteLine("}");
            }
        }

        private void GenerateNamespaceStart(CodeNamespace e)
        {
            if (e.Name != null && e.Name.Length > 0)
            {
                if (e.UserData["decl"] != null)
                    this.Output.Write("declare ");
                this.Output.Write("module ");
                string[] array = e.Name.Split(new char[]
                {
                    '.'
                });
                this.OutputIdentifier(array[0]);
                for (int i = 1; i < array.Length; i++)
                {
                    this.Output.Write(".");
                    this.OutputIdentifier(array[i]);
                }
                this.OutputStartingBrace();
                int indent = this.Indent;
                this.Indent = indent + 1;
            }
        }

        private void GenerateCompileUnit(CodeCompileUnit e)
        {
            this.GenerateCompileUnitStart(e);
            this.GenerateNamespaces(e);
            this.GenerateCompileUnitEnd(e);
        }

        private void GenerateCompileUnitStart(CodeCompileUnit e)
        {
            if (e.StartDirectives.Count > 0)
            {
                this.GenerateDirectives(e.StartDirectives);
            }
            //this.Output.WriteLine("//------------------------------------------------------------------------------");
            //this.Output.Write("// <");
            //this.Output.WriteLine("AutoGen_Comment_Line1");
            //this.Output.Write("//     ");
            //this.Output.WriteLine("AutoGen_Comment_Line2");
            //this.Output.Write("//     ");
            //this.Output.Write("AutoGen_Comment_Line3");
            //this.Output.WriteLine(Environment.Version.ToString());
            //this.Output.WriteLine("//");
            //this.Output.Write("//     ");
            //this.Output.WriteLine("AutoGen_Comment_Line4");
            //this.Output.Write("//     ");
            //this.Output.WriteLine("AutoGen_Comment_Line5");
            //this.Output.Write("// </");
            //this.Output.WriteLine("AutoGen_Comment_Line1");
            //this.Output.WriteLine("//------------------------------------------------------------------------------");
            //this.Output.WriteLine("");
            SortedList sortedList = new SortedList(StringComparer.Ordinal);
            foreach (CodeNamespace codeNamespace in e.Namespaces)
            {
                if (string.IsNullOrEmpty(codeNamespace.Name))
                {
                    codeNamespace.UserData["GenerateImports"] = false;
                    foreach (CodeNamespaceImport codeNamespaceImport in codeNamespace.Imports)
                    {
                        if (!sortedList.Contains(codeNamespaceImport.Namespace))
                        {
                            sortedList.Add(codeNamespaceImport.Namespace, codeNamespaceImport.Namespace);
                        }
                    }
                }
            }
            foreach (string ident in sortedList.Keys)
            {
                this.Output.Write("import ");
                this.OutputIdentifier(ident);
                this.Output.WriteLine(";");
            }
            if (sortedList.Keys.Count > 0)
            {
                this.Output.WriteLine("");
            }
            if (e.AssemblyCustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.AssemblyCustomAttributes, "assembly: ");
                this.Output.WriteLine("");
            }
        }

        private void GenerateCompileUnitEnd(CodeCompileUnit e)
        {
            if (e.EndDirectives.Count > 0)
            {
                this.GenerateDirectives(e.EndDirectives);
            }
        }

        private void GenerateDirectionExpression(CodeDirectionExpression e)
        {
            this.OutputDirection(e.Direction);
            this.GenerateExpression(e.Expression);
        }

        private void GenerateDirectives(CodeDirectiveCollection directives)
        {
            for (int i = 0; i < directives.Count; i++)
            {
                CodeDirective codeDirective = directives[i];
                if (codeDirective is CodeChecksumPragma)
                {
                    this.GenerateChecksumPragma((CodeChecksumPragma)codeDirective);
                }
                else if (codeDirective is CodeRegionDirective)
                {
                    this.GenerateCodeRegionDirective((CodeRegionDirective)codeDirective);
                }
            }
        }

        private void GenerateChecksumPragma(CodeChecksumPragma checksumPragma)
        {
            this.Output.Write("#pragma checksum \"");
            this.Output.Write(checksumPragma.FileName);
            this.Output.Write("\" \"");
            this.Output.Write(checksumPragma.ChecksumAlgorithmId.ToString("B", CultureInfo.InvariantCulture));
            this.Output.Write("\" \"");
            if (checksumPragma.ChecksumData != null)
            {
                byte[] checksumData = checksumPragma.ChecksumData;
                for (int i = 0; i < checksumData.Length; i++)
                {
                    byte b = checksumData[i];
                    this.Output.Write(b.ToString("X2", CultureInfo.InvariantCulture));
                }
            }
            this.Output.WriteLine("\"");
        }

        private void GenerateCodeRegionDirective(CodeRegionDirective regionDirective)
        {
            if (regionDirective.RegionMode == CodeRegionMode.Start)
            {
                this.Output.Write("#region ");
                this.Output.WriteLine(regionDirective.RegionText);
                return;
            }
            if (regionDirective.RegionMode == CodeRegionMode.End)
            {
                this.Output.WriteLine("#endregion");
            }
        }

        private void GenerateNamespaceEnd(CodeNamespace e)
        {
            if (e.Name != null && e.Name.Length > 0)
            {
                int indent = this.Indent;
                this.Indent = indent - 1;
                this.Output.WriteLine("}");
            }
        }

        private void GenerateNamespaceImport(CodeNamespaceImport e)
        {
            this.Output.Write("using ");
            this.OutputIdentifier(e.Namespace);
            this.Output.WriteLine(";");
        }

        private void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes)
        {
            this.Output.Write("[");
        }

        private void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes)
        {
            this.Output.Write("]");
        }

        private void GenerateAttributes(CodeAttributeDeclarationCollection attributes)
        {
            this.GenerateAttributes(attributes, null, false);
        }

        private void GenerateAttributes(CodeAttributeDeclarationCollection attributes, string prefix)
        {
            this.GenerateAttributes(attributes, prefix, false);
        }

        private void GenerateAttributes(CodeAttributeDeclarationCollection attributes, string prefix, bool inLine)
        {
            return;
            //if (attributes.Count == 0)
            //{
            //    return;
            //}
            //IEnumerator enumerator = attributes.GetEnumerator();
            //bool flag = false;
            //while (enumerator.MoveNext())
            //{
            //    CodeAttributeDeclaration codeAttributeDeclaration = (CodeAttributeDeclaration)enumerator.Current;
            //    if (codeAttributeDeclaration.Name.Equals("system.paramarrayattribute", StringComparison.OrdinalIgnoreCase))
            //    {
            //        flag = true;
            //    }
            //    else
            //    {
            //        this.GenerateAttributeDeclarationsStart(attributes);
            //        if (prefix != null)
            //        {
            //            this.Output.Write(prefix);
            //        }
            //        if (codeAttributeDeclaration.AttributeType != null)
            //        {
            //            this.Output.Write(this.GetTypeOutput(codeAttributeDeclaration.AttributeType));
            //        }
            //        this.Output.Write("(");
            //        bool flag2 = true;
            //        foreach (CodeAttributeArgument arg in codeAttributeDeclaration.Arguments)
            //        {
            //            if (flag2)
            //            {
            //                flag2 = false;
            //            }
            //            else
            //            {
            //                this.Output.Write(", ");
            //            }
            //            this.OutputAttributeArgument(arg);
            //        }
            //        this.Output.Write(")");
            //        this.GenerateAttributeDeclarationsEnd(attributes);
            //        if (inLine)
            //        {
            //            this.Output.Write(" ");
            //        }
            //        else
            //        {
            //            this.Output.WriteLine();
            //        }
            //    }
            //}
            //if (flag)
            //{
            //    if (prefix != null)
            //    {
            //        this.Output.Write(prefix);
            //    }
            //    this.Output.Write("params");
            //    if (inLine)
            //    {
            //        this.Output.Write(" ");
            //        return;
            //    }
            //    this.Output.WriteLine();
            //}
        }

        private static bool IsKeyword(string value)
        {
            return TypeScriptCodeGenerator.keywords.Contains(value);
        }

        private static bool IsPrefixTwoUnderscore(string value)
        {
            return value.Length >= 3 && (value[0] == '_' && value[1] == '_') && value[2] != '_';
        }

        public bool Supports(GeneratorSupport support)
        {
            return (support & (GeneratorSupport.ArraysOfArrays | GeneratorSupport.EntryPointMethod | GeneratorSupport.GotoStatements | GeneratorSupport.MultidimensionalArrays | GeneratorSupport.StaticConstructors | GeneratorSupport.TryCatchStatements | GeneratorSupport.ReturnTypeAttributes | GeneratorSupport.DeclareValueTypes | GeneratorSupport.DeclareEnums | GeneratorSupport.DeclareDelegates | GeneratorSupport.DeclareInterfaces | GeneratorSupport.DeclareEvents | GeneratorSupport.AssemblyAttributes | GeneratorSupport.ParameterAttributes | GeneratorSupport.ReferenceParameters | GeneratorSupport.ChainedConstructorArguments | GeneratorSupport.NestedTypes | GeneratorSupport.MultipleInterfaceMembers | GeneratorSupport.PublicStaticMembers | GeneratorSupport.ComplexExpressions | GeneratorSupport.Win32Resources | GeneratorSupport.Resources | GeneratorSupport.PartialTypes | GeneratorSupport.GenericTypeReference | GeneratorSupport.GenericTypeDeclaration | GeneratorSupport.DeclareIndexerProperties)) == support;
        }

        public bool IsValidIdentifier(string value)
        {
            if (value == null || value.Length == 0)
            {
                return false;
            }
            if (value.Length > 512)
            {
                return false;
            }
            if (value[0] != '@')
            {
                if (TypeScriptCodeGenerator.IsKeyword(value))
                {
                    return false;
                }
            }
            else
            {
                value = value.Substring(1);
            }
            return System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(value);
        }

        public void ValidateIdentifier(string value)
        {
            if (!this.IsValidIdentifier(value))
            {
                throw new ArgumentException("InvalidIdentifier");
            }
        }

        public string CreateValidIdentifier(string name)
        {
            if (TypeScriptCodeGenerator.IsPrefixTwoUnderscore(name))
            {
                name = "_" + name;
            }
            while (TypeScriptCodeGenerator.IsKeyword(name))
            {
                name = "_" + name;
            }
            return name;
        }

        public string CreateEscapedIdentifier(string name)
        {
            //if (TypeScriptCodeGenerator.IsKeyword(name) || TypeScriptCodeGenerator.IsPrefixTwoUnderscore(name))
            if (TypeScriptCodeGenerator.IsKeyword(name))
            {
                return "$" + name;
            }
            return name;
        }

        private string GetBaseTypeOutput(CodeTypeReference typeRef)
        {
            string baseType = typeRef.BaseType;
            if (baseType.Length == 0)
            {
                return "void";
            }
            switch (baseType.ToLower())
            {
                case "system.int16":
                    return "number";

                case "system.int32":
                    return "number";

                case "system.int64":
                    return "number";

                case "system.string":
                    return "string";

                case "system.object":
                    return "any";

                case "system.boolean":
                    return "boolean";

                case "system.void":
                    return "void";

                case "system.char":
                    return "string";

                case "system.byte":
                    return "number";

                case "system.uint16":
                    return "number";

                case "system.uint32":
                    return "number";

                case "system.uint64":
                    return "number";

                case "system.sbyte":
                    return "number";

                case "system.single":
                    return "number";

                case "system.double":
                    return "number";

                case "system.datetime":
                    return "Date";

                case "system.timespan":
                    return "number";

                case "system.decimal":
                    throw new Exception();
            }

            StringBuilder stringBuilder = new StringBuilder(baseType.Length + 10);
            if ((typeRef.Options & CodeTypeReferenceOptions.GlobalReference) != (CodeTypeReferenceOptions)0)
            {
                stringBuilder.Append("global::");
            }

            int num2 = 0;
            int num3 = 0;
            for (int i = 0; i < baseType.Length; i++)
            {
                char c = baseType[i];
                if (c != '+' && c != '.')
                {
                    if (c == '`')
                    {
                        stringBuilder.Append(this.CreateEscapedIdentifier(baseType.Substring(num2, i - num2)));
                        i++;
                        int num4 = 0;
                        while (i < baseType.Length && baseType[i] >= '0' && baseType[i] <= '9')
                        {
                            num4 = num4 * 10 + (int)(baseType[i] - '0');
                            i++;
                        }
                        this.GetTypeArgumentsOutput(typeRef.TypeArguments, num3, num4, stringBuilder);
                        num3 += num4;
                        if (i < baseType.Length && (baseType[i] == '+' || baseType[i] == '.'))
                        {
                            stringBuilder.Append('.');
                            i++;
                        }
                        num2 = i;
                    }
                }
                else
                {
                    stringBuilder.Append(this.CreateEscapedIdentifier(baseType.Substring(num2, i - num2)));
                    stringBuilder.Append('.');
                    i++;
                    num2 = i;
                }
            }
            if (num2 < baseType.Length)
            {
                stringBuilder.Append(this.CreateEscapedIdentifier(baseType.Substring(num2)));
            }
            return stringBuilder.ToString();
        }

        private string GetTypeArgumentsOutput(CodeTypeReferenceCollection typeArguments)
        {
            StringBuilder stringBuilder = new StringBuilder(128);
            this.GetTypeArgumentsOutput(typeArguments, 0, typeArguments.Count, stringBuilder);
            return stringBuilder.ToString();
        }

        private void GetTypeArgumentsOutput(CodeTypeReferenceCollection typeArguments, int start, int length, StringBuilder sb)
        {
            sb.Append('<');
            bool flag = true;
            for (int i = start; i < start + length; i++)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    sb.Append(", ");
                }
                if (i < typeArguments.Count)
                {
                    sb.Append(this.GetTypeOutput(typeArguments[i]));
                }
            }
            sb.Append('>');
        }

        public string GetTypeOutput(CodeTypeReference typeRef)
        {
            string text = string.Empty;
            CodeTypeReference codeTypeReference = typeRef;
            while (codeTypeReference.ArrayElementType != null)
            {
                codeTypeReference = codeTypeReference.ArrayElementType;
            }
            text += this.GetBaseTypeOutput(codeTypeReference);
            while (typeRef != null && typeRef.ArrayRank > 0)
            {
                char[] array = new char[typeRef.ArrayRank + 1];
                array[0] = '[';
                array[typeRef.ArrayRank] = ']';
                for (int i = 1; i < typeRef.ArrayRank; i++)
                {
                    array[i] = ',';
                }
                text += new string(array);
                typeRef = typeRef.ArrayElementType;
            }
            return text;
        }

        private void OutputStartingBrace()
        {
            if (this.Options.BracingStyle == "C")
            {
                this.Output.WriteLine("");
                this.Output.WriteLine("{");
                return;
            }
            this.Output.WriteLine(" {");
        }

        //private CompilerResults FromFileBatch(CompilerParameters options, string[] fileNames)
        //{
        //    if (options == null)
        //    {
        //        throw new ArgumentNullException("options");
        //    }
        //    if (fileNames == null)
        //    {
        //        throw new ArgumentNullException("fileNames");
        //    }
        //    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
        //    string file = null;
        //    int num = 0;
        //    CompilerResults compilerResults = new CompilerResults(options.TempFiles);
        //    new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Assert();
        //    try
        //    {
        //        compilerResults.Evidence = options.Evidence;
        //    }
        //    finally
        //    {
        //        CodeAccessPermission.RevertAssert();
        //    }
        //    bool flag = false;
        //    if (options.OutputAssembly == null || options.OutputAssembly.Length == 0)
        //    {
        //        string fileExtension = options.GenerateExecutable ? "exe" : "dll";
        //        options.OutputAssembly = compilerResults.TempFiles.AddExtension(fileExtension, !options.GenerateInMemory);
        //        new FileStream(options.OutputAssembly, FileMode.Create, FileAccess.ReadWrite).Close();
        //        flag = true;
        //    }
        //    string text = "pdb";
        //    if (options.CompilerOptions != null && -1 != CultureInfo.InvariantCulture.CompareInfo.IndexOf(options.CompilerOptions, "/debug:pdbonly", CompareOptions.IgnoreCase))
        //    {
        //        compilerResults.TempFiles.AddExtension(text, true);
        //    }
        //    else
        //    {
        //        compilerResults.TempFiles.AddExtension(text);
        //    }
        //    string text2 = this.CmdArgsFromParameters(options) + " " + TypeScriptCodeGenerator.JoinStringArray(fileNames, " ");
        //    string responseFileCmdArgs = this.GetResponseFileCmdArgs(options, text2);
        //    string trueArgs = null;
        //    if (responseFileCmdArgs != null)
        //    {
        //        trueArgs = text2;
        //        text2 = responseFileCmdArgs;
        //    }
        //    this.Compile(options, RedistVersionInfo.GetCompilerPath(this.provOptions, this.CompilerName), this.CompilerName, text2, ref file, ref num, trueArgs);
        //    compilerResults.NativeCompilerReturnValue = num;
        //    if (num != 0 || options.WarningLevel > 0)
        //    {
        //        string[] array = TypeScriptCodeGenerator.ReadAllLines(file, Encoding.UTF8, FileShare.ReadWrite);
        //        for (int i = 0; i < array.Length; i++)
        //        {
        //            string text3 = array[i];
        //            compilerResults.Output.Add(text3);
        //            this.ProcessCompilerOutputLine(compilerResults, text3);
        //        }
        //        if (num != 0 & flag)
        //        {
        //            File.Delete(options.OutputAssembly);
        //        }
        //    }
        //    if (compilerResults.Errors.HasErrors || !options.GenerateInMemory)
        //    {
        //        compilerResults.PathToAssembly = options.OutputAssembly;
        //        return compilerResults;
        //    }
        //    byte[] rawAssembly = File.ReadAllBytes(options.OutputAssembly);
        //    byte[] rawSymbolStore = null;
        //    try
        //    {
        //        string path = options.TempFiles.BasePath + "." + text;
        //        if (File.Exists(path))
        //        {
        //            rawSymbolStore = File.ReadAllBytes(path);
        //        }
        //    }
        //    catch
        //    {
        //        rawSymbolStore = null;
        //    }
        //    new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Assert();
        //    try
        //    {
        //        compilerResults.CompiledAssembly = Assembly.Load(rawAssembly, rawSymbolStore, options.Evidence);
        //    }
        //    finally
        //    {
        //        CodeAccessPermission.RevertAssert();
        //    }
        //    return compilerResults;
        //}

        private static string[] ReadAllLines(string file, Encoding encoding, FileShare share)
        {
            string[] result;
            using (FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read, share))
            {
                List<string> list = new List<string>();
                using (StreamReader streamReader = new StreamReader(fileStream, encoding))
                {
                    string item;
                    while ((item = streamReader.ReadLine()) != null)
                    {
                        list.Add(item);
                    }
                }
                result = list.ToArray();
            }
            return result;
        }

        CompilerResults System.CodeDom.Compiler.ICodeCompiler.CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit e)
        {
            throw new NotImplementedException();
        }

        CompilerResults System.CodeDom.Compiler.ICodeCompiler.CompileAssemblyFromFile(CompilerParameters options, string fileName)
        {
            throw new NotImplementedException();
        }

        CompilerResults System.CodeDom.Compiler.ICodeCompiler.CompileAssemblyFromSource(CompilerParameters options, string source)
        {
            throw new NotImplementedException();
        }

        CompilerResults System.CodeDom.Compiler.ICodeCompiler.CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
        {
            throw new NotImplementedException();
        }

        CompilerResults System.CodeDom.Compiler.ICodeCompiler.CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
        {
            throw new NotImplementedException();
        }

        CompilerResults System.CodeDom.Compiler.ICodeCompiler.CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            throw new NotImplementedException();
        }

        //private CompilerResults FromDom(CompilerParameters options, CodeCompileUnit e)
        //{
        //    if (options == null)
        //    {
        //        throw new ArgumentNullException("options");
        //    }
        //    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
        //    return this.FromDomBatch(options, new CodeCompileUnit[]
        //    {
        //        e
        //    });
        //}

        //private CompilerResults FromFile(CompilerParameters options, string fileName)
        //{
        //    if (options == null)
        //    {
        //        throw new ArgumentNullException("options");
        //    }
        //    if (fileName == null)
        //    {
        //        throw new ArgumentNullException("fileName");
        //    }
        //    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
        //    using (File.OpenRead(fileName))
        //    {
        //    }
        //    return this.FromFileBatch(options, new string[]
        //    {
        //        fileName
        //    });
        //}

        //private CompilerResults FromSource(CompilerParameters options, string source)
        //{
        //    if (options == null)
        //    {
        //        throw new ArgumentNullException("options");
        //    }
        //    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
        //    return this.FromSourceBatch(options, new string[]
        //    {
        //        source
        //    });
        //}

        //private CompilerResults FromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        //{
        //    if (options == null)
        //    {
        //        throw new ArgumentNullException("options");
        //    }
        //    if (ea == null)
        //    {
        //        throw new ArgumentNullException("ea");
        //    }
        //    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
        //    string[] array = new string[ea.Length];
        //    CompilerResults result = null;
        //    try
        //    {
        //        WindowsImpersonationContext impersonation = Executor.RevertImpersonation();
        //        try
        //        {
        //            for (int i = 0; i < ea.Length; i++)
        //            {
        //                if (ea[i] != null)
        //                {
        //                    this.ResolveReferencedAssemblies(options, ea[i]);
        //                    array[i] = options.TempFiles.AddExtension(i + this.FileExtension);
        //                    Stream stream = new FileStream(array[i], FileMode.Create, FileAccess.Write, FileShare.Read);
        //                    try
        //                    {
        //                        using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
        //                        {
        //                            ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromCompileUnit(ea[i], streamWriter, this.Options);
        //                            streamWriter.Flush();
        //                        }
        //                    }
        //                    finally
        //                    {
        //                        stream.Close();
        //                    }
        //                }
        //            }
        //            result = this.FromFileBatch(options, array);
        //        }
        //        finally
        //        {
        //            Executor.ReImpersonate(impersonation);
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return result;
        //}

        private void ResolveReferencedAssemblies(CompilerParameters options, CodeCompileUnit e)
        {
            if (e.ReferencedAssemblies.Count > 0)
            {
                foreach (string current in e.ReferencedAssemblies)
                {
                    if (!options.ReferencedAssemblies.Contains(current))
                    {
                        options.ReferencedAssemblies.Add(current);
                    }
                }
            }
        }

        //private CompilerResults FromSourceBatch(CompilerParameters options, string[] sources)
        //{
        //    if (options == null)
        //    {
        //        throw new ArgumentNullException("options");
        //    }
        //    if (sources == null)
        //    {
        //        throw new ArgumentNullException("sources");
        //    }
        //    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
        //    string[] array = new string[sources.Length];
        //    CompilerResults result = null;
        //    try
        //    {
        //        WindowsImpersonationContext impersonation = Executor.RevertImpersonation();
        //        try
        //        {
        //            for (int i = 0; i < sources.Length; i++)
        //            {
        //                string text = options.TempFiles.AddExtension(i + this.FileExtension);
        //                Stream stream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.Read);
        //                try
        //                {
        //                    using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
        //                    {
        //                        streamWriter.Write(sources[i]);
        //                        streamWriter.Flush();
        //                    }
        //                }
        //                finally
        //                {
        //                    stream.Close();
        //                }
        //                array[i] = text;
        //            }
        //            result = this.FromFileBatch(options, array);
        //        }
        //        finally
        //        {
        //            Executor.ReImpersonate(impersonation);
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return result;
        //}

        private static string JoinStringArray(string[] sa, string separator)
        {
            if (sa == null || sa.Length == 0)
            {
                return string.Empty;
            }
            if (sa.Length == 1)
            {
                return "\"" + sa[0] + "\"";
            }
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < sa.Length - 1; i++)
            {
                stringBuilder.Append("\"");
                stringBuilder.Append(sa[i]);
                stringBuilder.Append("\"");
                stringBuilder.Append(separator);
            }
            stringBuilder.Append("\"");
            stringBuilder.Append(sa[sa.Length - 1]);
            stringBuilder.Append("\"");
            return stringBuilder.ToString();
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o)
        {
            bool flag = false;
            if (this.output != null && w != this.output.InnerWriter)
            {
                throw new InvalidOperationException("CodeGenOutputWriter");
            }
            if (this.output == null)
            {
                flag = true;
                this.options = ((o == null) ? new CodeGeneratorOptions() : o);
                this.output = new IndentedTextWriter(w, this.options.IndentString);
            }
            try
            {
                this.GenerateType(e);
            }
            finally
            {
                if (flag)
                {
                    this.output = null;
                    this.options = null;
                }
            }
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o)
        {
            bool flag = false;
            if (this.output != null && w != this.output.InnerWriter)
            {
                throw new InvalidOperationException("CodeGenOutputWriter");
            }
            if (this.output == null)
            {
                flag = true;
                this.options = ((o == null) ? new CodeGeneratorOptions() : o);
                this.output = new IndentedTextWriter(w, this.options.IndentString);
            }
            try
            {
                this.GenerateExpression(e);
            }
            finally
            {
                if (flag)
                {
                    this.output = null;
                    this.options = null;
                }
            }
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o)
        {
            bool flag = false;
            if (this.output != null && w != this.output.InnerWriter)
            {
                throw new InvalidOperationException("CodeGenOutputWriter");
            }
            if (this.output == null)
            {
                flag = true;
                this.options = ((o == null) ? new CodeGeneratorOptions() : o);
                this.output = new IndentedTextWriter(w, this.options.IndentString);
            }
            try
            {
                if (e is CodeSnippetCompileUnit)
                {
                    this.GenerateSnippetCompileUnit((CodeSnippetCompileUnit)e);
                }
                else
                {
                    this.GenerateCompileUnit(e);
                }
            }
            finally
            {
                if (flag)
                {
                    this.output = null;
                    this.options = null;
                }
            }
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o)
        {
            bool flag = false;
            if (this.output != null && w != this.output.InnerWriter)
            {
                throw new InvalidOperationException("CodeGenOutputWriter");
            }
            if (this.output == null)
            {
                flag = true;
                this.options = ((o == null) ? new CodeGeneratorOptions() : o);
                this.output = new IndentedTextWriter(w, this.options.IndentString);
            }
            try
            {
                this.GenerateNamespace(e);
            }
            finally
            {
                if (flag)
                {
                    this.output = null;
                    this.options = null;
                }
            }
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o)
        {
            bool flag = false;
            if (this.output != null && w != this.output.InnerWriter)
            {
                throw new InvalidOperationException("CodeGenOutputWriter");
            }
            if (this.output == null)
            {
                flag = true;
                this.options = ((o == null) ? new CodeGeneratorOptions() : o);
                this.output = new IndentedTextWriter(w, this.options.IndentString);
            }
            try
            {
                this.GenerateStatement(e);
            }
            finally
            {
                if (flag)
                {
                    this.output = null;
                    this.options = null;
                }
            }
        }

        class Indentation
        {
            private int indent;
            private string s;
            private IndentedTextWriter writer;
            private string tabString;

            internal Indentation(IndentedTextWriter writer, int indent, string tabString)
            {
                this.writer = writer;
                this.indent = indent;
                this.s = null;
                this.tabString = tabString;
            }

            internal string IndentationString
            {
                get
                {
                    if (this.s == null)
                    {
                        string tabString = this.tabString;
                        StringBuilder builder = new StringBuilder(this.indent * tabString.Length);
                        for (int i = 0; i < this.indent; i++)
                        {
                            builder.Append(tabString);
                        }
                        this.s = builder.ToString();
                    }
                    return this.s;
                }
            }
        }
    }
}
