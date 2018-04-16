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
using Ntreev.Crema.Runtime.Generation.NativeC.CodeDom;

namespace Ntreev.Crema.Runtime.Generation.NativeC
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

            var classType = new CodeTypeDeclaration();
            codeNamespace.Types.Add(classType);
            classType.Name = generationInfo.ClassName;
            classType.IsClass = true;
            classType.BaseTypes.Add(generationInfo.BaseNamespace, "CremaData");

            CreateNameField(classType, generationInfo);
            CreateRevisionField(classType, generationInfo);
            CreateTypesHashValueField(classType, generationInfo);
            CreateTablesHashValueField(classType, generationInfo);
            CreateTagsField(classType, generationInfo);
            CreateNameMethod(classType, generationInfo);
            CreateRevisionMethod(classType, generationInfo);
            CreateTypesHashValueMethod(classType, generationInfo);
            CreateTablesHashValueMethod(classType, generationInfo);
            CreateTagsMethod(classType, generationInfo);
            CreateFieldsTable(classType, generationInfo);
            CreateConstructor(classType, generationInfo);
            CreateConstructorFromFile(classType, generationInfo);
            CreateLoadFromFile(classType, generationInfo);
            CreateLoad(classType, generationInfo);
            CreateDestructor(classType, generationInfo);
        }

        private static void CreateConstructor(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;
            cc.Parameters.Add(generationInfo.ReaderNamespace, "idataset&", "dataSet");
            cc.Parameters.Add(new CodeTypeReference(typeof(bool)), "verifyRevision");

            foreach (var item in generationInfo.GetTables(true))
            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), item.Name);
                cc.Statements.AddAssign(field, new CodePrimitiveExpression(null));
            }

            var methodInvokeExp = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Load", new CodeVariableReferenceExpression("dataSet"), new CodeVariableReferenceExpression("verifyRevision"));
            cc.Statements.Add(methodInvokeExp);

            classType.Members.Add(cc);
        }

        private static void CreateDestructor(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeDestructor();
            cc.Attributes = MemberAttributes.Public | MemberAttributes.Override;

            foreach (var item in generationInfo.GetTables())
            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), item.Name);
                var delete = new CodeObjectDeleteExpression(field);

                cc.Statements.Add(delete);
            }

            classType.Members.Add(cc);
        }

        private static void CreateConstructorFromFile(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;

            var codeTypeRef = new CodeTypeReference(typeof(string));
            codeTypeRef.SetCodeType(CodeType.Reference | CodeType.Const);
            cc.Parameters.Add(codeTypeRef, "filename");
            cc.Parameters.Add(new CodeTypeReference(typeof(bool)), "verifyRevision");

            var paramExp = new CodeVariableReferenceExpression("filename");
            var readerTypeRef = new CodeTypeReferenceExpression(string.Join("::", generationInfo.ReaderNamespace, "CremaReader"));
            var methodInvokeExp = new CodeMethodInvokeExpression(readerTypeRef, "ReadFromFile", paramExp);

            cc.ChainedConstructorArgs.Add(methodInvokeExp);
            cc.ChainedConstructorArgs.Add(new CodeVariableReferenceExpression("verifyRevision"));

            classType.Members.Add(cc);
        }

        private static void CreateLoad(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeMemberMethod();
            cc.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cc.Name = "Load";
            cc.Parameters.Add(generationInfo.ReaderNamespace, "idataset&", "dataSet");
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
                var ccs = CreateCompareTypesHashValueStatement(classType, generationInfo);
                var tst = CreateTryCatchStatement(classType, ccs, generationInfo);
                cc.Statements.Add(tst);
            }

            {
                var ccs = CreateCompareTablesHashValueStatement(classType, generationInfo);
                var tst = CreateTryCatchStatement(classType, ccs, generationInfo);
                cc.Statements.Add(tst);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_name");
                var property = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(dataSet, "name"));
                cc.Statements.AddAssign(field, property);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_revision");
                var property = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(dataSet, "revision"));
                cc.Statements.AddAssign(field, property);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_typesHashValue");
                var property = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(dataSet, "types_hash_value"));
                cc.Statements.AddAssign(field, property);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_tablesHashValue");
                var property = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(dataSet, "tables_hash_value"));
                cc.Statements.AddAssign(field, property);
            }

            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_tags");
                var property = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(dataSet, "tags"));
                cc.Statements.AddAssign(field, property);
            }

            foreach (var item in generationInfo.GetTables(true))
            {
                var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), item.Name);
                var operatorExp = new CodeBinaryOperatorExpression(field, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
                var ccs = new CodeConditionStatement(operatorExp);
                var delete = new CodeObjectDeleteExpression(field);
                ccs.TrueStatements.Add(delete);
                cc.Statements.Add(ccs);

                var tableName = new CodePrimitiveExpression(item.Name);
                var tablesProp = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(dataSet, "tables"));
                var table = new CodeIndexerExpression(tablesProp, tableName);

                //var field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), item.Name);
                var instance = new CodeObjectCreateExpression(item.GetCodeType(generationInfo.Namespace, CodeType.None), table);

                cc.Statements.AddAssign(field, instance);
            }

            classType.Members.Add(cc);
        }

        private static void CreateLoadFromFile(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeMemberMethod();
            cc.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cc.Name = "Load";

            var codeTypeRef = new CodeTypeReference(typeof(string));
            codeTypeRef.SetCodeType(CodeType.Reference | CodeType.Const);
            cc.Parameters.Add(codeTypeRef, "filename");
            cc.Parameters.Add(new CodeTypeReference(typeof(bool)), "verifyRevision");

            var paramExp = new CodeVariableReferenceExpression("filename");
            var readerTypeRef = new CodeTypeReferenceExpression(string.Join("::", generationInfo.ReaderNamespace, "CremaReader"));
            var readerInvokeExp = new CodeMethodInvokeExpression(readerTypeRef, "ReadFromFile", paramExp);

            var methodInvokeExp = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Load", readerInvokeExp, new CodeVariableReferenceExpression("verifyRevision"));
            cc.Statements.Add(methodInvokeExp);
            //cc.ChainedConstructorArgs.Add(methodInvokeExp);
            //cc.ChainedConstructorArgs.Add(new CodeVariableReferenceExpression("verifyRevision"));

            classType.Members.Add(cc);
        }

        private static void CreateFieldsTable(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetTables(true))
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Public;
                cmf.Name = item.Name;
                cmf.Type = item.GetCodeType(CodeType.Pointer | CodeType.Const);
                classType.Members.Add(cmf);
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

        private static void CreateNameMethod(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "name";
            cmm.ReturnType = new CodeTypeReference(typeof(string));
            cmm.Statements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_name"));

            classType.Members.Add(cmm);
        }

        private static void CreateRevisionMethod(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "revision";
            cmm.ReturnType = new CodeTypeReference(typeof(long));
            cmm.Statements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_revision"));

            classType.Members.Add(cmm);
        }

        private static void CreateTypesHashValueMethod(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "typesHashValue";
            cmm.ReturnType = new CodeTypeReference(typeof(string));
            cmm.Statements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_typesHashValue"));

            classType.Members.Add(cmm);
        }

        private static void CreateTablesHashValueMethod(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "tablesHashValue";
            cmm.ReturnType = new CodeTypeReference(typeof(string));
            cmm.Statements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_tablesHashValue"));

            classType.Members.Add(cmm);
        }

        private static void CreateTagsMethod(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.Name = "tags";
            cmm.ReturnType = new CodeTypeReference(typeof(string));
            cmm.Statements.AddMethodReturn(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "_tags"));

            classType.Members.Add(cmm);
        }

        private static CodeStatement CreateCompareDataBaseStatement(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var ccs = new CodeConditionStatement();

            var left = new CodePrimitiveExpression(generationInfo.DataBaseName);
            var right = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("dataSet"), "name()");
            ccs.Condition = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityInequality, right);

            var message = string.Format("데이터의 이름이 코드 이름({0})과 다릅니다.", generationInfo.DataBaseName);
            var exception = new CodeMethodInvokeExpression(null, "std::exception", new CodePrimitiveExpression(message));
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
            var right2 = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("dataSet"), "revision()");
            var right = new CodeBinaryOperatorExpression(left2, CodeBinaryOperatorType.IdentityInequality, right2);

            ccs.Condition = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.BooleanAnd, right);

            var message = string.Format("데이터의 리비전 코드 리비전({0})과 다릅니다.", generationInfo.Revision);
            var exception = new CodeMethodInvokeExpression(null, "std::exception", new CodePrimitiveExpression(message));
            ccs.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return ccs;
        }

        private static CodeStatement CreateCompareTypesHashValueStatement(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var ccs = new CodeConditionStatement();

            var left = new CodePrimitiveExpression(generationInfo.TypesHashValue);
            var right = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("dataSet"), "types_hash_value()");
            ccs.Condition = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityInequality, right);

            var message = "타입 해시값이 잘못되었습니다.";
            var exception = new CodeMethodInvokeExpression(null, "std::exception", new CodePrimitiveExpression(message));
            ccs.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return ccs;
        }

        private static CodeStatement CreateCompareTablesHashValueStatement(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var ccs = new CodeConditionStatement();

            var left = new CodePrimitiveExpression(generationInfo.TablesHashValue);
            var right = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("dataSet"), "tables_hash_value()");
            ccs.Condition = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityInequality, right);

            var message = "테이블 해시값이 잘못되었습니다.";
            var exception = new CodeMethodInvokeExpression(null, "std::exception", new CodePrimitiveExpression(message));
            ccs.TrueStatements.Add(new CodeThrowExceptionStatement(exception));

            return ccs;
        }

        public static CodeStatement CreateTryCatchStatement(CodeTypeDeclaration classType, CodeStatement statement, CodeGenerationInfo generationInfo)
        {
            var tryStatement = new CodeTryCatchFinallyStatement();

            tryStatement.TryStatements.Add(statement);

            var catchClause = new CodeCatchClause("e", new CodeTypeReference("std::exception&"));
            var methodRefExp = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression($"{generationInfo.BaseNamespace}.CremaData"), "InvokeErrorOccured");
            var methodInvokeExp = new CodeMethodInvokeExpression(methodRefExp, new CodeVariableReferenceExpression("e"));

            var conditionStatement = new CodeConditionStatement();
            conditionStatement.Condition = new CodeBinaryOperatorExpression(methodInvokeExp, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false));
            conditionStatement.TrueStatements.Add(new CodeThrowExceptionStatement(new CodeVariableReferenceExpression("e")));

            catchClause.Statements.Add(conditionStatement);
            tryStatement.CatchClauses.Add(catchClause);

            return tryStatement;
        }
    }
}
