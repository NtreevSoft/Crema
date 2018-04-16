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
using Ntreev.Crema.Runtime.Generation.TypeScript.Properties;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using System.Reflection;

namespace Ntreev.Crema.Runtime.Generation.TypeScript
{
    static class CremaRowClassCreator
    {
        private static readonly CodeThisReferenceExpression thisRef = new CodeThisReferenceExpression();

        public static void Create(CodeNamespace codeNamespace, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var classType = new CodeTypeDeclaration();
            classType.Attributes = MemberAttributes.Public;
            classType.Name = tableInfo.GetRowClassName();
            classType.IsClass = true;
            classType.TypeAttributes |= TypeAttributes.Sealed;
            classType.BaseTypes.Add("base", "CremaRow");

            CreateStaticChildEmptyFields(classType, tableInfo, generationInfo);
            CreateTableField(classType, tableInfo);
            CreateColumnFields(classType, tableInfo, generationInfo);
            CreateChildFields(classType, tableInfo, generationInfo);

            CreateConstructor(classType, tableInfo, generationInfo);

            CreateSetChildsMethod(classType, tableInfo, generationInfo);
            CreateColumnProperties(classType, tableInfo, generationInfo);
            CreateChildProperties(classType, tableInfo, generationInfo);
            CreateTableProperty(classType, tableInfo);
            CreateParentProperty(classType, tableInfo);

            codeNamespace.Types.Add(classType);
        }

        private static void CreateConstructor(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;
            cc.Parameters.Add("reader", "IRow", "row");
            cc.Parameters.Add(tableInfo.GetCodeType(), "table");
            cc.BaseConstructorArgs.AddVariableReference("row");

            // assign table field
            {
                var tableField = tableInfo.GetFieldExpression();
                var table = new CodeVariableReferenceExpression("table");
                cc.Statements.AddAssign(tableField, table);
            }

            // assign column fields
            {
                var index = 0;
                foreach (var item in tableInfo.Columns)
                {
                    var cas = new CodeAssignStatement();
                    cas.Left = item.GetFieldExpression();
                    cas.Right = item.GetGetValueMethodExpression(index, false);

                    if (item.IsKey == false)
                    {
                        var ccs = new CodeConditionStatement(item.GetHasValueMethodExpression(index, false));
                        ccs.TrueStatements.Add(cas);
                        var tst = CremaDataClassCreator.CreateTryCatchStatement(classType, ccs, false);
                        cc.Statements.Add(tst);
                    }
                    else
                    {
                        cc.Statements.Add(cas);
                    }
                    index++;
                }
            }

            // invoke setKey
            {
                var setKey = new CodeMethodReferenceExpression(thisRef, "setKey");
                var invokeSetKey = new CodeMethodInvokeExpression(setKey);

                foreach (var item in tableInfo.Columns)
                {
                    if (item.IsKey == true)
                    {
                        invokeSetKey.Parameters.Add(item.GetFieldExpression());
                    }
                }
                cc.Statements.Add(invokeSetKey);
            }

            classType.Members.Add(cc);
        }

        private static void CreateTableField(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Private;
            cmf.Name = tableInfo.GetFieldName();
            cmf.Type = tableInfo.GetCodeType();

            classType.Members.Add(cmf);
        }

        private static void CreateSetChildsMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmm = new CodeMemberMethod();
                cmm.Attributes = MemberAttributes.FamilyAndAssembly | MemberAttributes.Static;
                cmm.Name = "Set" + item.TableName;
                cmm.Parameters.Add(tableInfo.GetRowCodeType(), "target");
                cmm.Parameters.Add(typeof(string), "childName");
                cmm.Parameters.Add(item.GetRowCodeType(), 1, "childs");

                // invoke setParent
                {
                    var target = new CodeVariableReferenceExpression("target");
                    var childs = new CodeVariableReferenceExpression("childs");
                    var setParent = new CodeMethodReferenceExpression(tableInfo.GetRowCodeTypeExpression(), "setParent");
                    setParent.TypeArguments.Add(tableInfo.GetRowCodeType());
                    setParent.TypeArguments.Add(item.GetRowCodeType());
                    var invokeSetParent = new CodeMethodInvokeExpression(setParent, target, childs);

                    cmm.Statements.AddExpression(invokeSetParent);
                }

                // create and assign child table
                {
                    var target = new CodeVariableReferenceExpression("target");
                    var childName = new CodeVariableReferenceExpression("childName");
                    var childs = new CodeVariableReferenceExpression("childs");
                    var field = item.GetFieldExpression(target);
                    var createFromRows = new CodeMethodReferenceExpression(item.GetCodeTypeExpression(), "createFromRows");
                    var InvokeCreateFromRows = new CodeMethodInvokeExpression(createFromRows, childName, childs);

                    cmm.Statements.AddAssign(field, InvokeCreateFromRows);
                }

                classType.Members.Add(cmm);
            }
        }

        private static void CreateColumnFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in tableInfo.Columns)
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Private;
                cmf.Name = item.GetFieldName();
                cmf.Type = item.GetCodeType();

                classType.Members.Add(cmf);
            }
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

        private static void CreateStaticChildEmptyFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                cmf.Name = item.GetFieldName() + "Empty";
                cmf.Type = item.GetCodeType();
                cmf.InitExpression = new CodeObjectCreateExpression(item.GetCodeType());
                
                classType.Members.Add(cmf);
            }
        }

        private static void CreateColumnProperties(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in tableInfo.Columns)
            {
                var cmp = new CodeMemberProperty();
                cmp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                cmp.Name = item.Name;
                cmp.Type = item.GetCodeType();
               
                if (generationInfo.OmitComment == false)
                {
                    cmp.Comments.AddSummary(item.Comment);
                }
                if (generationInfo.OmitSignatureDate == false)
                {
                    cmp.Comments.Add(CremaCodeGenerator.Creator, item.CreationInfo.ID);
                    cmp.Comments.Add(CremaCodeGenerator.CreatedDateTime, item.CreationInfo.DateTime);
                    cmp.Comments.Add(CremaCodeGenerator.Modifier, item.ModificationInfo.ID);
                    cmp.Comments.Add(CremaCodeGenerator.ModifiedDateTime, item.ModificationInfo.DateTime);
                }
                cmp.HasGet = true;
                cmp.HasSet = false;

                // return field;
                {
                    var field = item.GetFieldExpression();
                    cmp.GetStatements.AddMethodReturn(field);
                }

                classType.Members.Add(cmp);
            }
        }

        private static void CreateChildProperties(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmp = new CodeMemberProperty();
                cmp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                cmp.Name = item.GetPropertyName();
                cmp.Type = item.GetCodeType();
                cmp.HasGet = true;
                cmp.HasSet = false;

                // check null and return defaultField
                {
                    var state = new CodeConditionStatement();
                    var condition = new CodeBinaryOperatorExpression(item.GetFieldExpression(), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null));
                    state.Condition = condition;

                    var staticField = new CodeFieldReferenceExpression(tableInfo.GetRowCodeTypeExpression(), item.GetFieldName() + "Empty");
                    state.TrueStatements.AddMethodReturn(staticField);

                    cmp.GetStatements.Add(state);
                }

                // return field;
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

            var cmp = new CodeMemberProperty();
            cmp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmp.Name = "parent";
            cmp.Type = tableInfo.GetParentRowCodeType();
            cmp.HasGet = true;
            cmp.HasSet = false;

            // return super.parentInternal;
            {
                var parentInternal = new CodePropertyReferenceExpression(thisRef, "parentInternal");
                var parent = new CodeCastExpression(tableInfo.GetParentRowCodeType(), parentInternal);
                cmp.GetStatements.AddMethodReturn(parent);
            }

            classType.Members.Add(cmp);
        }

        private static void CreateTableProperty(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            var cmp = new CodeMemberProperty();
            cmp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmp.Name = "table";
            cmp.Type = tableInfo.GetCodeType();
            cmp.HasGet = true;
            cmp.HasSet = false;

            // return field
            {
                var field = tableInfo.GetFieldExpression();
                cmp.GetStatements.AddMethodReturn(field);
            }

            classType.Members.Add(cmp);
        }
    }
}
