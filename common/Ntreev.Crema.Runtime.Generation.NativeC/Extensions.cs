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

using Ntreev.Crema.Data;
using Ntreev.Crema.Runtime.Generation.NativeC.CodeDom;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Generation.NativeC
{
    public static class Extensions
    {
        public static void Add(this CodeCommentStatementCollection comments, string name, object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()) == true)
                return;

            comments.Add(new CodeCommentStatement(string.Format("{0}: {1}", name, value)));
        }

        public static void Add(this CodeCommentStatementCollection comments, string name, DateTime value)
        {
            if (value == DateTime.MinValue)
                return;

            comments.Add(new CodeCommentStatement(string.Format("{0}: {1}", name, value)));
        }

        public static void AddSummary(this CodeCommentStatementCollection comments, object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()) == true)
                return;

            comments.Add(new CodeCommentStatement("<summary>", true));
            comments.Add(new CodeCommentStatement(value.ToString(), true));
            comments.Add(new CodeCommentStatement("</summary>", true));
        }

        public static void AddAssign(this CodeStatementCollection statements, CodeExpression left, CodeExpression right)
        {
            statements.Add(new CodeAssignStatement(left, right));
        }

        public static void AddAssignReference(this CodeStatementCollection statements, CodeExpression left, CodeExpression right)
        {
            statements.Add(new CodeAssignReferenceStatement(left, right));
        }

        public static void AddAssign(this CodeStatementCollection statements, CodeExpression left, string variableName)
        {
            statements.Add(new CodeAssignStatement(left, new CodeVariableReferenceExpression(variableName)));
        }

        public static void AddVariableDeclaration(this CodeStatementCollection statements, string type, string name, CodeExpression initExpression)
        {
            statements.Add(new CodeVariableDeclarationStatement(type, name, initExpression));
        }

        public static void AddExpression(this CodeStatementCollection statements, CodeExpression expression)
        {
            statements.Add(new CodeExpressionStatement(expression));
        }

        public static void AddMethodReturn(this CodeStatementCollection statements, CodeExpression expression)
        {
            statements.AddMethodReturn(expression, CodeType.None);
        }

        public static void AddMethodReturn(this CodeStatementCollection statements, CodeExpression expression, CodeType value)
        {
            var statement = new CodeMethodReturnStatement(expression);
            statement.SetCodeType(value);
            statements.Add(statement);
        }

        public static void Add(this CodeExpressionCollection expressions, string variableName)
        {
            expressions.Add(new CodeVariableReferenceExpression(variableName));
        }

        public static void Add(this CodeTypeReferenceCollection collection, string ns, string type)
        {
            collection.Add(new CodeTypeReference(string.Join(".", ns, type)));
        }

        public static void Add(this CodeTypeReferenceCollection collection, string ns, string type, params CodeTypeReference[] typeArguments)
        {
            collection.Add(new CodeTypeReference(string.Join(".", ns, type), typeArguments));
        }

        public static void Add(this CodeParameterDeclarationExpressionCollection collection, IEnumerable<ColumnInfo> columns)
        {
            foreach (var item in columns)
            {
                collection.Add(new CodeParameterDeclarationExpression(item.GetPropertyType(), item.Name));
            }
        }

        public static void Add(this CodeParameterDeclarationExpressionCollection collection, string type, string name)
        {
            collection.Add(new CodeParameterDeclarationExpression(type, name));
        }

        public static void Add(this CodeParameterDeclarationExpressionCollection collection, string ns, string type, string name)
        {
            collection.Add(new CodeParameterDeclarationExpression(string.Join(".", ns, type), name));
        }

        public static void Add(this CodeParameterDeclarationExpressionCollection collection, Type type, string name)
        {
            collection.Add(new CodeParameterDeclarationExpression(type, name));
        }

        public static void Add(this CodeParameterDeclarationExpressionCollection collection, CodeTypeReference type, string name)
        {
            collection.Add(new CodeParameterDeclarationExpression(type, name));
        }

        public static void Add(this CodeParameterDeclarationExpressionCollection collection, CodeTypeReference type, int rank, string name)
        {
            var codeTypeRef = new CodeTypeReference(type, rank);
            collection.Add(new CodeParameterDeclarationExpression(codeTypeRef, name));
        }

        public static void Add(this CodeAttributeDeclarationCollection collection, Type type)
        {
            collection.Add(new CodeAttributeDeclaration(new CodeTypeReference(type)));
        }
    }
}
