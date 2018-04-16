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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Generation.NativeC
{
    public static class CodeDomExtensions
    {
        private const string key_includes = "includes";
        private const string key_customIncludes = "custom_includes";
        private const string key_const = "const";
        private const string key_pointer = "pointer";
        private const string key_codeType = "codeType";

        private const string key_statements = "statements";
        private const string key_headerStatements = "header_statements";
        private const string key_cppStatements = "cpp_statements";
        private const string key_constructorStatements = "constructor_statements";

        public static void AddInclude(this CodeCompileUnit compileUnit, string filename)
        {
            if (compileUnit.UserData.Contains(key_includes) == false)
            {
                compileUnit.UserData[key_includes] = new List<string>();
            }

            List<string> includes = compileUnit.UserData[key_includes] as List<string>;
            includes.Add(filename);
        }

        public static void AddCustomInclude(this CodeCompileUnit compileUnit, string filename)
        {
            if (compileUnit.UserData.Contains(key_customIncludes) == false)
            {
                compileUnit.UserData[key_customIncludes] = new List<string>();
            }

            List<string> includes = compileUnit.UserData[key_customIncludes] as List<string>;
            includes.Add(filename);
        }

        public static IEnumerable<string> GetIncludes(this CodeCompileUnit compileUnit)
        {
            if (compileUnit.UserData.Contains(key_includes) == false)
                return Enumerable.Empty<string>();
            return compileUnit.UserData[key_includes] as List<string>;
        }

        public static IEnumerable<string> GetCustomIncludes(this CodeCompileUnit compileUnit)
        {
            if (compileUnit.UserData.Contains(key_customIncludes) == false)
                return Enumerable.Empty<string>();
            return compileUnit.UserData[key_customIncludes] as List<string>;
        }

        public static bool IsConst(this CodeMemberMethod codeMember)
        {
            if (codeMember.UserData.Contains(key_const) == false)
                return false;
            return (bool)codeMember.UserData[key_const];
        }

        public static void IsConst(this CodeMemberMethod codeMember, bool value)
        {
            codeMember.UserData[key_const] = value;
        }

        public static bool HasCodeType(this CodeTypeReference codeTypeReference, CodeType codeType)
        {
            if (codeTypeReference.UserData.Contains(key_codeType) == false)
                return false;
            return ((CodeType)codeTypeReference.UserData[key_codeType]).HasFlag(codeType);
        }

        public static void SetCodeType(this CodeTypeReference codeTypeReference, CodeType value)
        {
            codeTypeReference.UserData[key_codeType] = value;
        }

        public static bool HasCodeType(this CodeMethodReturnStatement statement, CodeType codeType)
        {
            if (statement.UserData.Contains(key_codeType) == false)
                return false;
            return ((CodeType)statement.UserData[key_codeType]).HasFlag(codeType);
        }

        public static void SetCodeType(this CodeMethodReturnStatement statement, CodeType value)
        {
            statement.UserData[key_codeType] = value;
        }

        public static bool HasCodeType(this CodeAssignStatement statement, CodeType codeType)
        {
            if (statement.UserData.Contains(key_codeType) == false)
                return false;
            return ((CodeType)statement.UserData[key_codeType]).HasFlag(codeType);
        }

        public static void SetCodeType(this CodeAssignStatement statement, CodeType value)
        {
            statement.UserData[key_codeType] = value;
        }

        public static void AddHeaderStatement(this CodeNamespace codeNamespace, string statement)
        {
            if (codeNamespace.UserData.Contains(key_headerStatements) == false)
            {
                codeNamespace.UserData[key_headerStatements] = new List<string>();
            }

            List<string> includes = codeNamespace.UserData[key_headerStatements] as List<string>;
            includes.Add(statement);
        }

        public static IEnumerable<string> GetHeaderStatements(this CodeNamespace codeNamespace)
        {
            if (codeNamespace.UserData.Contains(key_headerStatements) == false)
                return Enumerable.Empty<string>();
            return codeNamespace.UserData[key_headerStatements] as List<string>;
        }

        public static void AddCppStatement(this CodeNamespace codeNamespace, string statement)
        {
            if (codeNamespace.UserData.Contains(key_cppStatements) == false)
            {
                codeNamespace.UserData[key_cppStatements] = new List<string>();
            }

            List<string> includes = codeNamespace.UserData[key_cppStatements] as List<string>;
            includes.Add(statement);
        }

        public static IEnumerable<string> GetCppStatements(this CodeNamespace codeNamespace)
        {
            if (codeNamespace.UserData.Contains(key_cppStatements) == false)
                return Enumerable.Empty<string>();
            return codeNamespace.UserData[key_cppStatements] as List<string>;
        }

        public static void AddConstructorStatement(this CodeConstructor codeConstructor, string statement)
        {
            if (codeConstructor.UserData.Contains(key_constructorStatements) == false)
            {
                codeConstructor.UserData[key_constructorStatements] = new List<string>();
            }

            List<string> includes = codeConstructor.UserData[key_constructorStatements] as List<string>;
            includes.Add(statement);
        }

        public static IEnumerable<string> GetConstructorStatements(this CodeConstructor codeConstructor)
        {
            if (codeConstructor.UserData.Contains(key_constructorStatements) == false)
                return Enumerable.Empty<string>();
            return codeConstructor.UserData[key_constructorStatements] as List<string>;
        }

        public static IEnumerable<string> GetStatements(this CodeMemberMethod codeMethod)
        {
            if (codeMethod.UserData.Contains(key_statements) == false)
                return Enumerable.Empty<string>();
            return codeMethod.UserData[key_statements] as List<string>;
        }

        public static void AddStatement(this CodeMemberMethod codeMethod, string statement)
        {
            if (codeMethod.UserData.Contains(key_statements) == false)
            {
                codeMethod.UserData[key_statements] = new List<string>();
            }

            List<string> includes = codeMethod.UserData[key_statements] as List<string>;
            includes.Add(statement);
        }
    }
}
