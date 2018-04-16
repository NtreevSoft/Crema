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
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using System.Reflection;

namespace Ntreev.Crema.Runtime.Generation.CSharp
{
    static class CremaRowClassCreator
    {
        public static void Create(CodeNamespace codeNamespace, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var classType = new CodeTypeDeclaration()
            {
                Attributes = MemberAttributes.Public,
                Name = tableInfo.GetRowClassName(),
                IsClass = true
            };
            classType.TypeAttributes |= TypeAttributes.Sealed;
            classType.BaseTypes.Add(generationInfo.BaseNamespace, "CremaRow");

            CreateChildStaticEmptyFields(classType, tableInfo, generationInfo);
            CreateConstructor(classType, tableInfo, generationInfo);
            CreateTableField(classType, tableInfo);
            CreateColumnFields(classType, tableInfo, generationInfo);
            CreateChildFields(classType, tableInfo, generationInfo);

            CreateSetChildsMethod(classType, tableInfo, generationInfo);
            CreateColumnProperties(classType, tableInfo, generationInfo);
            CreateChildProperties(classType, tableInfo, generationInfo);
            CreateTableProperty(classType, tableInfo);
            CreateParentProperty(classType, tableInfo);

            codeNamespace.Types.Add(classType);
        }

        private static void CreateConstructor(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };
            cc.Parameters.Add(generationInfo.ReaderNamespace, "IRow", "row");
            cc.Parameters.Add(tableInfo.GetCodeType(), "table");
            cc.BaseConstructorArgs.Add("row");

            // assign table field
            {
                var leftExp = tableInfo.GetFieldExpression();
                var rightExp = new CodeVariableReferenceExpression("table");
                cc.Statements.AddAssign(leftExp, rightExp);
            }

            // assign fields
            {
                var index = 0;
                foreach (var item in tableInfo.Columns)
                {
                    var cas = new CodeAssignStatement()
                    {
                        Left = item.GetFieldExpression(),
                        Right = item.GetGetValueMethodExpression(index, false)
                    };
                    if (item.IsKey == false)
                    {
                        var ccs = new CodeConditionStatement(item.GetHasValueMethodExpression(index, false));
                        ccs.TrueStatements.Add(cas);
                        cc.Statements.Add(ccs);
                    }
                    else
                    {
                        cc.Statements.Add(cas);
                    }
                    index++;
                }
            }

            // invoke SetKey method
            {
                var methodRefExp = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "SetKey");
                var methodInvokeExp = new CodeMethodInvokeExpression(methodRefExp);

                foreach (var item in tableInfo.Columns)
                {
                    if (item.IsKey == true)
                    {
                        var getHashCode = new CodeMethodReferenceExpression(item.GetFieldExpression(), "GetHashCode");
                        methodInvokeExp.Parameters.Add(new CodeMethodInvokeExpression(getHashCode));
                    }
                }
                cc.Statements.Add(methodInvokeExp);
            }

            classType.Members.Add(cc);
        }

        private static void CreateTableField(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            var cmf = new CodeMemberField()
            {
                Attributes = MemberAttributes.Private,
                Name = tableInfo.GetFieldName(),
                Type = tableInfo.GetCodeType()
            };
            classType.Members.Add(cmf);
        }

        private static void CreateSetChildsMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmm = new CodeMemberMethod()
                {
                    Attributes = MemberAttributes.FamilyAndAssembly | MemberAttributes.Static,
                    Name = "Set" + item.TableName
                };
                cmm.Parameters.Add(tableInfo.GetRowCodeType(), "target");
                cmm.Parameters.Add(typeof(string), "childName");
                cmm.Parameters.Add(item.GetRowCodeType(), 1, "childs");

                {
                    var methodRefExp = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(tableInfo.GetRowCodeType()), "SetParent");
                    methodRefExp.TypeArguments.Add(tableInfo.GetRowCodeType());
                    methodRefExp.TypeArguments.Add(item.GetRowCodeType());

                    var methodInvokeExp = new CodeMethodInvokeExpression(methodRefExp, new CodeVariableReferenceExpression("target"), new CodeVariableReferenceExpression("childs"));

                    cmm.Statements.AddExpression(methodInvokeExp);
                }

                {
                    var target = new CodeVariableReferenceExpression("target");
                    var childName = new CodeVariableReferenceExpression("childName");
                    var childs = new CodeVariableReferenceExpression("childs");
                    var targetField = item.GetFieldExpression(target);
                    var targetInstance = new CodeObjectCreateExpression(item.GetCodeType(), childName, childs);
                    cmm.Statements.AddAssign(targetField, targetInstance);
                }

                classType.Members.Add(cmm);
            }
        }

        private static void CreateColumnFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in tableInfo.Columns)
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

        private static void CreateChildStaticEmptyFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmf = new CodeMemberField()
                {
                    Attributes = MemberAttributes.Private | MemberAttributes.Static,
                    Name = item.GetFieldName() + "Empty",
                    Type = item.GetCodeType(),
                    InitExpression = new CodeObjectCreateExpression(item.GetCodeType())
                };
                classType.Members.Add(cmf);
            }
        }

        private static void CreateColumnProperties(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in tableInfo.Columns)
            {
                var cmp = new CodeMemberProperty()
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    Name = item.Name,
                    Type = item.GetCodeType()
                };
                if (generationInfo.OmitComment == false)
                {
                    cmp.Comments.AddSummary(item.Comment);
                }
                if (generationInfo.OmitSignatureDate == false)
                {
                    cmp.Comments.Add(CremaSchema.Creator, item.CreationInfo.ID);
                    cmp.Comments.Add(CremaSchema.CreatedDateTime, item.CreationInfo.DateTime);
                    cmp.Comments.Add(CremaSchema.Modifier, item.ModificationInfo.ID);
                    cmp.Comments.Add(CremaSchema.ModifiedDateTime, item.ModificationInfo.DateTime);
                }
                cmp.HasGet = true;
                cmp.HasSet = false;
                cmp.GetStatements.AddMethodReturn(item.GetFieldExpression());

                classType.Members.Add(cmp);
            }
        }

        private static void CreateChildProperties(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmp = new CodeMemberProperty()
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    Name = item.GetPropertyName(),
                    Type = item.GetCodeType()
                };
                if (generationInfo.OmitComment == false)
                {
                    cmp.Comments.AddSummary(item.Comment);
                }
                if (generationInfo.OmitSignatureDate == false)
                {
                    cmp.Comments.Add(CremaSchema.Creator, item.CreationInfo.ID);
                    cmp.Comments.Add(CremaSchema.CreatedDateTime, item.CreationInfo.DateTime);
                    cmp.Comments.Add(CremaSchema.Modifier, item.ModificationInfo.ID);
                    cmp.Comments.Add(CremaSchema.ModifiedDateTime, item.ModificationInfo.DateTime);
                }
                cmp.HasGet = true;
                cmp.HasSet = false;

                {
                    var state = new CodeConditionStatement();
                    var testExp = new CodeBinaryOperatorExpression(item.GetFieldExpression(), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null));
                    state.Condition = testExp;

                    var staticRefExp = new CodeTypeReferenceExpression(tableInfo.GetRowCodeType());
                    var fieldExp = new CodeFieldReferenceExpression(staticRefExp, item.GetFieldName() + "Empty");
                    state.TrueStatements.AddMethodReturn(fieldExp);

                    cmp.GetStatements.Add(state);
                }

                {
                    var fieldExp = item.GetFieldExpression();
                    cmp.GetStatements.AddMethodReturn(fieldExp);
                }

                classType.Members.Add(cmp);
            }
        }

        private static void CreateParentProperty(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            if (string.IsNullOrEmpty(tableInfo.ParentName) == true)
                return;

            var cmp = new CodeMemberProperty()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Parent",
                Type = tableInfo.GetParentRowCodeType(),
                HasGet = true,
                HasSet = false
            };

            // statement
            {
                var parentExp = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "ParentInternal");
                var castExp = new CodeCastExpression(tableInfo.GetParentRowCodeType(), parentExp);
                cmp.GetStatements.AddMethodReturn(castExp);
            }

            classType.Members.Add(cmp);
        }

        private static void CreateTableProperty(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            var cmp = new CodeMemberProperty()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Table",
                Type = tableInfo.GetCodeType(),
                HasGet = true,
                HasSet = false
            };
            var fieldExp = tableInfo.GetFieldExpression();
            cmp.GetStatements.AddMethodReturn(fieldExp);

            classType.Members.Add(cmp);
        }
    }
}
