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
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using Ntreev.Crema.Runtime.Generation.NativeC;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Runtime.Generation.NativeC
{
    public static class CremaTypeEnumCreator
    {
        public static bool NoCpp
        {
            get;
            set;
        }

        public static void Create(CodeNamespace codeNamespace, CodeGenerationInfo generationInfo)
        {
            codeNamespace.Comments.Add(new CodeCommentStatement($"------------------------------------------------------------------------------"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"dataBase: {generationInfo.DataBaseName}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"revision: {generationInfo.Revision}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"requested revision: {generationInfo.RequestedRevision}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"devmode: {generationInfo.IsDevmode}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"hash value: {generationInfo.TypesHashValue}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"tags: {generationInfo.Tags}"));
            codeNamespace.Comments.Add(new CodeCommentStatement($"------------------------------------------------------------------------------"));

            foreach (var item in generationInfo.Types)
            {
                CreateDataType(codeNamespace, item, generationInfo);
            }

            //if (NoCpp == false)
            //    codeNamespace.AddHeaderStatement("void register_enums();");
        }

        private static void CreateDataTypeRegisterMethods(CodeNamespace codeNamespace, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.Types)
            {
                CreateDataTypeRegisterMethod(codeNamespace, item, generationInfo);
            }

            using (var tw = new IndentedTextWriter(new StringWriter()))
            {
                tw.WriteLine("void register_enums()");
                tw.WriteLine("{");

                tw.Indent++;
                foreach (var item in generationInfo.Types)
                {
                    tw.WriteLine("register_{0}();", item.Name);
                }
                tw.Indent--;

                tw.Write("}");
                tw.Flush();
                codeNamespace.AddCppStatement(tw.InnerWriter.ToString());
            }
        }

        private static void CreateDataType(CodeNamespace codeNamespace, TypeInfo dataTypeInfo, CodeGenerationInfo generationInfo)
        {
            var classType = new CodeTypeDeclaration(dataTypeInfo.Name);
            classType.Attributes = MemberAttributes.Public;
            classType.IsEnum = true;

            if (dataTypeInfo.IsFlag == true)
            {
                CodeAttributeDeclaration cad = new CodeAttributeDeclaration(new CodeTypeReference(typeof(FlagsAttribute)));

                classType.CustomAttributes.Add(cad);
            }

            if (generationInfo.NoComment == false)
            {
                classType.Comments.AddSummary(dataTypeInfo.Comment);
            }

            if (generationInfo.NoChanges == false)
            {
                classType.Comments.Add(CremaSchema.Creator, dataTypeInfo.CreationInfo.ID);
                classType.Comments.Add(CremaSchema.CreatedDateTime, dataTypeInfo.CreationInfo.DateTime);
                classType.Comments.Add(CremaSchema.Modifier, dataTypeInfo.ModificationInfo.ID);
                classType.Comments.Add(CremaSchema.ModifiedDateTime, dataTypeInfo.ModificationInfo.DateTime);
            }

            foreach (var item in dataTypeInfo.Members)
            {
                CreateDataMember(classType, item, generationInfo);
            }
            //CreateEnumRegisterMethod(enum1, isFlag);
            codeNamespace.Types.Add(classType);
        }

        private static void CreateDataMember(CodeTypeDeclaration classType, TypeMemberInfo typeMemberInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberField();
            cmm.Name = typeMemberInfo.Name;
            cmm.InitExpression = new CodeSnippetExpression(generationInfo.EnumFomrat(typeMemberInfo.Value));
            if (generationInfo.NoComment == false)
            {
                cmm.Comments.AddSummary(typeMemberInfo.Comment);
            }
            if (generationInfo.NoChanges == false)
            {
                cmm.Comments.Add(CremaSchema.Creator, typeMemberInfo.CreationInfo.ID);
                cmm.Comments.Add(CremaSchema.CreatedDateTime, typeMemberInfo.CreationInfo.DateTime);
                cmm.Comments.Add(CremaSchema.Modifier, typeMemberInfo.ModificationInfo.ID);
                cmm.Comments.Add(CremaSchema.ModifiedDateTime, typeMemberInfo.ModificationInfo.DateTime);
            }

            classType.Members.Add(cmm);
        }

        private static void CreateDataTypeRegisterMethod(CodeNamespace codeNamespace, TypeInfo dataTypeInfo, CodeGenerationInfo generationInfo)
        {
            using (IndentedTextWriter tw = new IndentedTextWriter(new StringWriter()))
            {
                tw.WriteLine("void register_{0}()", dataTypeInfo.Name);
                tw.WriteLine("{");
                tw.Indent++;
                tw.WriteLine("static ntreev::crema::enum_info enumData({0});", dataTypeInfo.IsFlag.ToString().ToLower());
                tw.WriteLine("if(ntreev::crema::enum_util::contains(typeid({0})) == true)", dataTypeInfo.Name);
                tw.Indent++;
                tw.WriteLine("return;");
                tw.Indent--;

                foreach (var item in dataTypeInfo.Members)
                {
                    tw.WriteLine("enumData.add(\"{0}_{1}\", {0}_{1});", dataTypeInfo.Name, item.Name);
                }

                tw.WriteLine("ntreev::crema::enum_util::add(typeid({0}), &enumData);", dataTypeInfo.Name);
                tw.Indent--;
                tw.Write("}");
                tw.Flush();

                codeNamespace.AddCppStatement(tw.InnerWriter.ToString());
            }
        }
    }
}
