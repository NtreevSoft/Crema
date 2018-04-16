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

using Ntreev.Crema.Runtime.Generation.TypeScript.Properties;
using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Runtime.Generation.TypeScript.TypeScript
{
    static class CremaDeclCreator
    {
        public static void Create(CodeNamespace codeNamespace, CodeGenerationInfo generationInfo)
        {
            CreateCremaRow(codeNamespace);
            CreateCremaTable(codeNamespace);
            CreateCremaData(codeNamespace);
            CremaTypeEnumCreator.CreateTypes(codeNamespace, generationInfo);
            CreateTables(codeNamespace, generationInfo);
        }

        public static void CreateBase(CodeNamespace codeNamespace)
        {
            CreateCremaRow(codeNamespace);
            CreateCremaTable(codeNamespace);
            CreateCremaData(codeNamespace);
        }

        private static void CreateCremaData(CodeNamespace codeNamespace)
        {
            var classType = new CodeTypeDeclaration();
            classType.Attributes = MemberAttributes.Private;
            classType.Name = "CremaData";
            classType.IsClass = true;
            codeNamespace.Types.Add(classType);
        }

        private static void CreateCremaRow(CodeNamespace codeNamespace)
        {
            var classType = new CodeTypeDeclaration();
            classType.Attributes = MemberAttributes.Private;
            classType.Name = "CremaRow";
            classType.IsClass = true;
            codeNamespace.Types.Add(classType);
        }

        private static void CreateCremaTable(CodeNamespace codeNamespace)
        {
            var classType = new CodeTypeDeclaration();
            classType.Attributes = MemberAttributes.Private;
            classType.Name = "CremaTable";
            classType.IsClass = true;
            classType.TypeParameters.Add(new CodeTypeParameter("T extends CremaRow"));
            codeNamespace.Types.Add(classType);

            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Public;
            cmf.Name = "rows";
            cmf.Type = new CodeTypeReference(new CodeTypeReference("T"), 1);
            classType.Members.Add(cmf);
        }

        public static void CreateTables(CodeNamespace codeNamespace, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetTables())
            {
                CreateTable(codeNamespace, item, generationInfo);
            }

            CreateDataSet(codeNamespace, generationInfo);
        }

        public static void CreateTable(CodeNamespace codeNamespace, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var classType = new CodeTypeDeclaration();

            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                CreateTable(codeNamespace, item, generationInfo);
            }

            CremaDeclCreator.CreateRowDecl(codeNamespace, tableInfo, generationInfo);

            classType.Attributes = MemberAttributes.Public;
            classType.Name = tableInfo.GetClassName();
            classType.IsClass = true;
            classType.TypeAttributes |= System.Reflection.TypeAttributes.Sealed;
            classType.BaseTypes.Add(new CodeTypeReference("CremaTable", tableInfo.GetRowCodeType()));

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

            CreateTableFields(classType, tableInfo, generationInfo);
            //CreateProperties(classType, tableInfo);
            //CreateConstructor(classType, tableInfo);
            CreateTableFindMethodDecl(classType, tableInfo);
            //CreateCreateRowInstanceMethod(classType, tableInfo);

            
            codeNamespace.Types.Add(classType);
        }

        public static void CreateRowDecl(CodeNamespace codeNamespace, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var classType = new CodeTypeDeclaration();
            classType.Attributes = MemberAttributes.Public | MemberAttributes.Abstract;
            classType.Name = tableInfo.GetRowClassName();
            classType.IsClass = true;
            classType.TypeAttributes |= System.Reflection.TypeAttributes.Sealed;
            classType.BaseTypes.Add("CremaRow");

            CreateTableField(classType, tableInfo);
            CreateRowFieldsDecl(classType, tableInfo);
            CreateChildFields(classType, tableInfo, generationInfo);
            CreateParentField(classType, tableInfo);

            codeNamespace.Types.Add(classType);
        }

        private static void CreateRowFieldsDecl(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            foreach (var item in tableInfo.Columns)
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Public;
                cmf.Name = item.GetPropertyName();
                cmf.Type = item.GetCodeType();
                classType.Members.Add(cmf);
            }
        }

        private static void CreateTableField(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Public;
            cmf.Name = "Table";
            cmf.Type = tableInfo.GetCodeType();
            classType.Members.Add(cmf);
        }

        private static void CreateChildFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Public;
                cmf.Name = item.GetPropertyName();
                cmf.Type = item.GetCodeType();
                classType.Members.Add(cmf);
            }
        }

        private static void CreateParentField(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            if (string.IsNullOrEmpty(tableInfo.ParentName) == true)
                return;

            var cmp = new CodeMemberField();
            cmp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmp.Name = "parent";
            cmp.Type = tableInfo.GetParentRowCodeType();

            classType.Members.Add(cmp);
        }

        public static void CreateDataSet(CodeNamespace codeNamespace, CodeGenerationInfo generationInfo)
        {
            var classType = new CodeTypeDeclaration();
            codeNamespace.Types.Add(classType);
            classType.Attributes = MemberAttributes.Private;
            classType.Name = generationInfo.ClassName;
            classType.IsClass = true;
            classType.TypeAttributes |= System.Reflection.TypeAttributes.Sealed;
            classType.BaseTypes.Add("CremaData");

            CreateDataSetNameField(classType);
            CreateDataSetRevisionField(classType);
            CreateDataSetFields(classType, generationInfo.GetTables(true));
            CreateDataSetMethodCreateFromFile(classType, generationInfo);
            //CreateDataSetMethodCreateFromDataSet(classType, generationInfo);
            CreateDataSetMethodReadFromFile(classType, generationInfo);
        }

        private static void CreateTableFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Public;
                cmf.Name = item.GetPropertyName();
                cmf.Type = item.GetCodeType();
                classType.Members.Add(cmf);
            }
        }

        private static void CreateTableFindMethodDecl(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public;
            cmm.Name = "find";
            cmm.ReturnType = tableInfo.GetRowCodeType();
            cmm.Parameters.Add(tableInfo.Columns.Where(item => item.IsKey));

            classType.Members.Add(cmm);
        }

        private static void CreateDataSetNameField(CodeTypeDeclaration classType)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Public;
            cmf.Name = "name";
            cmf.Type = new CodeTypeReference(typeof(string));

            classType.Members.Add(cmf);
        }

        private static void CreateDataSetRevisionField(CodeTypeDeclaration classType)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Public;
            cmf.Name = "revision";
            cmf.Type = new CodeTypeReference(typeof(long));

            classType.Members.Add(cmf);
        }

        private static void CreateDataSetFields(CodeTypeDeclaration classType, IEnumerable<TableInfo> tables)
        {
            foreach (var item in tables)
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Public;
                cmf.Name = item.GetPropertyName();
                cmf.Type = item.GetCodeType();

                classType.Members.Add(cmf);
            }
        }

        private static void CreateDataSetMethodCreateFromFile(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            cmm.Name = "createFromFile";
            cmm.Parameters.Add(typeof(string), "filename");
            cmm.Parameters.Add(typeof(bool), "verifyRevision");
            cmm.ReturnType = new CodeTypeReference(generationInfo.ClassName);

            classType.Members.Add(cmm);
        }

        private static void CreateDataSetMethodCreateFromDataSet(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            cmm.Name = "createFromDataSet";
            cmm.Parameters.Add("reader", "IDataSet", "dataSet");
            cmm.Parameters.Add(typeof(bool), "verifyRevision");
            cmm.ReturnType = new CodeTypeReference(generationInfo.ClassName);

            classType.Members.Add(cmm);
        }

        private static void CreateDataSetMethodReadFromFile(CodeTypeDeclaration classType, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Family;
            cmm.Name = "readFromFile";
            cmm.Parameters.Add(typeof(string), "filename");
            cmm.Parameters.Add(typeof(bool), "verifyRevision");

            classType.Members.Add(cmm);
        }
    }
}
