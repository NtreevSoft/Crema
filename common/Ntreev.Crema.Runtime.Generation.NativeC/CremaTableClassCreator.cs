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
using System.Collections.Specialized;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Runtime.Generation.NativeC.CodeDom;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Runtime.Generation.NativeC
{
    static class CremaTableClassCreator
    {
        private readonly static CodeThisReferenceExpression thisRef = new CodeThisReferenceExpression();

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

            classType.Name = tableInfo.GetClassName();
            classType.IsClass = true;
            classType.BaseTypes.Add(generationInfo.BaseNamespace, "CremaTable", tableInfo.GetRowCodeType(CodeType.None));
            if (generationInfo.NoComment == false)
            {
                classType.Comments.AddSummary(tableInfo.Comment);
            }

            if (generationInfo.NoChanges == false)
            {
                classType.Comments.Add(CremaSchema.Creator, tableInfo.CreationInfo.ID);
                classType.Comments.Add(CremaSchema.CreatedDateTime, tableInfo.CreationInfo.DateTime);
                classType.Comments.Add(CremaSchema.Modifier, tableInfo.ModificationInfo.ID);
                classType.Comments.Add(CremaSchema.ModifiedDateTime, tableInfo.ModificationInfo.DateTime);
                classType.Comments.Add(CremaSchema.ContentsModifier, tableInfo.ContentsInfo.ID);
                classType.Comments.Add(CremaSchema.ContentsModifiedDateTime, tableInfo.ContentsInfo.DateTime);
            }

            CreateCreateRowInstanceMethod(classType, tableInfo, generationInfo);
            CreateFields(classType, tableInfo, generationInfo);
            CreateConstructor(classType, tableInfo, generationInfo);
            CreateConstructorFromTable(classType, tableInfo, generationInfo);
            CreateConstructorFromRows(classType, tableInfo);
            CreateDestructor(classType, tableInfo, generationInfo);
            CreateFindMethod(classType, tableInfo, generationInfo);

            return classType;
        }

        private static void CreateFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Public;
                cmf.Name = item.TableName;
                cmf.Type = item.GetCodeType(CodeType.Pointer | CodeType.Const);
                classType.Members.Add(cmf);
            }
        }

        private static void CreateConstructor(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;
            classType.Members.Add(cc);
        }

        private static void CreateConstructorFromTable(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;
            cc.Name = tableInfo.TableName;
            cc.Parameters.Add(generationInfo.ReaderNamespace, "itable&", "table");
            //cc.BaseConstructorArgs.Add("table");

            //cc.AddConstructorStatement(string.Format("{0}(table)", CodeGenerationInfo.CremaTableName));

            // verify hashValue
            {
                var css = CreateCompareTypeStatement(classType, tableInfo, generationInfo);
                var tst = CremaDataClassCreator.CreateTryCatchStatement(classType, css, generationInfo);
                cc.Statements.Add(css);
            }

            {
                var table = new CodeVariableReferenceExpression("table");
                var dataSet = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(table, "dataset"));
                var tables = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(dataSet, "tables"));

                foreach (var item in generationInfo.GetChilds(tableInfo))
                {
                    var tableName = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(table, "name"));
                    var childName = new CodeBinaryOperatorExpression(tableName, CodeBinaryOperatorType.Add, new CodePrimitiveExpression("." + item.TableName));
                    var childTable = new CodeIndexerExpression(tables, childName);
                    var childField = new CodeFieldReferenceExpression(thisRef, item.TableName);
                    var instance = new CodeObjectCreateExpression(item.GetCodeType(CodeType.None), childTable);

                    cc.Statements.AddAssign(childField, instance);
                    //cc.AddConstructorStatement(string.Format("{0}(table.dataset().tables()[\"{1}\"])", item.GetFieldName(), item.GetUniqueName()));
                }
            }


            {
                var table = new CodeVariableReferenceExpression("table");
                var readFromFile = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "ReadFromTable");
                var methodInvoke = new CodeMethodInvokeExpression(readFromFile, table);

                cc.Statements.Add(methodInvoke);
            }

            // SetRelations
            {
                var table = new CodeVariableReferenceExpression("table");
                foreach (var item in generationInfo.GetChilds(tableInfo))
                {
                    var setRelations = new CodeMethodReferenceExpression(thisRef, "SetRelations");
                    var field = new CodeFieldPointerExpression(thisRef, item.TableName);
                    var rows = new CodePropertyReferenceExpression(field, "Rows");
                    var codeTypeRef = new CodeTypeReferenceExpression(tableInfo.GetRowCodeType(CodeType.None));
                    var setChildAction = new CodePropertyReferenceExpression(null, tableInfo.TableName + "Set" + item.TableName);
                    var tableName = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(table, "name"));
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

            var cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;

            var codeTypeRef = tableInfo.GetRowCodeType(CodeType.Pointer);
            var arrayTypeRef = new CodeTypeReference(codeTypeRef, 1);
            arrayTypeRef.SetCodeType(CodeType.Const | CodeType.Reference);
            cc.Parameters.Add(typeof(string), "name");
            cc.Parameters.Add(arrayTypeRef, 1, "rows");
            //cc.BaseConstructorArgs.Add("rows");

            {
                var name = new CodeVariableReferenceExpression("name");
                var rows = new CodeVariableReferenceExpression("rows");
                var readFromRows = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "ReadFromRows");
                var methodInvoke = new CodeMethodInvokeExpression(readFromRows, name, rows);

                cc.Statements.Add(methodInvoke);
            }

            classType.Members.Add(cc);
        }

        private static void CreateDestructor(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeDestructor();
            cc.Attributes = MemberAttributes.Public | MemberAttributes.Override;

            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var field = new CodeFieldReferenceExpression(thisRef, item.TableName);
                var delete = new CodeObjectDeleteExpression(field);

                cc.Statements.Add(delete);
            }

            classType.Members.Add(cc);
        }

        private static void CreateFindMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "Find";
            cmm.ReturnType = tableInfo.GetRowCodeType(CodeType.Pointer | CodeType.Const);
            cmm.Parameters.Add(tableInfo.Columns.Where(item => item.IsKey));
            cmm.IsConst(true);

            // invoke base.FindRow
            {
                var query = from item in tableInfo.Columns
                            where item.IsKey
                            select new CodeVariableReferenceExpression(item.Name);

                var invokeFindRow = new CodeMethodInvokeExpression(thisRef, "FindRow", query.ToArray());

                cmm.Statements.AddMethodReturn(invokeFindRow);
            }

            classType.Members.Add(cmm);
        }

        private static void CreateCreateRowInstanceMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            cmm.Name = "CreateRow";
            cmm.ReturnType = new CodeTypeReference(typeof(object));
            cmm.Parameters.Add(generationInfo.ReaderNamespace, "irow&", "row");
            cmm.Parameters.Add(typeof(object), "table");

            // statement
            {
                var row = new CodeVariableReferenceExpression("row");
                var tablePtr = new CodeVariableReferenceExpression("table");
                var table = new CodeCastExpression(tableInfo.GetCodeType(CodeType.Pointer), tablePtr);
                var instance = new CodeObjectCreateExpression(tableInfo.GetRowCodeType(CodeType.None), row, table);

                cmm.Statements.AddMethodReturn(instance);
            }

            classType.Members.Add(cmm);
        }

        private static CodeStatement CreateCompareTypeStatement(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var table = new CodeVariableReferenceExpression("table");
            var version = new CodeMethodInvokeExpression(table, "hash_value");

            var state = new CodeConditionStatement();
            state.Condition = new CodeBinaryOperatorExpression(version, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(tableInfo.HashValue));

            var message = string.Format("{0} 테이블과 데이터의 형식이 맞지 않습니다.", tableInfo.Name);
            var exception = new CodeObjectCreateExpression("std::exception", new CodePrimitiveExpression(message));
            state.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return state;
        }
    }
}
