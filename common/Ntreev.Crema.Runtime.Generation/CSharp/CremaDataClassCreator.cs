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
using System.Reflection;
using System.IO;

namespace Ntreev.Crema.Runtime.Generation.CSharp
{
    static class CremaDataClassCreator
    {
        public static void Create(CodeNamespace codeNamespace, CodeGenerationInfo generationInfo)
        {
            codeNamespace.Comments.Add(new CodeCommentStatement($"------------------------------------------------------------------------------"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"dataBase: {generationInfo.DataBaseName}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"revision: {generationInfo.Revision}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"requested revision: {generationInfo.RequestedRevision}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"devmode: {generationInfo.IsDevmode}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"hash value: {generationInfo.TablesHashValue}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"tags: {generationInfo.Tags}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"------------------------------------------------------------------------------"));

            foreach (var item in generationInfo.GetTables())
            {
                CremaTableClassCreator.Create(codeNamespace, item, generationInfo);
            }

            var classType = new CodeTypeDeclaration()
            {
                Attributes = MemberAttributes.Public,
                Name = generationInfo.ClassName,
                IsClass = true
            };
            classType.TypeAttributes |= TypeAttributes.Sealed;
            classType.BaseTypes.Add(generationInfo.BaseNamespace, "CremaData");

            CreateNameField(classType, generationInfo);
            CreateRevisionField(classType, generationInfo);
            CreateTypesHashValueField(classType, generationInfo);
            CreateTablesHashValueField(classType, generationInfo);
            CreateTagsField(classType, generationInfo);
            CreateNameProperty(classType, generationInfo);
            CreateRevisionProperty(classType, generationInfo);
            CreateTypesHashValueProperty(classType, generationInfo);
            CreateTablesHashValueProperty(classType, generationInfo);
            CreateTagsProperty(classType, generationInfo);
            CreateTableFields(classType, generationInfo);
            CreateConstructor(classType, generationInfo);
            CreateConstructorFromFile(classType, generationInfo);
            CreateConstructorFromStream(classType, generationInfo);
            CreateLoadFromFile(classType, generationInfo);
            CreateLoadStream(classType, generationInfo);
            CreateLoad(classType, generationInfo);
            CreateTableProperties(classType, generationInfo);

            codeNamespace.Types.Add(classType);
        }

        private static void CreateConstructor(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };
            cc.Parameters.Add("IDataSet", "dataSet");
            cc.Parameters.Add(new CodeTypeReference(typeof(bool)), "verifyRevision");

            //cc.BaseConstructorArgs.Add(new CodePrimitiveExpression(generationInfo.DataBaseName));
            //cc.BaseConstructorArgs.Add(new CodePrimitiveExpression(generationInfo.Revision));

            var methodInvokeExp = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Load", new CodeVariableReferenceExpression("dataSet"), new CodeVariableReferenceExpression("verifyRevision"));
            cc.Statements.Add(methodInvokeExp);

            classType.Members.Add(cc);
        }

        private static void CreateConstructorFromFile(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };
            cc.Parameters.Add(typeof(string), "filename");
            cc.Parameters.Add(new CodeTypeReference(typeof(bool)), "verifyRevision");

            var readerRef = new CodeTypeReferenceExpression(string.Join(".", generationInfo.ReaderNamespace, "CremaReader"));
            var paramExp = new CodeVariableReferenceExpression("filename");
            var methodInvokeExp = new CodeMethodInvokeExpression(readerRef, "Read", paramExp);

            cc.ChainedConstructorArgs.Add(methodInvokeExp);
            cc.ChainedConstructorArgs.Add(new CodeVariableReferenceExpression("verifyRevision"));

            classType.Members.Add(cc);
        }

        private static void CreateConstructorFromStream(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };
            cc.Parameters.Add(typeof(Stream), "stream");
            cc.Parameters.Add(typeof(bool), "verifyRevision");

            var readerRef = new CodeTypeReferenceExpression(string.Join(".", generationInfo.ReaderNamespace, "CremaReader"));
            var paramExp = new CodeVariableReferenceExpression("stream");
            var methodInvokeExp = new CodeMethodInvokeExpression(readerRef, "Read", paramExp);

            cc.ChainedConstructorArgs.Add(methodInvokeExp);
            var left = new CodeVariableReferenceExpression("verifyRevision");
            var right = new CodePrimitiveExpression(false);
            cc.ChainedConstructorArgs.Add(new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.Assign, right));

            classType.Members.Add(cc);
        }

        private static void CreateLoadFromFile(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Load",
            };
            cc.Parameters.Add(typeof(string), "filename");
            cc.Parameters.Add(new CodeTypeReference(typeof(bool)), "verifyRevision");

            var readerRef = new CodeTypeReferenceExpression(string.Join(".", generationInfo.ReaderNamespace, "CremaReader"));
            var paramExp = new CodeVariableReferenceExpression("filename");
            var readExp = new CodeMethodInvokeExpression(readerRef, "Read", paramExp);

            var methodInvokeExp = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Load", readExp, new CodeVariableReferenceExpression("verifyRevision"));
            cc.Statements.Add(methodInvokeExp);

            classType.Members.Add(cc);
        }

        private static void CreateLoadStream(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Load",
            };
            cc.Parameters.Add(typeof(Stream), "stream");
            cc.Parameters.Add(typeof(bool), "verifyRevision");

            var readerRef = new CodeTypeReferenceExpression(string.Join(".", generationInfo.ReaderNamespace, "CremaReader"));
            var paramExp = new CodeVariableReferenceExpression("stream");
            var readExp = new CodeMethodInvokeExpression(readerRef, "Read", paramExp);

            var methodInvokeExp = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Load", readExp, new CodeVariableReferenceExpression("verifyRevision"));
            cc.Statements.Add(methodInvokeExp);

            classType.Members.Add(cc);
        }

        private static void CreateLoad(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Load",
            };
            cc.Parameters.Add("IDataSet", "dataSet");
            cc.Parameters.Add(new CodeTypeReference(typeof(bool)), "verifyRevision");

            var dataSet = new CodeVariableReferenceExpression("dataSet");

            {
                var ccs = CreateCompareDataBaseStatement(classType, generationInfo);
                var tst = CreateTryCatchStatement(classType, ccs, generationInfo);
                cc.Statements.Add(tst);
            }

            {
                var ccs = CreateCompareRevisionStatement(classType, generationInfo);
                var tst = CreateTryCatchStatement(classType, ccs, generationInfo);
                cc.Statements.Add(tst);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_name");
                var property = new CodePropertyReferenceExpression(dataSet, "Name");
                cc.Statements.AddAssign(field, property);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_revision");
                var property = new CodePropertyReferenceExpression(dataSet, "Revision");
                cc.Statements.AddAssign(field, property);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_typesHashValue");
                var property = new CodePropertyReferenceExpression(dataSet, "TypesHashValue");
                cc.Statements.AddAssign(field, property);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_tablesHashValue");
                var property = new CodePropertyReferenceExpression(dataSet, "TablesHashValue");
                cc.Statements.AddAssign(field, property);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_tags");
                var property = new CodePropertyReferenceExpression(dataSet, "Tags");
                cc.Statements.AddAssign(field, property);
            }

            foreach (var item in generationInfo.GetTables(true))
            {
                var tableName = new CodePrimitiveExpression(item.Name);
                var tablesProp = new CodePropertyReferenceExpression(dataSet, "Tables");
                var table = new CodeIndexerExpression(tablesProp, tableName);

                var field = item.GetFieldExpression();
                var instance = new CodeObjectCreateExpression(item.GetCodeType(), table);
                var assignStatement = new CodeAssignStatement(field, instance);

                if (generationInfo.IsDevmode == true)
                {
                    var tryStatement = CreateTryCatchStatement(classType, assignStatement, generationInfo);
                    cc.Statements.Add(tryStatement);
                }
                else
                {
                    cc.Statements.Add(assignStatement);
                }
            }

            classType.Members.Add(cc);
        }

        private static void CreateTableFields(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetTables(true))
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

        private static void CreateTableProperties(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetTables(true))
            {
                var cmp = new CodeMemberProperty()
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    Name = item.GetPropertyName(),
                    Type = item.GetCodeType(),
                    HasGet = true,
                    HasSet = false
                };
                cmp.Comments.AddSummary(item.Comment);
                cmp.GetStatements.AddMethodReturn(item.GetFieldExpression());

                classType.Members.Add(cmp);
            }
        }

        private static void CreateNameField(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Private;
            cmf.Name = "_name";
            cmf.Type = new CodeTypeReference(typeof(string));

            classType.Members.Add(cmf);
        }

        private static void CreateRevisionField(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Private;
            cmf.Name = "_revision";
            cmf.Type = new CodeTypeReference(typeof(long));

            classType.Members.Add(cmf);
        }

        private static void CreateTypesHashValueField(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Private;
            cmf.Name = "_typesHashValue";
            cmf.Type = new CodeTypeReference(typeof(string));

            classType.Members.Add(cmf);
        }

        private static void CreateTablesHashValueField(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Private;
            cmf.Name = "_tablesHashValue";
            cmf.Type = new CodeTypeReference(typeof(string));

            classType.Members.Add(cmf);
        }

        private static void CreateTagsField(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Private;
            cmf.Name = "_tags";
            cmf.Type = new CodeTypeReference(typeof(string));

            classType.Members.Add(cmf);
        }

        private static void CreateNameProperty(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberProperty();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "Name";
            cmm.HasGet = true;
            cmm.Type = new CodeTypeReference(typeof(string));
            cmm.GetStatements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_name"));

            classType.Members.Add(cmm);
        }

        private static void CreateRevisionProperty(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberProperty();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "Revision";
            cmm.HasGet = true;
            cmm.Type = new CodeTypeReference(typeof(long));
            cmm.GetStatements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_revision"));

            classType.Members.Add(cmm);
        }

        private static void CreateTypesHashValueProperty(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberProperty();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "TypesHashValue";
            cmm.HasGet = true;
            cmm.Type = new CodeTypeReference(typeof(string));
            cmm.GetStatements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_typesHashValue"));

            classType.Members.Add(cmm);
        }

        private static void CreateTablesHashValueProperty(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberProperty();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "TablesHashValue";
            cmm.HasGet = true;
            cmm.Type = new CodeTypeReference(typeof(string));
            cmm.GetStatements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_tablesHashValue"));

            classType.Members.Add(cmm);
        }

        private static void CreateTagsProperty(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberProperty();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "Tags";
            cmm.HasGet = true;
            cmm.Type = new CodeTypeReference(typeof(string));
            cmm.GetStatements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_tags"));

            classType.Members.Add(cmm);
        }

        private static CodeStatement CreateCompareDataBaseStatement(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var ccs = new CodeConditionStatement();

            var left = new CodePrimitiveExpression(generationInfo.DataBaseName);
            var right = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("dataSet"), "Name");
            ccs.Condition = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityInequality, right);

            var message = string.Format("데이터의 이름이 코드 이름({0})과 다릅니다.", generationInfo.DataBaseName);
            var exception = new CodeObjectCreateExpression(typeof(Exception), new CodePrimitiveExpression(message));
            ccs.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return ccs;
        }

        private static CodeStatement CreateCompareRevisionStatement(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var ccs = new CodeConditionStatement();

            var left1 = new CodeVariableReferenceExpression("verifyRevision");
            var right1 = new CodePrimitiveExpression(true);
            var left = new CodeBinaryOperatorExpression(left1, CodeBinaryOperatorType.IdentityEquality, right1);

            var left2 = new CodeCastExpression(typeof(int), new CodePrimitiveExpression(generationInfo.Revision));
            var right2 = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("dataSet"), "Revision");
            var right = new CodeBinaryOperatorExpression(left2, CodeBinaryOperatorType.IdentityInequality, right2);

            ccs.Condition = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.BooleanAnd, right);

            var message = string.Format("데이터의 리비전 코드 리비전({0})과 다릅니다.", generationInfo.Revision);
            var exception = new CodeObjectCreateExpression(typeof(Exception), new CodePrimitiveExpression(message));
            ccs.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return ccs;
        }

        private static CodeStatement CreateCompareTypesHashValueStatement(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var ccs = new CodeConditionStatement();

            var left = new CodePrimitiveExpression(generationInfo.TypesHashValue);
            var right = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("dataSet"), "TypesHashValue");
            ccs.Condition = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityInequality, right);

            var message = string.Format("타입 해시값이 '{0}'이 아닙니다.", generationInfo.TypesHashValue);
            var exception = new CodeObjectCreateExpression(typeof(Exception), new CodePrimitiveExpression(message));
            ccs.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return ccs;
        }

        private static CodeStatement CreateCompareTablesHashValueStatement(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var ccs = new CodeConditionStatement();

            var left = new CodePrimitiveExpression(generationInfo.TablesHashValue);
            var right = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("dataSet"), "TablesHashValue");
            ccs.Condition = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityInequality, right);

            var message = string.Format("테이블 해시값이 '{0}'이 아닙니다.", generationInfo.TablesHashValue);
            var exception = new CodeObjectCreateExpression(typeof(Exception), new CodePrimitiveExpression(message));
            ccs.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return ccs;
        }

        public static CodeStatement CreateTryCatchStatement(CodeTypeDeclaration classType, CodeStatement statement, CodeGenerationInfo generationInfo)
        {
            var tryStatement = new CodeTryCatchFinallyStatement();

            tryStatement.TryStatements.Add(statement);

            var catchClause = new CodeCatchClause("e");
            var methodRefExp = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression($"{generationInfo.BaseNamespace}.CremaData"), "InvokeErrorOccuredEvent");
            var methodInvokeExp = new CodeMethodInvokeExpression(methodRefExp, new CodeThisReferenceExpression(), new CodeVariableReferenceExpression("e"));

            var conditionStatement = new CodeConditionStatement()
            {
                Condition = new CodeBinaryOperatorExpression(methodInvokeExp, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false))
            };
            conditionStatement.TrueStatements.Add(new CodeThrowExceptionStatement(new CodeVariableReferenceExpression("e")));

            catchClause.Statements.Add(conditionStatement);
            tryStatement.CatchClauses.Add(catchClause);

            return tryStatement;
        }
    }
}
