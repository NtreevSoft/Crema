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
using Ntreev.Crema.Runtime.Generation.TypeScript.Properties;
using Ntreev.Crema.Data.Xml.Schema;
using System.Reflection;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Runtime.Generation.TypeScript
{
    static class CremaTableClassCreator
    {
        private readonly static CodeThisReferenceExpression thisRef = new CodeThisReferenceExpression();
        private readonly static CodeBaseReferenceExpression baseRef = new CodeBaseReferenceExpression();

        public static void Create(CodeNamespace codeNamespace, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetTables())
            {
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
            var classType = new CodeTypeDeclaration();
            classType.Attributes = MemberAttributes.Public;
            classType.Name = tableInfo.GetClassName();
            classType.IsClass = true;
            classType.TypeAttributes |= TypeAttributes.Sealed;
            classType.BaseTypes.Add("base", "CremaTable", tableInfo.GetRowCodeType());
            if (generationInfo.OmitComment == false)
            {
                classType.Comments.AddSummary(tableInfo.Comment);
            }
            if (generationInfo.OmitSignatureDate == false)
            {
                classType.Comments.Add(CremaCodeGenerator.Creator, tableInfo.CreationInfo.ID);
                classType.Comments.Add(CremaCodeGenerator.CreatedDateTime, tableInfo.CreationInfo.DateTime);
                classType.Comments.Add(CremaCodeGenerator.Modifier, tableInfo.ModificationInfo.ID);
                classType.Comments.Add(CremaCodeGenerator.ModifiedDateTime, tableInfo.ModificationInfo.DateTime);
                classType.Comments.Add(CremaCodeGenerator.ContentsModifier, tableInfo.ContentsInfo.ID);
                classType.Comments.Add(CremaCodeGenerator.ContentsModifiedDateTime, tableInfo.ContentsInfo.DateTime);
            }

            CreateChildFields(classType, tableInfo, generationInfo);
            CreateChildProperties(classType, tableInfo, generationInfo);
            CreateCreateFromTableMethod(classType, tableInfo, generationInfo);
            CreateCreateFromRowsMethod(classType, tableInfo, generationInfo);
            
            CreateReadFromRowsMethod(classType, tableInfo);
            CreateFindMethod(classType, tableInfo, generationInfo);
            CreateCreateRowInstanceMethod(classType, tableInfo, generationInfo);
            CreateReadFromTableMethod(classType, tableInfo, generationInfo);

            return classType;
        }

        private static void CreateChildFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Private;
                cmf.Name = item.GetFieldName();
                cmf.Type = item.GetCodeType();

                classType.Members.Add(cmf);
            }
        }

        private static void CreateChildProperties(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmp = new CodeMemberProperty();
                cmp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                cmp.Name = item.TableName;
                cmp.Type = item.GetCodeType();
                cmp.HasGet = true;
                cmp.HasSet = false;

                // return field
                {
                    var fieldExp = item.GetFieldExpression();
                    cmp.GetStatements.AddMethodReturn(fieldExp);
                }
                cmp.Comments.AddSummary(item.Comment);

                classType.Members.Add(cmp);
            }
        }

        private static void CreateCreateFromTableMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            cmm.Name = "createFromTable";
            cmm.Parameters.Add("reader", "ITable", "table");
            cmm.ReturnType = tableInfo.GetCodeType();

            // verify hashValue
            {
                var css = CreateCompareTypeStatement(classType, tableInfo, generationInfo);
                var tst = CremaDataClassCreator.CreateTryCatchStatement(classType, css, generationInfo.IsDevmode);
                cmm.Statements.Add(css);
            }

            // create instance;
            {
                var create = new CodeObjectCreateExpression(tableInfo.GetCodeType());
                cmm.Statements.AddVariableDeclaration(tableInfo.GetCodeType().BaseType, "instance", create);
            }

            // invoke readFromFile
            {
                var instance = new CodeVariableReferenceExpression("instance");
                var table = new CodeVariableReferenceExpression("table");
                var readFromTable = new CodeMethodReferenceExpression(instance, "readFromTable");
                cmm.Statements.Add(new CodeMethodInvokeExpression(readFromTable, table));
            }

            // return instance;
            {
                var instance = new CodeVariableReferenceExpression("instance");
                cmm.Statements.AddMethodReturn(instance);
            }

            classType.Members.Add(cmm);
        }

        private static void CreateCreateFromRowsMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            cmm.Name = "createFromRows";
            cmm.Parameters.Add(typeof(string), "name");
            cmm.Parameters.Add(tableInfo.GetRowCodeType(), 1, "rows");
            cmm.ReturnType = tableInfo.GetCodeType();

            // create instance;
            {
                var create = new CodeObjectCreateExpression(tableInfo.GetCodeType());
                cmm.Statements.AddVariableDeclaration(tableInfo.GetCodeType().BaseType, "instance", create);
            }

            // invoke readFromFile
            {
                var instance = new CodeVariableReferenceExpression("instance");
                var name = new CodeVariableReferenceExpression("name");
                var rows = new CodeVariableReferenceExpression("rows");
                var readFromRows = new CodeMethodReferenceExpression(instance, "readFromRows");
                cmm.Statements.Add(new CodeMethodInvokeExpression(readFromRows, name, rows));
            }

            // return instance;
            {
                var instance = new CodeVariableReferenceExpression("instance");
                cmm.Statements.AddMethodReturn(instance);
            }

            classType.Members.Add(cmm);
        }

        private static void CreateReadFromTableMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Family;
            cmm.Name = "readFromTable";
            cmm.Parameters.Add("reader", "ITable", "table");

            // invoke super.readFromFile
            {
                var table = new CodeVariableReferenceExpression("table");
                var readFromTable = new CodeMethodReferenceExpression(new CodeBaseReferenceExpression(), "readFromTable");
                cmm.Statements.Add(new CodeMethodInvokeExpression(readFromTable, table));
            }

            // initialize childs
            {
                var table = new CodeVariableReferenceExpression("table");
                var dataSet = new CodePropertyReferenceExpression(table, "dataSet");
                var tables = new CodePropertyReferenceExpression(dataSet, "tables");

                foreach (var item in generationInfo.GetChilds(tableInfo))
                {
                    var tableName = new CodePropertyReferenceExpression(table, "name");
                    var childName = new CodeBinaryOperatorExpression(tableName, CodeBinaryOperatorType.Add, new CodePrimitiveExpression("." + item.TableName));
                    var childTable = new CodeIndexerExpression(tables, childName);
                    var childField = item.GetFieldExpression();
                    var createFromTable = new CodeMethodReferenceExpression(item.GetCodeTypeExpression(), "createFromTable");
                    var createFromTableInvoke = new CodeMethodInvokeExpression(createFromTable, childTable);

                    cmm.Statements.AddAssign(childField, createFromTableInvoke);
                }
            }

            // setRelations
            {
                var table = new CodeVariableReferenceExpression("table");
                foreach (var item in generationInfo.GetChilds(tableInfo))
                {
                    var setRelations = new CodeMethodReferenceExpression(thisRef, "SetRelations");
                    var rows = new CodePropertyReferenceExpression(item.GetFieldExpression(), "rows");
                    var setChildAction = new CodePropertyReferenceExpression(tableInfo.GetRowCodeTypeExpression(), "set" + item.TableName);
                    var tableName = new CodePropertyReferenceExpression(table, "name");
                    var childName = new CodeBinaryOperatorExpression(tableName, CodeBinaryOperatorType.Add, new CodePrimitiveExpression("." + item.TableName));

                    var setRelationsInvoke = new CodeMethodInvokeExpression(setRelations, childName, rows, setChildAction);
                    cmm.Statements.Add(setRelationsInvoke);
                }
            }

            classType.Members.Add(cmm);
        }

        private static void CreateReadFromRowsMethod(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            if (string.IsNullOrEmpty(tableInfo.ParentName) == true)
                return;

            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Family;
            cmm.Name = "readFromRows";
            cmm.Parameters.Add(typeof(string), "name");
            cmm.Parameters.Add(tableInfo.GetRowCodeType(), 1, "rows");

            // invoke super.readFromRows
            {
                var name = new CodeVariableReferenceExpression("name");
                var rows = new CodeVariableReferenceExpression("rows");
                var readFromTable = new CodeMethodReferenceExpression(new CodeBaseReferenceExpression(), "readFromRows");
                cmm.Statements.Add(new CodeMethodInvokeExpression(readFromTable, name, rows));
            }

            classType.Members.Add(cmm);
        }

        private static void CreateFindMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "find";
            cmm.ReturnType = tableInfo.GetRowCodeType();
            cmm.Parameters.Add(tableInfo.Columns.Where(item => item.IsKey));

            // invoke super.findRow
            {
                var query = from item in tableInfo.Columns
                            where item.IsKey
                            select new CodeVariableReferenceExpression(item.Name);

                var invokeFindRow = new CodeMethodInvokeExpression(baseRef, "findRow", query.ToArray());

                cmm.Statements.AddMethodReturn(invokeFindRow);
            }

            classType.Members.Add(cmm);
        }

        private static void CreateCreateRowInstanceMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            cmm.Name = "CreateRowInstance";
            cmm.ReturnType = tableInfo.GetRowCodeType();
            cmm.Parameters.Add("reader", "IRow", "row");
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
            var version = new CodePropertyReferenceExpression(table, "hashValue");

            var state = new CodeConditionStatement();
            state.Condition = new CodeBinaryOperatorExpression(version, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(tableInfo.HashValue));

            var message = string.Format("{0} 테이블과 데이터의 형식이 맞지 않습니다.", tableInfo.Name);
            var exception = new CodeObjectCreateExpression("Error", new CodePrimitiveExpression(message));
            state.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return state;
        }
    }
}
