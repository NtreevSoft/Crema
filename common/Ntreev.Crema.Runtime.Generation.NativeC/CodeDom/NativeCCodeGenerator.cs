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
using System.CodeDom.Compiler;
using System.IO;
using System.CodeDom;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections;
using System.Globalization;

namespace Ntreev.Crema.Runtime.Generation.NativeC.CodeDom
{
    class NativeCCodeGenerator : System.CodeDom.Compiler.ICodeGenerator
    {
        HashSet<string> preDefinedTypes = new HashSet<string>();
        Stack typeStack = new Stack();
        IndentedTextWriter output;
        IndentedTextWriter cppOutput = new IndentedTextWriter(new StringWriter());
        bool cppMode = false;
        string indentString = IndentedTextWriter.DefaultTabString;
        CodeGeneratorOptions options;
        private bool generatingForLoop;
        private bool inNestedBinary;
        MemberAttributes memberAttribute;
        CodeMemberProperty currentProperty;

        CodeTypeDeclaration currentClass;
        CodeTypeMember currentMember;

        private static readonly string[] keywords = new string[]
        {
        "alignas"
        , "alignof" 
        , "and"
        , "and_eq"
        , "asm"
        , "auto"
        , "bitand"
        , "bitor"
        , "bool"
        , "break"
        , "case"
        , "catch"
        , "char"
        , "char16_t"
        , "char32_t"
        , "class"
        , "compl"
        , "const"
        , "constexpr" 
        , "const_cast"
        , "continue"
        , "decltype" 
        , "default"
        , "delete"
        , "do"
        , "double"
        , "dynamic_cast"
        , "else"
        , "enum"
        , "explicit"
        , "export"
        , "extern"
        , "false"
        , "float"
        , "for"
        , "friend"
        , "goto"
        , "if"
        , "inline"
        , "int"
        , "long"
        , "mutable"
        , "namespace"
        , "new"
        , "noexcept"
        , "not"
        , "not_eq"
        , "nullptr" 
        , "operator"
        , "or"
        , "or_eq"
        , "private"
        , "protected"
        , "public"
        , "register"
        , "reinterpret_cast"
        , "return"
        , "short"
        , "signed"
        , "sizeof"
        , "static"
        , "static_assert" 
        , "static_cast"
        , "struct"
        , "switch"
        , "template"
        , "this"
        , "thread_local" 
        , "throw"
        , "true"
        , "try"
        , "typedef"
        , "typeid"
        , "typename"
        , "union"
        , "unsigned"
        , "using"
        , "virtual"
        , "void"
        , "volatile"
        , "wchar_t"
        , "while"
        , "xor"
        , "xor_eq"
        };

        private static HashSet<string> keywordSet = new HashSet<string>(keywords);

        private static bool IsKeyword(string value)
        {
            return keywordSet.Contains(value);
        }

        private void GenerateNamespaceStart(CodeNamespace e)
        {
            string ns = string.Join(" { ", e.Name.Split(new string[] { "::", }, StringSplitOptions.RemoveEmptyEntries).Select(item => "namespace " + item));

            this.Output.WriteLine(ns);
            this.Output.WriteLine("{");
        }

        private void GenerateNamespaceEnd(CodeNamespace e)
        {
            string ns = string.Join(" } ", e.Name.Split(new string[] { "::", }, StringSplitOptions.RemoveEmptyEntries).Select(item => string.Format("/*namespace {0}*/", item)));

            this.Output.Write("}");
            this.Output.WriteLine(ns);
        }

        private void GenerateStatement(CodeStatement e)
        {
            if (e.StartDirectives.Count > 0)
            {
                //this.GenerateDirectives(e.StartDirectives);
            }
            if (e.LinePragma != null)
            {
                // this.GenerateLinePragmaStart(e.LinePragma);
            }
            if (e is CodeCommentStatement)
            {
                this.GenerateCommentStatement(e as CodeCommentStatement);
            }
            else if (e is CodeMethodReturnStatement)
            {
                this.GenerateMethodReturnStatement(e as CodeMethodReturnStatement);
            }
            else if (e is CodeConditionStatement)
            {
                this.GenerateConditionStatement(e as CodeConditionStatement);
            }
            else if (e is CodeTryCatchFinallyStatement)
            {
                this.GenerateTryCatchFinallyStatement((CodeTryCatchFinallyStatement)e);
            }
            else if (e is CodeAssignStatement)
            {
                this.GenerateAssignStatement(e as CodeAssignStatement);
            }
            else if (e is CodeExpressionStatement)
            {
                this.GenerateExpressionStatement(e as CodeExpressionStatement);
            }
            else if (e is CodeIterationStatement)
            {
                this.GenerateIterationStatement(e as CodeIterationStatement);
            }
            else if (e is CodeThrowExceptionStatement)
            {
                this.GenerateThrowExceptionStatement(e as CodeThrowExceptionStatement);
            }
            else if (e is CodeSnippetStatement)
            {
                this.GenerateSnippetStatement((CodeSnippetStatement)e);
            }
            else if (e is CodeVariableDeclarationStatement)
            {
                this.GenerateVariableDeclarationStatement((CodeVariableDeclarationStatement)e);
            }
            else if (e is CodeAttachEventStatement)
            {
                throw new NotImplementedException();
                //this.GenerateAttachEventStatement((CodeAttachEventStatement)e);
            }
            else if (e is CodeRemoveEventStatement)
            {
                throw new NotImplementedException();
                //this.GenerateRemoveEventStatement((CodeRemoveEventStatement)e);
            }
            else if (e is CodeGotoStatement)
            {
                throw new NotImplementedException();
                //this.GenerateGotoStatement((CodeGotoStatement)e);
            }
            else
            {
                throw new NotImplementedException();
                //if (!(e is CodeLabeledStatement))
                //{
                //    throw new ArgumentException(SR.GetString("InvalidElementType", new object[] { e.GetType().FullName }), "e");
                //}
                //this.GenerateLabeledStatement((CodeLabeledStatement)e);
            }
            if (e.LinePragma != null)
            {
                // this.GenerateLinePragmaEnd(e.LinePragma);
            }
            if (e.EndDirectives.Count > 0)
            {
                //this.GenerateDirectives(e.EndDirectives);
            }

        }

        private void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e)
        {
            this.OutputTypeNamePair(e.Type, e.Name);
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

        private void GenerateSnippetStatement(CodeSnippetStatement e)
        {
            this.Output.WriteLine(e.Value);
        }

        private void GenerateExpressionStatement(CodeExpressionStatement e)
        {
            this.GenerateExpression(e.Expression);
            if (!this.generatingForLoop)
            {
                this.Output.WriteLine(";");
            }
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

        private void GenerateAssignStatement(CodeAssignStatement e)
        {
            this.GenerateExpression(e.Left);
            this.Output.Write(" = ");
            if (e is CodeAssignReferenceStatement == true)
            {
                this.Output.Write("&(");
            }
            this.GenerateExpression(e.Right);
            if (e is CodeAssignReferenceStatement == true)
            {
                this.Output.Write(")");
            }
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
            this.Indent++;
            this.GenerateStatements(e.Statements);
            this.Indent--;
            this.Output.WriteLine("}");
        }

        private void GenerateConditionStatement(CodeConditionStatement e)
        {
            this.Output.Write("if (");
            this.GenerateExpression(e.Condition);
            this.Output.Write(")");
            this.OutputStartingBrace();
            this.Indent++;
            this.GenerateStatements(e.TrueStatements);
            this.Indent--;
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
                this.Indent++;
                this.GenerateStatements(e.FalseStatements);
                this.Indent--;
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
                    this.OutputType(codeCatchClause.CatchExceptionType);
                    this.Output.Write(" ");
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

        private void GenerateComment(CodeComment e)
        {
            string str = e.DocComment ? "///" : "//";
            this.Output.Write(str);
            this.Output.Write(" ");
            string text = e.Text;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '\0')
                {
                    this.Output.Write(text[i]);
                    if (text[i] == '\r')
                    {
                        if ((i < (text.Length - 1)) && (text[i + 1] == '\n'))
                        {
                            this.Output.Write('\n');
                            i++;
                        }
                        this.OutputTabs();
                        this.Output.Write(str);
                    }
                    else if (text[i] == '\n')
                    {
                        this.OutputTabs();
                        this.Output.Write(str);
                    }
                    else if (((text[i] == '\u2028') || (text[i] == '\u2029')) || (text[i] == '\x0085'))
                    {
                        this.Output.Write(str);
                    }
                }
            }
            this.Output.WriteLine();

        }

        private void GenerateMethodReturnStatement(CodeMethodReturnStatement e)
        {
            this.Output.Write("return");
            if (e.Expression != null)
            {
                this.Output.Write(" ");

                if (e.HasCodeType(CodeType.Pointer) == true)
                {
                    this.Output.Write("*(");
                    this.GenerateExpression(e.Expression);
                    this.Output.Write(")");
                }
                else if (e.HasCodeType(CodeType.Reference) == true)
                {
                    this.Output.Write("&(");
                    this.GenerateExpression(e.Expression);
                    this.Output.Write(")");
                }
                else
                {
                    this.GenerateExpression(e.Expression);
                }
            }
            this.Output.WriteLine(";");

        }

        private void GenerateExpression(CodeExpression e)
        {
            if (e is CodeArrayCreateExpression)
            {
                throw new NotImplementedException();
                //this.GenerateArrayCreateExpression((CodeArrayCreateExpression)e);
            }
            else if (e is CodeBaseReferenceExpression)
            {
                this.GenerateBaseReferenceExpression((CodeBaseReferenceExpression)e);
            }
            else if (e is CodeBinaryOperatorExpression)
            {
                this.GenerateBinaryOperatorExpression(e as CodeBinaryOperatorExpression);
            }
            else if (e is CodeCastExpression)
            {
                this.GenerateCastExpression(e as CodeCastExpression);
            }
            else if (e is CodeDelegateCreateExpression)
            {
                throw new NotImplementedException();
                //this.GenerateDelegateCreateExpression((CodeDelegateCreateExpression)e);
            }
            else if (e is CodeFieldReferenceExpression)
            {
                this.GenerateFieldReferenceExpression(e as CodeFieldReferenceExpression);
            }
            else if (e is CodeArgumentReferenceExpression)
            {
                this.GenerateArgumentReferenceExpression((CodeArgumentReferenceExpression)e);
            }
            else if (e is CodeVariableReferenceExpression)
            {
                this.GenerateVariableReferenceExpression(e as CodeVariableReferenceExpression);
            }
            else if (e is CodeIndexerExpression)
            {
                this.GenerateIndexerExpression(e as CodeIndexerExpression);
            }
            else if (e is CodeArrayIndexerExpression)
            {
                throw new NotImplementedException();
                //this.GenerateArrayIndexerExpression((CodeArrayIndexerExpression)e);
            }
            else if (e is CodeSnippetExpression)
            {
                this.GenerateSnippetExpression((CodeSnippetExpression)e);
            }
            else if (e is CodeMethodInvokeExpression)
            {
                this.GenerateMethodInvokeExpression(e as CodeMethodInvokeExpression);
            }
            else if (e is CodeMethodReferenceExpression)
            {
                this.GenerateMethodReferenceExpression((CodeMethodReferenceExpression)e);
            }
            else if (e is CodeEventReferenceExpression)
            {
                throw new NotImplementedException();
                //this.GenerateEventReferenceExpression((CodeEventReferenceExpression)e);
            }
            else if (e is CodeDelegateInvokeExpression)
            {
                throw new NotImplementedException();
                //this.GenerateDelegateInvokeExpression((CodeDelegateInvokeExpression)e);
            }
            else if (e is CodeObjectCreateExpression)
            {
                this.GenerateObjectCreateExpression(e as CodeObjectCreateExpression);
            }
            else if (e is CodeObjectDeleteExpression)
            {
                this.GenerateObjectDeleteExpression(e as CodeObjectDeleteExpression);
            }
            else if (e is CodeParameterDeclarationExpression)
            {
                this.GenerateParameterDeclarationExpression(e as CodeParameterDeclarationExpression);
            }
            else if (e is CodeDirectionExpression)
            {
                throw new NotImplementedException();
                //this.GenerateDirectionExpression((CodeDirectionExpression)e);
            }
            else if (e is CodePrimitiveExpression)
            {
                this.GeneratePrimitiveExpression(e as CodePrimitiveExpression);
            }
            else if (e is CodePropertyReferenceExpression)
            {
                this.GeneratePropertyReferenceExpression(e as CodePropertyReferenceExpression);
            }
            else if (e is CodePropertySetValueReferenceExpression)
            {
                throw new NotImplementedException();
                //this.GeneratePropertySetValueReferenceExpression((CodePropertySetValueReferenceExpression)e);
            }
            else if (e is CodeThisReferenceExpression)
            {
                this.GenerateThisReferenceExpression(e as CodeThisReferenceExpression);
            }
            else if (e is CodeTypeReferenceExpression)
            {
                this.GenerateTypeReferenceExpression(e as CodeTypeReferenceExpression);
            }
            else if (e is CodeTypeOfExpression)
            {
                this.GenerateTypeOfExpression(e as CodeTypeOfExpression);
            }
            else if (e is CodeDefaultValueExpression)
            {
                throw new NotImplementedException();
                // this.GenerateDefaultValueExpression((CodeDefaultValueExpression)e);
            }
            else
            {
                if (e == null)
                {
                    throw new ArgumentNullException("e");
                }
                throw new NotImplementedException();
                //throw new ArgumentException(SR.GetString("InvalidElementType", new object[] { e.GetType().FullName }), "e");
            }

        }

        private void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e)
        {
            this.Output.Write("this");
        }

        private void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e)
        {
            this.OutputIdentifier(e.ParameterName);
        }

        private void GenerateObjectCreateExpression(CodeObjectCreateExpression e)
        {
            if (e.UserData.Contains("reference") == true)
            {
                this.Output.Write("*");
            }

            this.Output.Write("new ");
            this.OutputType(e.CreateType);
            this.Output.Write("(");
            this.OutputExpressionList(e.Parameters);
            this.Output.Write(")");
        }

        private void GenerateObjectDeleteExpression(CodeObjectDeleteExpression e)
        {
            this.Output.Write("delete ");
            this.GenerateExpression(e.TargetObject);
        }

        private void GenerateIndexerExpression(CodeIndexerExpression e)
        {
            this.GenerateExpression(e.TargetObject);
            this.Output.Write("[");
            bool flag = true;
            foreach (CodeExpression expression in e.Indices)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    this.Output.Write(", ");
                }
                this.GenerateExpression(expression);
            }
            this.Output.Write("]");
        }

        private void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                this.GenerateExpression(e.TargetObject);
                if (e.TargetObject is CodeThisReferenceExpression == true)
                    this.Output.Write("->");
                else if (e.TargetObject is CodeVariablePointerExpression == true)
                    this.Output.Write("->");
                else if (e.TargetObject is CodeFieldPointerExpression == true)
                    this.Output.Write("->");
                else
                    this.Output.Write(".");
            }
            if (e.UserData.Contains("propertyName") == true)
                this.OutputIdentifier(e.UserData["propertyName"] as string);
            else
                this.OutputIdentifier(e.PropertyName);
        }

        private void GenerateTypeOfExpression(CodeTypeOfExpression e)
        {
            this.Output.Write("typeid(");
            this.OutputType(e.Type);
            this.Output.Write(")");
        }

        private void GenerateCastExpression(CodeCastExpression e)
        {
            this.Output.Write("((");
            this.OutputType(e.TargetType);
            this.Output.Write(")(");
            this.GenerateExpression(e.Expression);
            this.Output.Write("))");
        }

        private void GenerateTypeReferenceExpression(CodeTypeReferenceExpression e)
        {
            this.OutputType(e.Type);
        }

        private void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e)
        {
            this.GenerateMethodReferenceExpression(e.Method);
            this.Output.Write("(");
            this.OutputExpressionList(e.Parameters);
            this.Output.Write(")");
        }

        private void OutputExpressionList(CodeExpressionCollection expressions)
        {
            this.OutputExpressionList(expressions, false);
        }

        private void OutputExpressionList(CodeExpressionCollection expressions, bool newlineBetweenItems)
        {
            bool flag = true;
            IEnumerator enumerator = expressions.GetEnumerator();
            this.Indent++;
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
            this.Indent--;
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

                if (e.TargetObject is CodeTypeReferenceExpression)
                {
                    this.Output.Write("::");
                }
                else if (e.TargetObject is CodeThisReferenceExpression)
                {
                    this.Output.Write("->");
                }
                else if (e.TargetObject is CodeVariablePointerExpression)
                {
                    this.Output.Write("->");
                }
                else
                {
                    this.Output.Write(".");
                }
            }
            this.OutputIdentifier(e.MethodName);
            if (e.TypeArguments.Count > 0)
            {
                this.Output.Write(this.GetTypeArgumentsOutput(e.TypeArguments));
            }
        }

        private string GetTypeArgumentsOutput(CodeTypeReferenceCollection typeArguments)
        {
            StringBuilder sb = new StringBuilder(0x80);
            this.GetTypeArgumentsOutput(typeArguments, 0, typeArguments.Count, sb);
            return sb.ToString();
        }

        private void GetTypeArgumentsOutput(CodeTypeReferenceCollection typeArguments, int start, int length, StringBuilder sb)
        {
            sb.Append('<');
            bool flag = true;
            for (int i = start; i < (start + length); i++)
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

        private void GeneratePrimitiveChar(char c)
        {
            this.Output.Write('\'');
            switch (c)
            {
                case '\t':
                    this.Output.Write(@"\t");
                    break;

                case '\n':
                    this.Output.Write(@"\n");
                    break;

                case '\r':
                    this.Output.Write(@"\r");
                    break;

                case '"':
                    this.Output.Write("\\\"");
                    break;

                case '\0':
                    this.Output.Write(@"\0");
                    break;

                case '\'':
                    this.Output.Write(@"\'");
                    break;

                case '\\':
                    this.Output.Write(@"\\");
                    break;

                case '\x0084':
                case '\x0085':
                case '\u2028':
                case '\u2029':
                    this.AppendEscapedChar(null, c);
                    break;

                default:
                    if (char.IsSurrogate(c))
                    {
                        this.AppendEscapedChar(null, c);
                    }
                    else
                    {
                        this.Output.Write(c);
                    }
                    break;
            }
            this.Output.Write('\'');
        }

        private void AppendEscapedChar(StringBuilder b, char value)
        {
            if (b == null)
            {
                this.Output.Write(@"\u");
                int num = value;
                this.Output.Write(num.ToString("X4", CultureInfo.InvariantCulture));
            }
            else
            {
                b.Append(@"\u");
                b.Append(((int)value).ToString("X4", CultureInfo.InvariantCulture));
            }
        }

        private void GeneratePrimitiveExpression(CodePrimitiveExpression e)
        {
            if (e.Value is char)
            {
                this.GeneratePrimitiveChar((char)e.Value);
            }
            else if (e.Value is sbyte)
            {
                sbyte num = (sbyte)e.Value;
                this.Output.Write(num.ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is ushort)
            {
                ushort num2 = (ushort)e.Value;
                this.Output.Write(num2.ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is uint)
            {
                uint num3 = (uint)e.Value;
                this.Output.Write(num3.ToString(CultureInfo.InvariantCulture));
                this.Output.Write("u");
            }
            else if (e.Value is ulong)
            {
                ulong num4 = (ulong)e.Value;
                this.Output.Write(num4.ToString(CultureInfo.InvariantCulture));
                this.Output.Write("ul");
            }
            else
            {
                this.GeneratePrimitiveExpressionBase(e);
            }
        }

        private string NullToken
        {
            get
            {
                return "nullptr";
            }
        }

        private string QuoteSnippetString(string value)
        {
            if (((value.Length >= 0x100) && (value.Length <= 0x5dc)) && (value.IndexOf('\0') == -1))
            {
                return this.QuoteSnippetStringVerbatimStyle(value);
            }
            return this.QuoteSnippetStringCStyle(value);
        }

        private string QuoteSnippetStringCStyle(string value)
        {
            StringBuilder b = new StringBuilder(value.Length + 5);
            Indentation indentation = new Indentation((IndentedTextWriter)this.Output, this.Indent + 1, this.indentString);
            b.Append("\"");
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '\u2028':
                    case '\u2029':
                        this.AppendEscapedChar(b, value[i]);
                        break;

                    case '\\':
                        b.Append(@"\\");
                        break;

                    case '\'':
                        b.Append(@"\'");
                        break;

                    case '\t':
                        b.Append(@"\t");
                        break;

                    case '\n':
                        b.Append(@"\n");
                        break;

                    case '\r':
                        b.Append(@"\r");
                        break;

                    case '"':
                        b.Append("\\\"");
                        break;

                    case '\0':
                        b.Append(@"\0");
                        break;

                    default:
                        b.Append(value[i]);
                        break;
                }
                if ((i > 0) && ((i % 80) == 0))
                {
                    if ((char.IsHighSurrogate(value[i]) && (i < (value.Length - 1))) && char.IsLowSurrogate(value[i + 1]))
                    {
                        b.Append(value[++i]);
                    }
                    b.Append("\" +");
                    b.Append(Environment.NewLine);
                    b.Append(indentation.IndentationString);
                    b.Append('"');
                }
            }
            b.Append("\"");
            return b.ToString();
        }

        private string QuoteSnippetStringVerbatimStyle(string value)
        {
            StringBuilder builder = new StringBuilder(value.Length + 5);
            builder.Append("@\"");
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '"')
                {
                    builder.Append("\"\"");
                }
                else
                {
                    builder.Append(value[i]);
                }
            }
            builder.Append("\"");
            return builder.ToString();
        }

        private void GeneratePrimitiveExpressionBase(CodePrimitiveExpression e)
        {
            if (e.Value == null)
            {
                this.Output.Write(this.NullToken);
            }
            else if (e.Value is string)
            {
                this.Output.Write(this.QuoteSnippetString((string)e.Value));
            }
            else if (e.Value is char)
            {
                this.Output.Write("'" + e.Value.ToString() + "'");
            }
            else if (e.Value is byte)
            {
                byte num = (byte)e.Value;
                this.Output.Write(num.ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is short)
            {
                short num2 = (short)e.Value;
                this.Output.Write(num2.ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is int)
            {
                int num3 = (int)e.Value;
                this.Output.Write(num3.ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is long)
            {
                long num4 = (long)e.Value;
                this.Output.Write(num4.ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is float)
            {
                this.GenerateSingleFloatValue((float)e.Value);
            }
            else if (e.Value is double)
            {
                this.GenerateDoubleValue((double)e.Value);
            }
            else if (e.Value is decimal)
            {
                throw new NotImplementedException();
                //this.GenerateDecimalValue((decimal)e.Value);
            }
            else if (e.Value.GetType().IsEnum == true)
            {
                this.Output.Write(e.Value.ToString());
            }
            else
            {
                if (!(e.Value is bool))
                {
                    throw new NotImplementedException();
                    //throw new ArgumentException(SR.GetString("InvalidPrimitiveType", new object[] { e.Value.GetType().ToString() }));
                }
                if ((bool)e.Value)
                {
                    this.Output.Write("true");
                }
                else
                {
                    this.Output.Write("false");
                }
            }
        }

        private void GenerateDoubleValue(double d)
        {
            if (double.IsNaN(d))
            {
                this.Output.Write("double.NaN");
            }
            else if (double.IsNegativeInfinity(d))
            {
                this.Output.Write("double.NegativeInfinity");
            }
            else if (double.IsPositiveInfinity(d))
            {
                this.Output.Write("double.PositiveInfinity");
            }
            else
            {
                this.Output.Write(d.ToString("R", CultureInfo.InvariantCulture));
                this.Output.Write("D");
            }
        }

        private void GenerateSingleFloatValue(float s)
        {
            if (float.IsNaN(s))
            {
                this.Output.Write("float.NaN");
            }
            else if (float.IsNegativeInfinity(s))
            {
                this.Output.Write("float.NegativeInfinity");
            }
            else if (float.IsPositiveInfinity(s))
            {
                this.Output.Write("float.PositiveInfinity");
            }
            else
            {
                this.Output.Write(s.ToString(CultureInfo.InvariantCulture));
                this.Output.Write('F');
            }
        }

        private void GenerateSnippetExpression(CodeSnippetExpression e)
        {
            this.Output.Write(e.Value);
        }

        private void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e)
        {
            this.OutputIdentifier(e.VariableName);
        }

        private void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e)
        {
            bool flag = false;
            this.Output.Write("(");
            this.GenerateExpression(e.Left);
            this.Output.Write(" ");
            if ((e.Left is CodeBinaryOperatorExpression) || (e.Right is CodeBinaryOperatorExpression))
            {
                if (!this.inNestedBinary)
                {
                    flag = true;
                    this.inNestedBinary = true;
                    this.Indent += 3;
                }
                this.ContinueOnNewLine("");
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
                    this.Output.Write("!=");
                    return;

                case CodeBinaryOperatorType.IdentityEquality:
                    this.Output.Write("==");
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
            }
        }

        private void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e)
        {
            //if (e.CustomAttributes.Count > 0)
            //{
            //    this.GenerateAttributes(e.CustomAttributes, null, true);
            //}
            this.OutputDirection(e.Direction);
            this.OutputTypeNamePair(e.Type, e.Name);
        }

        private void OutputTypeNamePair(CodeTypeReference typeRef, string name)
        {
            this.OutputType(typeRef);
            this.Output.Write(" ");
            this.OutputIdentifier(name);
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



        private void GenerateThisReferenceExpression(CodeThisReferenceExpression e)
        {
            this.Output.Write("this");
        }

        private void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                this.GenerateExpression(e.TargetObject);

                if (e.TargetObject is CodeThisReferenceExpression == true)
                    this.Output.Write("->");
                else if(e.TargetObject is CodeTypeReferenceExpression == true)
                    this.Output.Write("::");
                else if(e.TargetObject is CodeVariablePointerExpression == true)
                    this.Output.Write("->");
                else
                    this.Output.Write(".");
            }
            this.OutputIdentifier(e.FieldName);
        }

        private void OutputIdentifier(string ident)
        {
            if(IsKeyword(ident) == true)
                this.Output.Write("$" + ident);
            else
                this.Output.Write(ident);
        }

        private void GenerateStatements(CodeStatementCollection statements)
        {
            foreach (CodeStatement item in statements)
            {
                GenerateStatement(item);
            }
        }

        private void GenerateField(CodeMemberField e, CodeTypeDeclaration c)
        {
            if (c.IsEnum == true)
            {
                if (e.CustomAttributes.Count > 0)
                {
                    this.GenerateAttributes(e.CustomAttributes);
                }
                this.GenerateCommentStatements(e.Comments);
                this.OutputIdentifier(string.Format("{0}_{1}", c.Name, e.Name));
                if (e.InitExpression != null)
                {
                    this.Output.Write(" = ");
                    this.GenerateExpression(e.InitExpression);
                }
                this.Output.WriteLine(",");
            }
            else
            {
                if (e.CustomAttributes.Count > 0)
                {
                    this.GenerateAttributes(e.CustomAttributes);
                }
                this.OutputMemberAccessModifier(e.Attributes);
                this.GenerateCommentStatements(e.Comments);
                this.OutputVTableModifier(e.Attributes);
                this.OutputFieldScopeModifier(e.Attributes);
                this.OutputTypeNamePair(e.Type, e.Name);
                if (e.InitExpression != null)
                {
                    this.Output.Write(" = ");
                    this.GenerateExpression(e.InitExpression);
                }

                this.Output.WriteLine(";");

                if (e.Attributes.HasFlag(MemberAttributes.Static) == true)
                {
                    this.CppMode = true;
                    this.OutputType(e.Type);
                    this.Output.Write(" ");
                    this.Output.Write(c.Name);
                    this.Output.Write("::");
                    this.OutputIdentifier(e.Name);
                    if (e.InitExpression != null)
                    {
                        this.Output.Write(" = ");
                        this.GenerateExpression(e.InitExpression);
                    }
                    this.Output.WriteLine(";");
                    this.Output.WriteLine();
                    this.CppMode = false;
                }
            }
        }

        private void OutputFieldScopeModifier(MemberAttributes attributes)
        {
            switch ((attributes & MemberAttributes.ScopeMask))
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

        private void OutputVTableModifier(MemberAttributes attributes)
        {
            MemberAttributes attributes2 = attributes & MemberAttributes.VTableMask;
            if (attributes2 == MemberAttributes.New)
            {
                this.Output.Write("new ");
            }
        }

        private void OutputMemberAccessModifier(MemberAttributes attributes)
        {
            MemberAttributes attributes2 = attributes & MemberAttributes.AccessMask;
            if (this.memberAttribute == attributes2)
                return;

            bool newLine = true;
            try
            {
                this.Indent--;

                if (attributes2 <= MemberAttributes.Family)
                {
                    if (attributes2 != MemberAttributes.Assembly)
                    {
                        if (attributes2 != MemberAttributes.FamilyAndAssembly)
                        {
                            if (attributes2 == MemberAttributes.Family)
                            {
                                this.Output.Write("protected: ");
                            }
                            return;
                        }
                        this.Output.Write("friend ");
                        newLine = false;
                        return;
                    }
                }
                else
                {
                    switch (attributes2)
                    {
                        case MemberAttributes.FamilyOrAssembly:
                            this.Output.Write("protected internal: ");
                            return;

                        case MemberAttributes.Private:
                            this.Output.Write("private: ");
                            return;

                        case MemberAttributes.Public:
                            this.Output.Write("public: ");
                            return;
                    }
                    return;
                }
                //this.Output.Write("internal: ");
            }
            finally
            {
                if (newLine == true)
                    this.Output.WriteLine();
                this.memberAttribute = attributes2;
                this.Indent++;
            }
        }

        private void GenerateAttributes(CodeAttributeDeclarationCollection codeAttributeDeclarationCollection)
        {
            //throw new NotImplementedException();
        }

        private void GenerateFields(CodeTypeDeclaration e)
        {
            SortedCodeTypeMembers members = new SortedCodeTypeMembers(e.Members, typeof(CodeMemberField));
            this.memberAttribute = 0;
            GenerateFields(members.PublicMembers, e);
            GenerateFields(members.InternalMembers, e);
            GenerateFields(members.ProtectedMembers, e);
            GenerateFields(members.PrivateMembers, e);
        }

        private void GenerateFields(CodeTypeMemberCollection members, CodeTypeDeclaration c)
        {
            if (members.Count > 0)
            {
                //this.Output.WriteLine("{0}:", access);
                foreach (CodeMemberField item in members)
                {
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }

                    GenerateField(item, c);
                }

                if (this.options.BlankLinesBetweenMembers)
                {
                    this.Output.WriteLine();
                }
            }
        }

        private void GenerateTypeConstructor(CodeTypeConstructor e)
        {
            if (this.IsCurrentClass || this.IsCurrentStruct)
            {
                if (e.CustomAttributes.Count > 0)
                {
                    this.GenerateAttributes(e.CustomAttributes);
                }
                this.Output.Write("static ");
                this.Output.Write(this.CurrentTypeName);
                this.Output.Write("()");
                this.OutputStartingBrace();
                this.Indent++;
                this.GenerateStatements(e.Statements);
                this.Indent--;
                this.Output.WriteLine("}");
            }
        }

        //private void GenerateTypeConstructors(CodeTypeMemberCollection members, string access, CodeTypeDeclaration typeDeclaration)
        //{
        //    if (members.Count > 0)
        //    {
        //        this.Output.WriteLine("{0}:", access);
        //        this.Indent++;
        //        foreach (CodeConstructor item in members)
        //        {
        //            GenerateTypeConstructor(item, typeDeclaration);
        //        }

        //        this.Indent--;
        //    }
        //}

        //private void GenerateTypeConstructors(CodeTypeDeclaration typeDeclaration)
        //{
        //    SortedCodeTypeMembers members = new SortedCodeTypeMembers(typeDeclaration.Members, typeof(CodeConstructor));

        //    GenerateTypeConstructors(members.PublicMembers, "public", typeDeclaration);
        //    GenerateTypeConstructors(members.InternalMembers, "public", typeDeclaration);
        //    GenerateTypeConstructors(members.ProtectedMembers, "protected", typeDeclaration);
        //    GenerateTypeConstructors(members.PrivateMembers, "private", typeDeclaration);
        //}

        private void GenerateProperty(CodeMemberProperty property, CodeTypeDeclaration c)
        {
            string baseType = property.Type.BaseType;
            this.currentProperty = property;

            try
            {
                if (property.HasGet == true)
                {
                    this.OutputType(property.Type);
                    this.Output.Write(" ");
                    this.Output.WriteLine("Get{0}() const;", property.Name);

                    {
                        this.CppMode = true;
                        this.Indent = 1;
                        this.OutputType(property.Type);
                        this.Output.Write(" ");
                        this.OutputClassNamespace();
                        this.Output.WriteLine("Get{0}() const", property.Name);
                        this.Output.WriteLine("{");
                        this.Indent++;
                        GenerateStatements(property.GetStatements);
                        this.Indent--;
                        this.Output.WriteLine("}");
                        this.Output.WriteLine();
                        this.CppMode = false;
                    }
                }

                if (property.HasSet == true)
                {
                    this.Output.Write("void");
                    this.Output.Write(" Set{0}", property.Name);
                    this.Output.Write("(");
                    this.OutputType(property.Type);
                    this.Output.Write(")");
                    this.Output.WriteLine("{");
                    this.Indent++;
                    GenerateStatements(property.SetStatements);
                    this.Indent--;
                    this.Output.WriteLine("}");
                    this.Output.WriteLine();
                }
            }
            finally
            {
                this.currentProperty = null;
            }
        }

        //private void GenerateProperties(CodeTypeMemberCollection members, string access)
        //{
        //    if (members.Count > 0)
        //    {
        //        this.Output.WriteLine("{0}:", access);
        //        this.Indent++;
        //        foreach (CodeMemberProperty item in members)
        //        {
        //            GenerateProperty(item);
        //        }

        //        this.Indent--;
        //    }
        //}

        //private void GenerateProperties(CodeTypeDeclaration e)
        //{
        //    SortedCodeTypeMembers members = new SortedCodeTypeMembers(e.Members, typeof(CodeMemberProperty));

        //    GenerateProperties(members.PublicMembers, "public");
        //    GenerateProperties(members.InternalMembers, "public");
        //    GenerateProperties(members.ProtectedMembers, "protected");
        //    GenerateProperties(members.PrivateMembers, "private");
        //}

        private void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c)
        {
            if ((this.IsCurrentClass || this.IsCurrentStruct) || this.IsCurrentInterface)
            {
                if (e.CustomAttributes.Count > 0)
                {
                    this.GenerateAttributes(e.CustomAttributes);
                }
                if (e.ReturnTypeCustomAttributes.Count > 0)
                {
                    //this.GenerateAttributes(e.ReturnTypeCustomAttributes, "return: ");
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
                this.OutputType(e.ReturnType);
                this.Output.Write(" ");
                if (e.PrivateImplementationType != null)
                {
                    this.Output.Write(e.PrivateImplementationType.BaseType);
                    this.Output.Write(".");
                }
                this.OutputIdentifier(e.Name);
                this.OutputTypeParameters(e.TypeParameters);
                this.Output.Write("(");
                this.OutputParameters(e.Parameters);
                this.Output.Write(")");

                if (e.IsConst() == true)
                {
                    this.Output.Write(" const");
                }

                this.Output.WriteLine(";");

                {
                    this.CppMode = true;
                    this.Indent = 1;
                    this.OutputType(e.ReturnType);
                    this.Output.Write(" ");
                    if (e.Attributes.HasFlag(MemberAttributes.FamilyAndAssembly | MemberAttributes.Static) == false)
                        this.OutputClassNamespace();
                    this.OutputIdentifier(e.Name);
                    this.OutputTypeParameters(e.TypeParameters);
                    this.Output.Write("(");
                    this.OutputParameters(e.Parameters);
                    this.Output.Write(")");

                    if (e.IsConst() == true)
                    {
                        this.Output.Write(" const");
                    }
                    this.OutputTypeParameterConstraints(e.TypeParameters);
                    //if (!this.IsCurrentInterface && ((e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract))
                    if (((e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract))
                    {
                        this.OutputStartingBrace();
                        this.Indent++;
                        this.GenerateStatements(e.Statements);
                        this.GenerateStatements(e);
                        this.Indent--;
                        this.Output.WriteLine("}");
                    }
                    else
                    {
                        this.Output.WriteLine(";");
                    }
                    this.Output.WriteLine();

                    this.CppMode = false;
                }
            }

        }

        private void OutputStartingBrace()
        {
            if (this.Options.BracingStyle == "C")
            {
                this.Output.WriteLine("");
                this.Output.WriteLine("{");
            }
            else
            {
                this.Output.WriteLine(" {");
            }
        }

        private void OutputMemberScopeModifier(MemberAttributes attributes)
        {
            switch ((attributes & MemberAttributes.ScopeMask))
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
                    this.Output.Write("virtual ");
                    return;
            }
            switch ((attributes & MemberAttributes.AccessMask))
            {
                case MemberAttributes.Assembly:
                case MemberAttributes.Family:
                case MemberAttributes.Public:
                    this.Output.Write("virtual ");
                    break;
            }
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
                CodeParameterDeclarationExpression current = (CodeParameterDeclarationExpression)enumerator.Current;
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
                this.GenerateExpression(current);
            }
            if (flag2)
            {
                this.Indent -= 3;
            }
        }

        private void ContinueOnNewLine(string p)
        {
            this.Output.WriteLine(p);
        }

        private void OutputTypeParameterConstraints(CodeTypeParameterCollection typeParameters)
        {
            if (typeParameters.Count != 0)
            {
                for (int i = 0; i < typeParameters.Count; i++)
                {
                    this.Output.WriteLine();
                    this.Indent++;
                    bool flag = true;
                    if (typeParameters[i].Constraints.Count > 0)
                    {
                        foreach (CodeTypeReference reference in typeParameters[i].Constraints)
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
                            this.OutputType(reference);
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
                    this.Indent--;
                }
            }
        }



        private void OutputTypeParameters(CodeTypeParameterCollection typeParameters)
        {
            if (typeParameters.Count != 0)
            {
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
                        //this.GenerateAttributes(typeParameters[i].CustomAttributes, null, true);
                        this.Output.Write(' ');
                    }
                    this.Output.Write(typeParameters[i].Name);
                }
                this.Output.Write('>');
            }

        }



        private void OutputType(CodeTypeReference typeRef)
        {
            this.Output.Write(this.GetTypeOutput(typeRef));

        }

        //private void GenerateMethods(CodeTypeMemberCollection members, string access)
        //{
        //    if (members.Count > 0)
        //    {
        //        this.Output.WriteLine("{0}:", access);
        //        this.Indent++;
        //        foreach (CodeMemberMethod item in members)
        //        {
        //            this.GenerateMethod(item);
        //        }

        //        this.Indent--;
        //    }
        //}

        //private void GenerateMethods(CodeTypeDeclaration e)
        //{
        //    SortedCodeTypeMembers members = new SortedCodeTypeMembers(e.Members, typeof(CodeMemberMethod));

        //    this.GenerateMethods(members.PublicMembers, "public");
        //    this.GenerateMethods(members.InternalMembers, "public");
        //    this.GenerateMethods(members.ProtectedMembers, "protected");
        //    this.GenerateMethods(members.PrivateMembers, "private");
        //}

        private void GenerateTypeStart(CodeTypeDeclaration e)
        {
            if (e.CustomAttributes.Count > 0)
            {
                this.GenerateAttributes(e.CustomAttributes);
            }
            if (!this.IsCurrentDelegate)
            {
                this.OutputTypeAttributes(e);
                this.OutputIdentifier(e.Name);
                this.OutputTypeParameters(e.TypeParameters);
                bool flag = true;
                foreach (CodeTypeReference reference in e.BaseTypes)
                {
                    if (flag)
                    {
                        this.Output.Write(" : public ");
                        flag = false;
                    }
                    else
                    {
                        this.Output.Write(", ");
                    }
                    this.OutputType(reference);
                }
                this.OutputTypeParameterConstraints(e.TypeParameters);
                this.OutputStartingBrace();
                this.Indent++;
            }
            else
            {
                switch ((e.TypeAttributes & TypeAttributes.VisibilityMask))
                {
                    case TypeAttributes.Public:
                        this.Output.Write("public ");
                        break;
                }
                CodeTypeDelegate delegate2 = (CodeTypeDelegate)e;
                this.Output.Write("delegate ");
                this.OutputType(delegate2.ReturnType);
                this.Output.Write(" ");
                this.OutputIdentifier(e.Name);
                this.Output.Write("(");
                this.OutputParameters(delegate2.Parameters);
                this.Output.WriteLine(");");
            }
            //if (this.options.BlankLinesBetweenMembers)
            //{
            //    this.Output.WriteLine();
            //}
        }

        private void OutputTypeAttributes(CodeTypeDeclaration e)
        {
            TypeAttributes typeAttributes = e.TypeAttributes;

            TypeAttributes visibility = typeAttributes & TypeAttributes.VisibilityMask;

            switch (visibility)
            {
                case TypeAttributes.NestedPublic:
                    {
                        this.Indent--;
                        this.Output.WriteLine("public:");
                        this.Indent++;
                    }
                    break;
                case TypeAttributes.NestedPrivate:
                    {
                        this.Indent--;
                        this.Output.WriteLine("private:");
                        this.Indent++;
                    }
                    break;
            }

            if (e.IsStruct)
            {
                if (e.IsPartial)
                {
                    this.Output.Write("partial ");
                }
                this.Output.Write("struct ");
            }
            else if (e.IsEnum)
            {
                this.Output.Write("enum ");
            }
            else
            {
                TypeAttributes attributes3 = typeAttributes & TypeAttributes.Interface;
                if (attributes3 != TypeAttributes.AutoLayout)
                {
                    if (attributes3 != TypeAttributes.Interface)
                    {
                        return;
                    }
                }
                else
                {
                    if ((typeAttributes & TypeAttributes.Sealed) == TypeAttributes.Sealed)
                    {
                        this.Output.Write("sealed ");
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
                if (e.IsPartial)
                {
                    this.Output.Write("partial ");
                }
                this.Output.Write("interface ");
            }
        }

        private int Indent
        {
            get
            {
                if (this.cppMode == true)
                    return this.cppOutput.Indent;
                return this.output.Indent;
            }
            set
            {
                if (this.cppMode == true)
                    this.cppOutput.Indent = value;
                else
                    this.output.Indent = value;
            }
        }

        private TextWriter Output
        {
            get
            {
                if (this.cppMode == true)
                    return this.cppOutput;
                return this.output;
            }
        }

        private bool CppMode
        {
            get { return this.cppMode; }
            set { this.cppMode = value; }
        }

        private CodeGeneratorOptions Options
        {
            get { return this.options; }
        }

        private void OutputTabs()
        {
            IndentedTextWriter textWriter = this.cppMode == true ? this.cppOutput : this.output;
            for (int i = 0; i < textWriter.Indent; i++)
            {
                textWriter.Write(this.indentString);
            }
        }




        //void WriteField(string typeName, string name)
        //{
        //    this.Output.Write(GetNativeTypeName(typeName, false));
        //    this.Output.WriteLine(" {0};", name);
        //}

        //void WriteFileds(CodeTypeMemberCollection members)
        //{
        //    this.Output.WriteLine("private:");

        //    this.Indent++;
        //    foreach (CodeTypeMember item in members)
        //    {
        //        if (item is CodeMemberProperty == false)
        //            continue;

        //        CodeMemberProperty memberProperty = item as CodeMemberProperty;

        //        WriteField(memberProperty.Type.BaseType, memberProperty.Name);
        //    }
        //    this.Output.WriteLine();
        //    this.Indent--;
        //}

        //void WriteMember(string declarationName, CodeTypeMember typeMember)
        //{
        //    if (typeMember is CodeConstructor == true)
        //    {
        //        CodeConstructor c = typeMember as CodeConstructor;

        //        this.Output.Write(declarationName);
        //        this.Output.WriteLine("(){}");
        //    }
        //    else if (typeMember is CodeMemberProperty == true)
        //    {
        //        CodeMemberProperty c = typeMember as CodeMemberProperty;

        //        this.Output.Write(GetNativeTypeName(c.Type.BaseType, true));

        //        this.Output.WriteLine(" Get{0}() const {1}", c.Name, "{}");
        //    }

        //    else if (typeMember is CodeMemberField == true)
        //    {
        //        CodeMemberField c = typeMember as CodeMemberField;

        //        this.Output.Write(GetNativeTypeName(c.Type.BaseType, false));

        //        this.Output.WriteLine(" {0};", c.Name);
        //    }

        //    this.Output.WriteLine();
        //}

        //void WriteEnumMember(string declarationName, CodeMemberField typeMember)
        //{
        //    this.Output.Write(typeMember.Name);

        //    if (typeMember.InitExpression is CodePrimitiveExpression == true)
        //    {
        //        object value = (typeMember.InitExpression as CodePrimitiveExpression).Value;

        //        if (value != null)
        //        {
        //            this.Output.Write(" = ");
        //            this.Output.Write(value);
        //        }
        //    }
        //    this.Output.WriteLine(",");
        //}

        //void WriteSetPropertyMethod(CodeTypeMemberCollection members)
        //{
        //    this.Output.WriteLine("private:");
        //    this.Indent++;

        //    this.Output.WriteLine("void SetProperty(const std::string& propertyName, const std::string& text) ");
        //    this.Output.WriteLine("{");
        //    this.Indent++;

        //    int i = 0;
        //    foreach (CodeTypeMember item in members)
        //    {
        //        if (item is CodeMemberProperty == false)
        //            continue;

        //        CodeMemberProperty memberProperty = item as CodeMemberProperty;
        //        bool isEnum = false;

        //        if (memberProperty.UserData.Contains("isEnum") == true)
        //        {
        //            isEnum = (bool)memberProperty.UserData["isEnum"];
        //        }

        //        if (i != 0)
        //            this.Output.Write("else ");

        //        this.Output.WriteLine("if(propertyName == L\"{0}\")", memberProperty.Name);
        //        this.Output.WriteLine("{");
        //        this.Indent++;

        //        if (memberProperty.Type.BaseType == "System.String")
        //        {
        //            this.Output.WriteLine("this->{0} = text;", memberProperty.Name);
        //        }
        //        else if (isEnum == false)
        //        {
        //            this.Output.WriteLine("this->{0} = _wtoi(text.c_str());", memberProperty.Name);
        //        }
        //        else
        //        {
        //            string name = memberProperty.UserData["name"] as string;
        //            this.Output.WriteLine("this->{0} = {1}_Converter::ToValue(text);", memberProperty.Name, name);
        //        }
        //        this.Indent--;
        //        this.Output.WriteLine("}");

        //        i++;
        //    }
        //    this.Indent--;
        //    this.Output.WriteLine("}");

        //    this.Output.WriteLine();
        //    this.Indent--;
        //}

        //void WriteEnum(CodeTypeDeclaration e)
        //{
        //    BeinEnum(e.Name);
        //    this.Indent++;
        //    foreach (CodeTypeMember item in e.Members)
        //    {
        //        if (item is CodeMemberField == false)
        //            continue;
        //        WriteEnumMember(e.Name, item as CodeMemberField);
        //    }
        //    this.Indent--;

        //    EndDeclaration();

        //    WriteEnumConverter(e);
        //}

        //void GenerateEnumConverter(CodeTypeDeclaration e)
        //{
        //    // class eMessageType_Flags_Converter
        //    // {
        //    //        static eMessageType_Flags ToValue(std::string& text);
        //    //        static std::string& ToString(eMessageType_Flags value);
        //    //    }

        //    this.Output.WriteLine("class {0}_Converter", e.Name);
        //    this.Output.WriteLine("{");
        //    this.Output.WriteLine("public:");
        //    this.Indent++;

        //    this.Output.WriteLine("static {0} Parse(const std::string& text)", e.Name);
        //    this.Output.WriteLine("{");
        //    this.Indent++;

        //    int i = 0;
        //    foreach (CodeTypeMember item in e.Members)
        //    {
        //        if (item is CodeMemberField == false)
        //            continue;
        //        CodeMemberField field = item as CodeMemberField;

        //        if (i != 0)
        //        {
        //            this.Output.Write("else ");
        //        }

        //        this.Output.WriteLine("if(text == L\"{0}\")", field.Name);
        //        this.Indent++;
        //        this.Output.WriteLine("return {0};", field.Name);
        //        this.Indent--;

        //        i++;

        //    }
        //    this.Output.WriteLine("throw std::exception();");

        //    this.Indent--;
        //    this.Output.WriteLine("};");

        //    this.Output.WriteLine("static std::string ToString({0} value)", e.Name);
        //    this.Output.WriteLine("{");
        //    this.Indent++;

        //    i = 0;
        //    foreach (CodeTypeMember item in e.Members)
        //    {
        //        if (item is CodeMemberField == false)
        //            continue;
        //        CodeMemberField field = item as CodeMemberField;

        //        if (i != 0)
        //        {
        //            this.Output.Write("else ");
        //        }

        //        this.Output.WriteLine("if(value == {0})", field.Name);
        //        this.Indent++;
        //        this.Output.WriteLine("return L\"{0}\";", field.Name);
        //        this.Indent--;

        //        i++;
        //    }

        //    this.Output.WriteLine("throw std::exception();");

        //    this.Indent--;
        //    this.Output.WriteLine("};");


        //    this.Indent--;
        //    this.Output.WriteLine("};");
        //    this.Output.WriteLine();

        //}

        public string CreateEscapedIdentifier(string value)
        {
            return value;
        }

        public string CreateValidIdentifier(string value)
        {
            return value;
        }

        public string GetTypeOutput(CodeTypeReference typeRef)
        {
            string str = string.Empty;
            CodeTypeReference arrayElementType = typeRef;
            while (arrayElementType.ArrayElementType != null)
            {
                arrayElementType = arrayElementType.ArrayElementType;
            }

            if (typeRef.ArrayRank > 1)
                throw new NotSupportedException();

            str = str + this.GetBaseTypeOutput(arrayElementType);

            if (typeRef.ArrayRank == 1)
            {
                str = string.Format("std::vector<{0}>", str);

                if (typeRef.HasCodeType(CodeType.Pointer) == true)
                    str += "*";
                else if (typeRef.HasCodeType(CodeType.Reference) == true)
                    str += "&";

                if (typeRef.HasCodeType(CodeType.Const) == true)
                    str = "const " + str;
            }
            //while ((typeRef != null) && (typeRef.ArrayRank > 0))
            //{
            //    char[] chArray = new char[typeRef.ArrayRank + 1];
            //    chArray[0] = '[';
            //    chArray[typeRef.ArrayRank] = ']';
            //    for (int i = 1; i < typeRef.ArrayRank; i++)
            //    {
            //        chArray[i] = ',';
            //    }
            //    str = str + new string(chArray);
            //    typeRef = typeRef.ArrayElementType;
            //}
            return str;

        }

        private string GetBaseTypeOutput(CodeTypeReference typeRef)
        {
            string typeOutput = this.GetBaseTypeOutputCore(typeRef);

            if (typeRef.HasCodeType(CodeType.Pointer) == true)
                typeOutput += "*";
            else if (typeRef.HasCodeType(CodeType.Reference) == true)
                typeOutput += "&";

            if (typeRef.HasCodeType(CodeType.Const) == true)
                typeOutput = "const " + typeOutput;

            return typeOutput;
        }

        private string GetBaseTypeOutputCore(CodeTypeReference typeRef)
        {
            string baseType = typeRef.BaseType;
            if (baseType.Length == 0)
            {
                return "void";
            }
            switch (baseType.ToLower())
            {
                case "system.int16":
                    return "short";

                case "system.int32":
                    return "int";

                case "system.int64":
                    return "long long";

                case "system.string":
                    return "std::string";

                case "system.object":
                    return "void*";

                case "system.boolean":
                    return "bool";

                case "system.void":
                    return "void";

                case "system.char":
                    return "wchar_t";

                case "system.byte":
                    return "unsigned char";

                case "system.uint16":
                    return "unsigned short";

                case "system.uint32":
                    return "unsigned int";

                case "system.uint64":
                    return "unsigned long long";

                case "system.sbyte":
                    return "char";

                case "system.single":
                    return "float";

                case "system.double":
                    return "double";

                case "system.datetime":
                    return "time_t";

                case "system.timespan":
                    return "int";


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

            var text = stringBuilder.ToString().Replace(".", "::");

            if(this.cppMode == false)
            {
                if (this.preDefinedTypes.Contains(text) == false && (typeRef.HasCodeType(CodeType.Reference) || typeRef.HasCodeType(CodeType.Pointer)))
                {
                    preDefinedTypes.Add(text);
                    text = "class " + text;
                }
            }

            return text;
        }

        public bool IsValidIdentifier(string value)
        {
            throw new NotImplementedException();
        }

        public bool Supports(GeneratorSupport supports)
        {
            throw new NotImplementedException();
        }

        public void ValidateIdentifier(string value)
        {
            throw new NotImplementedException();
        }

        private void GenerateCompileUnit(CodeCompileUnit e)
        {
            this.Output.WriteLine("#pragma once");

            foreach (var item in e.GetIncludes())
            {
                this.Output.WriteLine("#include <{0}.h>", item);
            }

            foreach (var item in e.GetCustomIncludes())
            {
                this.Output.WriteLine("#include \"{0}.h\"", item);
            }

            this.Output.WriteLine();

            foreach (CodeNamespace nameSpace in e.Namespaces)
            {
                this.GenerateNamespace(nameSpace);
            }
        }

        private void GenerateCodeFromExpression(CodeExpression e)
        {
            throw new NotImplementedException();
        }

        private void GenerateNamespace(CodeNamespace e)
        {
            this.GenerateNamespaceStart(e);

            this.CppMode = true;
            this.GenerateNamespaceStart(e);
            this.CppMode = false;

            this.Indent++;
            this.GenerateTypes(e);
            this.GenerateHeaderStatements(e);
            this.Indent--;
            this.GenerateNamespaceEnd(e);



            this.CppMode = true;
            this.Indent = 1;
            this.GenerateCppStatements(e);
            this.Indent--;
            this.GenerateNamespaceEnd(e);
            this.CppMode = false;

            this.Indent--;


            StringWriter cppCode = this.cppOutput.InnerWriter as StringWriter;
            cppCode.Close();
            string c = cppCode.GetStringBuilder().ToString();

            this.Output.WriteLine("/// cpp start");
            this.Output.Write(c);
            this.Output.WriteLine("/// cpp end");
        }

        void GenerateCodeFromStatement(CodeStatement e)
        {
            throw new NotImplementedException();
        }

        private void GenerateProperties(CodeTypeDeclaration e)
        {
            this.memberAttribute = 0;
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeMemberProperty)
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeMemberProperty current = (CodeMemberProperty)enumerator.Current;
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(current.LinePragma);
                    }
                    this.GenerateProperty(current, e);
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(current.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                }
            }
        }

        void GenerateEnumRegisterMethod(CodeTypeDeclaration e)
        {
            bool isFlag = e.CustomAttributes.Contains(new CodeAttributeDeclaration(new CodeTypeReference(typeof(FlagsAttribute))));

            this.Output.WriteLine();
            this.Output.WriteLine("void RegisterEnumData_{0}()", e.Name);
            this.Output.WriteLine("{");
            this.Indent++;
            this.Output.WriteLine("static EnumData enumData({0});", isFlag.ToString().ToLower());

            this.Output.WriteLine("if(Enum::Contains(typeid({0})) == true) return;", e.Name);

            foreach (CodeTypeMember item in e.Members)
            {
                CodeMemberField field = item as CodeMemberField;
                if (field == null)
                    continue;
                this.Output.WriteLine("enumData.Add(L\"{0}\", {0});", field.Name);
            }
            this.Output.WriteLine("Enum::Add(typeid({0}), &enumData);", e.Name);
            this.Indent--;
            this.Output.WriteLine("}");
            //{
            //    static EnumData enumData(true);
            //    enumData.Add(L"eMSGTYPE_CHATLINE", eMSGTYPE_CHATLINE);
            //    enumData.Add(L"eMSGTYPE_SHORTMSG", eMSGTYPE_SHORTMSG);
            //    Enum::AddEnumData(typeid(eMessageType_Flags), &enumData);
            //}
        }

        private void GenerateTypes(CodeNamespace e)
        {
            foreach (CodeTypeDeclaration item in e.Types)
            {
                if (item.IsEnum == false)
                    continue;

                ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromType(item, this.output.InnerWriter, this.options);

                if (this.options.BlankLinesBetweenMembers)
                {
                    this.Output.WriteLine();
                }
            }

            foreach (CodeTypeDeclaration item in e.Types)
            {
                if (item.IsEnum == true)
                    continue;

                ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromType(item, this.output.InnerWriter, this.options);

                if (this.options.BlankLinesBetweenMembers)
                {
                    this.Output.WriteLine();
                }
            }
        }

        void GenerateType(CodeTypeDeclaration e)
        {
            this.typeStack.Push(e);
            this.currentClass = e;
            this.memberAttribute = 0;
            if (e.StartDirectives.Count > 0)
            {
                //this.GenerateDirectives(e.StartDirectives);
            }
            this.GenerateCommentStatements(e.Comments);
            if (e.LinePragma != null)
            {
                //this.GenerateLinePragmaStart(e.LinePragma);
            }
            this.GenerateTypeStart(e);
            if (this.Options.VerbatimOrder)
            {
                foreach (CodeTypeMember member in e.Members)
                {
                    this.GenerateTypeMember(member, e);
                }
            }
            else
            {
                this.GenerateNestedTypes(e);
                this.GenerateFields(e);
                //this.GenerateSnippetMembers(e);
                this.GenerateTypeConstructors(e);
                this.GenerateConstructors(e);
                this.GenerateDestructors(e);
                this.GenerateProperties(e);
                //this.GenerateEvents(e);
                this.GenerateMethods(e);

            }
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

            this.GenerateUserStatements(e, "statements");

            //if (e.IsEnum == true)
            //{
            //    GenerateEnumRegisterMethod(e);
            //}
            this.typeStack.Pop();
        }

        private void GenerateNestedTypes(CodeTypeDeclaration e)
        {
            this.memberAttribute = 0;
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeTypeDeclaration)
                {

                    CodeTypeDeclaration current = (CodeTypeDeclaration)enumerator.Current;
                    this.OutputMemberAccessModifier(current.Attributes);
                    ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromType(current, this.output.InnerWriter, this.options);

                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                }
            }
        }

        private void GenerateMethods(CodeTypeDeclaration e)
        {
            this.memberAttribute = 0;
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (((enumerator.Current is CodeMemberMethod) && !(enumerator.Current is CodeTypeConstructor)) && !(enumerator.Current is CodeConstructor))
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeMemberMethod current = (CodeMemberMethod)enumerator.Current;
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(current.LinePragma);
                    }
                    if (enumerator.Current is CodeEntryPointMethod)
                    {
                        throw new NotSupportedException();
                        //this.GenerateEntryPointMethod((CodeEntryPointMethod)enumerator.Current, e);
                    }
                    else
                    {
                        this.GenerateMethod(current, e);
                    }
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(current.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                }
            }
        }

        private void GenerateTypeConstructors(CodeTypeDeclaration e)
        {
            this.memberAttribute = 0;
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeTypeConstructor)
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;

                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeTypeConstructor current = (CodeTypeConstructor)enumerator.Current;
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(current.LinePragma);
                    }
                    this.GenerateTypeConstructor(current);
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(current.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                }
            }
        }

        private void GenerateTypeEnd(CodeTypeDeclaration e)
        {
            if (!this.IsCurrentDelegate)
            {
                if (e.IsClass == true && e.UserData.Contains("friend") == true)
                {
                    this.Output.WriteLine("friend class {0};", e.UserData["friend"]);
                }

                this.Indent--;
                this.Output.WriteLine("};");


            }
        }

        private void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c)
        {
            if (this.IsCurrentClass || this.IsCurrentStruct)
            {
                if (e.CustomAttributes.Count > 0)
                {
                    this.GenerateAttributes(e.CustomAttributes);
                }
                this.OutputMemberAccessModifier(e.Attributes);
                this.OutputIdentifier(c.Name);
                this.Output.Write("(");
                this.OutputParameters(e.Parameters);
                this.Output.WriteLine(");");

                {
                    this.CppMode = true;
                    this.Indent = 1;
                    this.OutputClassNamespace();
                    this.OutputIdentifier(c.Name);
                    this.Output.Write("(");
                    this.OutputParameters(e.Parameters);
                    this.Output.Write(")");
                    CodeExpressionCollection baseConstructorArgs = e.BaseConstructorArgs;
                    CodeExpressionCollection chainedConstructorArgs = e.ChainedConstructorArgs;
                    if (baseConstructorArgs.Count > 0)
                    {
                        this.Output.WriteLine();
                        this.Indent++;
                        this.Output.Write(": ");
                        this.Output.Write(string.Format("{0}(", c.BaseTypes[0].BaseType));
                        this.OutputExpressionList(baseConstructorArgs);
                        this.Output.Write(")");
                        this.Indent--;
                    }
                    if (chainedConstructorArgs.Count > 0)
                    {
                        this.Output.WriteLine();
                        this.Indent++;
                        this.Output.Write(": ");
                        this.Output.Write(c.Name);
                        this.Output.Write("(");
                        this.OutputExpressionList(chainedConstructorArgs);
                        this.Output.Write(")");
                        this.Indent--;
                    }
                    this.GenerateConstructorStatements(e);
                    this.OutputStartingBrace();
                    this.Indent++;
                    this.GenerateUserStatements(e, "statements");
                    this.GenerateStatements(e.Statements);
                    this.Indent--;
                    this.Output.WriteLine("}");
                    this.Output.WriteLine();
                    this.CppMode = false;
                }
            }
        }

        private void GenerateDestructor(CodeDestructor e, CodeTypeDeclaration c)
        {
            if (this.IsCurrentClass || this.IsCurrentStruct)
            {
                if (e.CustomAttributes.Count > 0)
                {
                    this.GenerateAttributes(e.CustomAttributes);
                }

                this.OutputMemberAccessModifier(e.Attributes);
                this.Output.Write("virtual ~");
                this.OutputIdentifier(c.Name);
                this.Output.Write("(");
                this.Output.WriteLine(");");

                {
                    this.CppMode = true;
                    this.Indent = 1;
                    this.OutputClassNamespace();
                    this.Output.Write("~");
                    this.OutputIdentifier(c.Name);
                    this.Output.Write("(");
                    this.Output.Write(")");
                    this.OutputStartingBrace();
                    this.Indent++;
                    this.GenerateStatements(e.Statements);
                    this.GenerateUserStatements(e, "statements");
                    this.Indent--;
                    this.Output.WriteLine("}");
                    this.Output.WriteLine();
                    this.CppMode = false;
                }
            }
        }

        private void OutputClassNamespace()
        {
            object[] types = this.typeStack.ToArray();
            for (int i = types.Length - 1; i >= 0; i--)
            {
                CodeTypeDeclaration type = types[i] as CodeTypeDeclaration;
                this.OutputIdentifier(type.Name);
                this.Output.Write("::");
            }
        }
        private void GenerateStatements(CodeMemberMethod codeMethod)
        {
            foreach (var item in codeMethod.GetStatements())
            {
                foreach (var line in item.Split(new string[] { Environment.NewLine, }, StringSplitOptions.None))
                {
                    this.Output.WriteLine(line);
                }

                if (this.options.BlankLinesBetweenMembers)
                {
                    this.Output.WriteLine();
                }
            }
        }

        private void GenerateHeaderStatements(CodeNamespace codeNamespace)
        {
            foreach (var item in codeNamespace.GetHeaderStatements())
            {
                foreach (var line in item.Split(new string[] { Environment.NewLine, }, StringSplitOptions.None))
                {
                    this.Output.WriteLine(line);
                }

                if (this.options.BlankLinesBetweenMembers)
                {
                    this.Output.WriteLine();
                }
            }
        }

        private void GenerateCppStatements(CodeNamespace codeNamespace)
        {
            foreach (var item in codeNamespace.GetCppStatements())
            {
                foreach (var line in item.Split(new string[] { Environment.NewLine, }, StringSplitOptions.None))
                {
                    this.Output.WriteLine(line);
                }

                if (this.options.BlankLinesBetweenMembers)
                {
                    this.Output.WriteLine();
                }
            }
        }

        private void GenerateConstructorStatements(CodeConstructor codeConstructor)
        {
            var statements = codeConstructor.GetConstructorStatements();
            if (statements.Any() == false)
                return;

            var ss = statements.Select(item => ", " + item).ToArray();

            if (codeConstructor.BaseConstructorArgs.Count == 0)
            {
                ss[0] = ": " + ss[0].Substring(2);
            }

            this.Indent++;
            foreach (var item in ss)
            {
                this.Output.WriteLine();
                this.Output.Write(item);
            }
            this.Indent--;
        }

        private void GenerateUserStatements(CodeObject codeObject, string name)
        {
            if (codeObject.UserData.Contains(name) == false)
                return;
            StringCollection stringCollection = codeObject.UserData[name] as StringCollection;
            foreach (string item in stringCollection)
            {
                this.Output.WriteLine(item);
            }
        }

        private void GenerateConstructors(CodeTypeDeclaration e)
        {
            this.memberAttribute = 0;
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeConstructor)
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeConstructor current = (CodeConstructor)enumerator.Current;
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(current.LinePragma);
                    }
                    this.GenerateConstructor(current, e);
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(current.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                }
            }
        }

        private void GenerateDestructors(CodeTypeDeclaration e)
        {
            this.memberAttribute = 0;
            IEnumerator enumerator = e.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is CodeDestructor)
                {
                    this.currentMember = (CodeTypeMember)enumerator.Current;
                    if (this.currentMember.StartDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.StartDirectives);
                    }
                    this.GenerateCommentStatements(this.currentMember.Comments);
                    CodeDestructor current = (CodeDestructor)enumerator.Current;
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaStart(current.LinePragma);
                    }
                    this.GenerateDestructor(current, e);
                    if (current.LinePragma != null)
                    {
                        this.GenerateLinePragmaEnd(current.LinePragma);
                    }
                    if (this.currentMember.EndDirectives.Count > 0)
                    {
                        this.GenerateDirectives(this.currentMember.EndDirectives);
                    }
                    if (this.options.BlankLinesBetweenMembers)
                    {
                        this.Output.WriteLine();
                    }
                }
            }
        }

        private void GenerateCommentStatement(CodeCommentStatement e)
        {
            if (e.Comment == null)
            {
                throw new Exception();
                //throw new ArgumentException(SR.GetString("Argument_NullComment", new object[] { "e" }), "e");
            }
            this.GenerateComment(e.Comment);
        }

        private void GenerateCommentStatements(CodeCommentStatementCollection e)
        {
            foreach (CodeCommentStatement statement in e)
            {
                this.GenerateCommentStatement(statement);
            }
        }

        private void GenerateLinePragmaEnd(CodeLinePragma e)
        {
            this.Output.WriteLine();
            this.Output.WriteLine("#line default");
            this.Output.WriteLine("#line hidden");
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

        private void GenerateDirectives(CodeDirectiveCollection directives)
        {
            for (int i = 0; i < directives.Count; i++)
            {
                CodeDirective directive = directives[i];
                if (directive is CodeChecksumPragma)
                {
                    throw new NotSupportedException();
                    //this.GenerateChecksumPragma((CodeChecksumPragma)directive);
                }
                else if (directive is CodeRegionDirective)
                {
                    throw new NotSupportedException();
                    //this.GenerateCodeRegionDirective((CodeRegionDirective)directive);
                }
            }
        }

        private void GenerateField(CodeMemberField e)
        {
            if (!this.IsCurrentDelegate && !this.IsCurrentInterface)
            {
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
                }
                else
                {
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
            }
        }

        private void GenerateTypeMember(CodeTypeMember member, CodeTypeDeclaration declaredType)
        {
            if (member is CodeTypeDeclaration)
            {
                ((System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromType((CodeTypeDeclaration)member, this.output.InnerWriter, this.options);
                this.currentClass = declaredType;
            }
            else
            {
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
                        throw new NotSupportedException();
                        //this.GenerateEntryPointMethod((CodeEntryPointMethod)member, declaredType);
                    }
                    else
                    {
                        this.GenerateMethod((CodeMemberMethod)member, declaredType);
                    }
                }
                else if (member is CodeMemberEvent)
                {
                    throw new NotSupportedException();
                    //this.GenerateEvent((CodeMemberEvent)member, declaredType);
                }
                else if (member is CodeSnippetTypeMember)
                {
                    throw new NotSupportedException();
                    //int indent = this.Indent;
                    //this.Indent = 0;
                    //this.GenerateSnippetMember((CodeSnippetTypeMember)member);
                    //this.Indent = indent;
                    //this.Output.WriteLine();
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
            if (this.options.BlankLinesBetweenMembers)
            {
                this.Output.WriteLine();
            }
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o)
        {
            if (this.output == null)
            {
                this.output = new IndentedTextWriter(w, o.IndentString);
            }

            this.options = o;

            this.GenerateCompileUnit(e);
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o)
        {
            if (this.output == null)
            {
                this.output = new IndentedTextWriter(w, o.IndentString);
            }

            this.options = o;

            GenerateExpression(e);
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o)
        {
            if (this.output == null)
            {
                this.output = new IndentedTextWriter(w, o.IndentString);
            }

            this.options = o;

            this.GenerateNamespace(e);
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o)
        {
            throw new NotImplementedException();
        }

        void System.CodeDom.Compiler.ICodeGenerator.GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o)
        {
            if (this.output == null)
            {
                this.output = new IndentedTextWriter(w, o.IndentString);
            }

            this.options = o;

            GenerateType(e);
        }

        class SortedCodeTypeMembers
        {

            CodeTypeMemberCollection publicMembers = new CodeTypeMemberCollection();
            CodeTypeMemberCollection protectedMembers = new CodeTypeMemberCollection();
            CodeTypeMemberCollection privateMembers = new CodeTypeMemberCollection();
            CodeTypeMemberCollection internalMembers = new CodeTypeMemberCollection();

            public SortedCodeTypeMembers(CodeTypeMemberCollection members, Type filter)
            {
                foreach (CodeTypeMember item in members)
                {
                    if (item.GetType() == filter)
                    {
                        Add(item);
                    }
                }
            }

            void Add(CodeTypeMember member)
            {
                if (member.Attributes.HasFlag(MemberAttributes.Public) == true)
                {
                    this.publicMembers.Add(member);
                }
                else if (member.Attributes.HasFlag(MemberAttributes.Family) == true)
                {
                    this.internalMembers.Add(member);
                }
                else if (member.Attributes.HasFlag(MemberAttributes.Private) == true)
                {
                    this.privateMembers.Add(member);
                }
                else
                {
                    this.protectedMembers.Add(member);
                }
            }

            public CodeTypeMemberCollection PublicMembers
            {
                get { return this.publicMembers; }
            }

            public CodeTypeMemberCollection PrivateMembers
            {
                get { return this.privateMembers; }
            }

            public CodeTypeMemberCollection ProtectedMembers
            {
                get { return this.protectedMembers; }
            }

            public CodeTypeMemberCollection InternalMembers
            {
                get { return this.internalMembers; }
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

        private CodeMemberProperty CurrentProperty
        {
            get { return this.currentProperty; }
        }

        private bool IsCurrentClass
        {
            get
            {
                return (((this.currentClass != null) && !(this.currentClass is CodeTypeDelegate)) && this.currentClass.IsClass);
            }
        }

        private bool IsCurrentDelegate
        {
            get
            {
                return ((this.currentClass != null) && (this.currentClass is CodeTypeDelegate));
            }
        }

        private bool IsCurrentEnum
        {
            get
            {
                return (((this.currentClass != null) && !(this.currentClass is CodeTypeDelegate)) && this.currentClass.IsEnum);
            }
        }

        private bool IsCurrentInterface
        {
            get
            {
                return (((this.currentClass != null) && !(this.currentClass is CodeTypeDelegate)) && this.currentClass.IsInterface);
            }
        }

        private bool IsCurrentStruct
        {
            get
            {
                return (((this.currentClass != null) && !(this.currentClass is CodeTypeDelegate)) && this.currentClass.IsStruct);
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

        //public bool IsCurrentDelegate { get; set; }

        //public bool IsCurrentInterface { get; set; }

        //public bool IsCurrentEnum { get; set; }
    }
}
