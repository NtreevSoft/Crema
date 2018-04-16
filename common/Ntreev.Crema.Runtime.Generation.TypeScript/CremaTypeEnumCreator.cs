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
using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Generation.TypeScript
{
    static class CremaTypeEnumCreator
    {
        public static void CreateTypes(CodeNamespace codeNamespace, CodeGenerationInfo generationInfo)
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
                CreateType(codeNamespace, item, generationInfo);
            }
        }

        public static void CreateType(CodeNamespace codeNamespace, TypeInfo typeInfo, CodeGenerationInfo generationInfo)
        {
            var classType = new CodeTypeDeclaration(typeInfo.Name);
            classType.Attributes = MemberAttributes.Public;
            classType.IsEnum = true;

            if (typeInfo.IsFlag == true)
            {
                classType.CustomAttributes.Add(typeof(FlagsAttribute));
            }

            if (generationInfo.OmitComment == false)
            {
                classType.Comments.AddSummary(typeInfo.Comment);
            }

            if (generationInfo.OmitSignatureDate == false)
            {
                classType.Comments.Add(CremaCodeGenerator.Creator, typeInfo.CreationInfo.ID);
                classType.Comments.Add(CremaCodeGenerator.CreatedDateTime, typeInfo.CreationInfo.DateTime);
                classType.Comments.Add(CremaCodeGenerator.Modifier, typeInfo.ModificationInfo.ID);
                classType.Comments.Add(CremaCodeGenerator.ModifiedDateTime, typeInfo.ModificationInfo.DateTime);
            }

            foreach (var item in typeInfo.Members)
            {
                CreateTypeMember(classType, item, generationInfo);
            }

            codeNamespace.Types.Add(classType);
        }

        public static void CreateTypeMember(CodeTypeDeclaration classType, TypeMemberInfo typeMemberInfo, CodeGenerationInfo generationInfo)
        {
            var cmm = new CodeMemberField();
            cmm.Name = typeMemberInfo.Name;
            cmm.InitExpression = new CodePrimitiveExpression(typeMemberInfo.Value);
            if (generationInfo.OmitComment == false)
            {
                cmm.Comments.AddSummary(typeMemberInfo.Comment);
            }
            if (generationInfo.OmitSignatureDate == false)
            {
                cmm.Comments.Add(CremaCodeGenerator.Creator, typeMemberInfo.CreationInfo.ID);
                cmm.Comments.Add(CremaCodeGenerator.CreatedDateTime, typeMemberInfo.CreationInfo.DateTime);
                cmm.Comments.Add(CremaCodeGenerator.Modifier, typeMemberInfo.ModificationInfo.ID);
                cmm.Comments.Add(CremaCodeGenerator.ModifiedDateTime, typeMemberInfo.ModificationInfo.DateTime);
            }

            classType.Members.Add(cmm);
        }
    }
}
