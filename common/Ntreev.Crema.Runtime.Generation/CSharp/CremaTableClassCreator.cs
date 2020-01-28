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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Data.Xml.Schema;
using System.Reflection;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Runtime.Generation.CSharp
{
    static class CremaTableClassCreator
    {
        private static readonly CodeThisReferenceExpression thisRef = new CodeThisReferenceExpression();
        private static readonly CodeBaseReferenceExpression baseRef = new CodeBaseReferenceExpression();

        public static void Create(CodeNamespace codeNamespace, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.Tables)
            {
                if (item.TemplatedParent != string.Empty)
                    continue;

                Create(codeNamespace, item, generationInfo);
            }
        }

        public static void Create(CodeNamespace codeNamespace, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                if (item.TemplatedParent != string.Empty)
                    continue;

                Create(codeNamespace, item, generationInfo);
            }

            CremaRowClassCreator.Create(codeNamespace, tableInfo, generationInfo);
            var classType = CreateCore(tableInfo, generationInfo);
            codeNamespace.Types.Add(classType);
        }

        private static CodeTypeDeclaration CreateCore(TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var classType = new CodeTypeDeclaration()
            {
                Attributes = MemberAttributes.Public,
                Name = tableInfo.GetClassName(),
                IsClass = true
            };
            classType.TypeAttributes |= TypeAttributes.Sealed;
            classType.BaseTypes.Add(generationInfo.BaseNamespace, "CremaTable", tableInfo.GetRowCodeType());
            if (generationInfo.OmitComment == false)
            {
                classType.Comments.AddSummary(tableInfo.Comment);
            }
            if (generationInfo.OmitSignatureDate == false)
            {
                classType.Comments.Add(CremaSchema.Creator, tableInfo.CreationInfo.ID);
                classType.Comments.Add(CremaSchema.CreatedDateTime, tableInfo.CreationInfo.DateTime);
                classType.Comments.Add(CremaSchema.Modifier, tableInfo.ModificationInfo.ID);
                classType.Comments.Add(CremaSchema.ModifiedDateTime, tableInfo.ModificationInfo.DateTime);
                classType.Comments.Add(CremaSchema.ContentsModifier, tableInfo.ContentsInfo.ID);
                classType.Comments.Add(CremaSchema.ContentsModifiedDateTime, tableInfo.ContentsInfo.DateTime);
            }

            CreateChildFields(classType, tableInfo, generationInfo);
            CreateChildProperties(classType, tableInfo, generationInfo);
            CreateConstructor(classType, tableInfo, generationInfo);
            CreateConstructorFromTable(classType, tableInfo, generationInfo);
            CreateConstructorFromRows(classType, tableInfo);
            CreateFindMethod(classType, tableInfo, generationInfo);
            CreateCreateRowInstanceMethod(classType, tableInfo, generationInfo);

            return classType;
        }

        private static void CreateChildFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmf = new CodeMemberField()
                {
                    Attributes = MemberAttributes.Private,
                    Name = item.GetFieldName(),
                    Type = item.GetCodeType()
                };
                classType.Members.Add(cmf);
            }
        }

        private static void CreateConstructor(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };
            classType.Members.Add(cc);
        }

        private static void CreateConstructorFromTable(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };
            cc.Parameters.Add(generationInfo.ReaderNamespace, "ITable", "table");
            cc.BaseConstructorArgs.Add("table");

            // verify revision
            {
                var css = CreateCompareTypeStatement(classType, tableInfo, generationInfo);
                var tst = CremaDataClassCreator.CreateTryCatchStatement(classType, css, generationInfo);
                cc.Statements.Add(tst);
            }

            // initialize childs
            {
                var table = new CodeVariableReferenceExpression("table");
                var dataSet = new CodePropertyReferenceExpression(table, "DataSet");
                var tables = new CodePropertyReferenceExpression(dataSet, "Tables");

                foreach (var item in generationInfo.GetChilds(tableInfo))
                {
                    var tableName = new CodePropertyReferenceExpression(table, "Name");
                    var childName = new CodeBinaryOperatorExpression(tableName, CodeBinaryOperatorType.Add, new CodePrimitiveExpression("." + item.TableName));
                    var childTable = new CodeIndexerExpression(tables, childName);
                    var childField = item.GetFieldExpression();
                    var instance = new CodeObjectCreateExpression(item.GetCodeType(), childTable);

                    cc.Statements.AddAssign(childField, instance);
                }
            }

            // SetRelations
            {
                var table = new CodeVariableReferenceExpression("table");
                foreach (var item in generationInfo.GetChilds(tableInfo))
                {
                    var setRelations = new CodeMethodReferenceExpression(thisRef, "SetRelations");
                    var rows = new CodePropertyReferenceExpression(item.GetFieldExpression(), "Rows");
                    var setChildAction = new CodePropertyReferenceExpression(tableInfo.GetRowCodeTypeExpression(), "Set" + item.TableName);
                    var tableName = new CodePropertyReferenceExpression(table, "Name");
                    var childName = new CodeBinaryOperatorExpression(tableName, CodeBinaryOperatorType.Add, new CodePrimitiveExpression("." + item.TableName));

                    var setRelationsInvoke = new CodeMethodInvokeExpression(setRelations, childName, rows, setChildAction);
                    cc.Statements.Add(setRelationsInvoke);
                }
            }

            classType.Members.Add(cc);
        }

        private static void CreateConstructorFromRows(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            if (string.IsNullOrEmpty(tableInfo.ParentName) == true)
                return;

            var cc = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };
            cc.Parameters.Add(typeof(string), "name");
            cc.Parameters.Add(tableInfo.GetRowCodeType(), 1, "rows");
            cc.BaseConstructorArgs.Add("name");
            cc.BaseConstructorArgs.Add("rows");

            classType.Members.Add(cc);
        }

        private static void CreateFindMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Find",
                ReturnType = tableInfo.GetRowCodeType()
            };
            cmm.Parameters.Add(tableInfo.Columns.Where(item => item.IsKey));

            // invoke base.FindRow
            {
                var query = from item in tableInfo.Columns
                    where item.IsKey
                    select new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression(item.Name), "ToString"));

                var invokeGenerateHashCode = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("CremaUtility"), "GenerateHashCode");
                invokeGenerateHashCode.Parameters.AddRange(query.ToArray());
                var invokeFindRow = new CodeMethodInvokeExpression(baseRef, "FindRow", invokeGenerateHashCode);

                cmm.Statements.AddMethodReturn(invokeFindRow);
            }

            classType.Members.Add(cmm);
        }

        private static void CreateChildProperties(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmp = new CodeMemberProperty()
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    Name = item.GetPropertyName(),
                    Type = item.GetCodeType(),
                    HasGet = true,
                    HasSet = false
                };
                var fieldExp = item.GetFieldExpression();
                cmp.GetStatements.AddMethodReturn(fieldExp);
                cmp.Comments.AddSummary(item.Comment);

                classType.Members.Add(cmp);
            }
        }

        private static void CreateCreateRowInstanceMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Family | MemberAttributes.Override,
                Name = "CreateRowInstance",
                ReturnType = tableInfo.GetRowCodeType()
            };
            cmm.Parameters.Add(generationInfo.ReaderNamespace, "IRow", "row");
            cmm.Parameters.Add(typeof(object), "table");

            // statement
            {
                var row = new CodeVariableReferenceExpression("row");
                var tablePtr = new CodeVariableReferenceExpression("table");
                var table = new CodeCastExpression(tableInfo.GetCodeType(), tablePtr);
                var instance = new CodeObjectCreateExpression(tableInfo.GetRowCodeType(), row, table);

                cmm.Statements.AddMethodReturn(instance);
            }

            classType.Members.Add(cmm);
        }

        private static CodeStatement CreateCompareTypeStatement(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var table = new CodeVariableReferenceExpression("table");
            var hashValue = new CodePropertyReferenceExpression(table, "HashValue");

            var state = new CodeConditionStatement()
            {
                Condition = new CodeBinaryOperatorExpression(hashValue, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(tableInfo.HashValue))
            };
            var message = string.Format("{0} 테이블과 데이터의 형식이 맞지 않습니다.", tableInfo.Name);
            var exception = new CodeObjectCreateExpression(typeof(Exception), new CodePrimitiveExpression(message));
            state.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return state;
        }
    }
}
