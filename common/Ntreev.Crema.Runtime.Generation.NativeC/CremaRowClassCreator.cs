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
using Ntreev.Crema.Runtime.Generation.NativeC.CodeDom;

namespace Ntreev.Crema.Runtime.Generation.NativeC
{
    static class CremaRowClassCreator
    {
        public static void Create(CodeNamespace codeNamespace, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var classType = new CodeTypeDeclaration();
            codeNamespace.Types.Add(classType);

            classType.IsClass = true;
            classType.Name = tableInfo.GetRowClassName();
            classType.BaseTypes.Add(generationInfo.BaseNamespace, "CremaRow");

            CreateColumnFields(classType, tableInfo, generationInfo);
            CreateTableField(classType, tableInfo);
            CreateChildFields(classType, tableInfo, generationInfo);
            CreateStaticChildEmptyFields(classType, tableInfo, generationInfo);
            CreateParentField(classType, tableInfo);
            CreateConstructor(classType, tableInfo, generationInfo);
            CreateSetChildsMethod(classType, tableInfo, generationInfo);
        }

        private static void CreateConstructor(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            var cc = new CodeConstructor();

            cc.Attributes = MemberAttributes.Public;
            cc.Parameters.Add(generationInfo.ReaderNamespace, "irow&", "row");
            cc.Parameters.Add(tableInfo.GetCodeType(CodeType.Pointer), "table");

            cc.AddConstructorStatement(string.Format("{0}(row)", CodeGenerationInfo.CremaRowName));
            //cc.AddConstructorStatement("Table(*table)");

            //foreach (var item in tableInfo.Columns)
            //{
            //    CodeBinaryOperatorExpression cbor = new CodeBinaryOperatorExpression();
            //    cbor.Left = new CodeVariableReferenceExpression("propertyName");
            //    cbor.Operator = CodeBinaryOperatorType.ValueEquality;
            //    cbor.Right = new CodePrimitiveExpression(item.Name);

            //    CodeConditionStatement ccs = new CodeConditionStatement(item.GetHasValueMethodExpression());

            //    CodeAssignStatement cas = new CodeAssignStatement();
            //    cas.Left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), item.Name);
            //    cas.Right = item.GetGetValueMethodExpression();

            //    ccs.TrueStatements.Add(cas);

            //    cc.Statements.Add(ccs);
            //}

            //var query = from item in tableInfo.Columns
            //            where item.IsKey
            //            select item;

            ////method.AddStatement(string.Format("cremarow::initialize(row, iniutil::generate_hash({0}, {1}));", query.Count(), string.Join(", ", query)));

            //CodeMethodInvokeExpression generate_hashExp = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("iniutil"), "generate_hash");
            //foreach (var item in query)
            //{
            //    CodeFieldReferenceExpression fieldExp = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), item.Name);
            //    if (item.IsCustomType() == false)
            //        generate_hashExp.Parameters.Add(fieldExp);
            //    else
            //        generate_hashExp.Parameters.Add(new CodeCastExpression(new CodeTypeReference(typeof(int)), fieldExp));
            //}

            // assign table
            {
                var tableField = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Table");
                var tableVar = new CodeVariableReferenceExpression("table");
                cc.Statements.AddAssign(tableField, tableVar);
            }

            // assign fields
            {
                int index = 0;
                foreach (var item in tableInfo.Columns)
                {
                    var cas = new CodeAssignStatement();
                    cas.Left = item.GetFieldExpression();
                    cas.Right = item.GetGetValueMethodExpression(index, false);

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

            //CodeMethodInvokeExpression methodExp = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("cremarow"), "initialize");

            //methodExp.Parameters.Add(new CodeVariableReferenceExpression("row"));
            //methodExp.Parameters.Add(generate_hashExp);

            //cc.Statements.Add(new CodeExpressionStatement(methodExp));

            //var query = from item in columns
            //            where item.IsKey
            //            select item.Name;

            //method.AddStatement(string.Format("cremarow::initialize(row, iniutil::generate_hash({0}, {1}));", query.Count(), string.Join(", ", query)));

            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                // check null and return defaultField
                {
                    var state = new CodeConditionStatement();
                    var condition = new CodeBinaryOperatorExpression(item.GetFieldExpression(), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null));
                    state.Condition = condition;

                    var codeTypeRef = new CodeTypeReferenceExpression(tableInfo.GetRowCodeType(CodeType.None));
                    var staticField = new CodeFieldReferenceExpression(codeTypeRef, item.GetFieldName() + "Empty");
                    state.TrueStatements.AddAssignReference(item.GetFieldExpression(), staticField);

                    cc.Statements.Add(state);
                }

                // return field;
                //{
                //    var fieldExp = item.GetFieldExpression();
                //    cmp.GetStatements.AddMethodReturn(fieldExp, CodeType.None);
                //}

                //classType.Members.Add(cmp);
            }

            // invoke SetKey method
            {
                var methodRefExp = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "SetKey");
                var methodInvokeExp = new CodeMethodInvokeExpression(methodRefExp);

                foreach (var item in tableInfo.Columns)
                {
                    if (item.IsKey == true)
                    {
                        methodInvokeExp.Parameters.Add(item.GetFieldExpression());
                    }
                }
                cc.Statements.Add(methodInvokeExp);
            }

            classType.Members.Add(cc);
        }

        private static void CreateColumnFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in tableInfo.Columns)
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Public;
                cmf.Name = item.Name;
                cmf.Type = item.GetCodeType(CodeType.None);

                cmf.InitExpression = item.GetInitExpression();

                if (generationInfo.NoComment == false)
                {
                    cmf.Comments.AddSummary(item.Comment);
                }
                if (generationInfo.NoChanges == false)
                {
                    cmf.Comments.Add(CremaSchema.Creator, item.CreationInfo.ID);
                    cmf.Comments.Add(CremaSchema.CreatedDateTime, item.CreationInfo.DateTime);
                    cmf.Comments.Add(CremaSchema.Modifier, item.ModificationInfo.ID);
                    cmf.Comments.Add(CremaSchema.ModifiedDateTime, item.ModificationInfo.DateTime);
                }
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
                cmp.Type = item.GetPropertyType();

                if (generationInfo.NoComment == false)
                {
                    cmp.Comments.AddSummary(item.Comment);
                }
                if (generationInfo.NoChanges == false)
                {
                    cmp.Comments.Add(CremaSchema.Creator, item.CreationInfo.ID);
                    cmp.Comments.Add(CremaSchema.CreatedDateTime, item.CreationInfo.DateTime);
                    cmp.Comments.Add(CremaSchema.Modifier, item.ModificationInfo.ID);
                    cmp.Comments.Add(CremaSchema.ModifiedDateTime, item.ModificationInfo.DateTime);
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

        private static void CreateTableField(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            var cmf = new CodeMemberField();
            cmf.Attributes = MemberAttributes.Public;
            cmf.Name = "Table";
            cmf.Type = tableInfo.GetCodeType(CodeType.Pointer | CodeType.Const);

            classType.Members.Add(cmf);
        }

        private static void CreateSetChildsMethod(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmm = new CodeMemberMethod();
                cmm.Attributes = MemberAttributes.FamilyAndAssembly | MemberAttributes.Static;
                cmm.Name = tableInfo.TableName + "Set" + item.TableName;
                cmm.Parameters.Add(tableInfo.GetRowCodeType(CodeType.Pointer), "target");
                var arrayType = new CodeTypeReference(item.GetRowCodeType(CodeType.Pointer), 1);
                arrayType.SetCodeType(CodeType.Const | CodeType.Reference);
                var childNameType = new CodeTypeReference(typeof(string));
                childNameType.SetCodeType(CodeType.Const | CodeType.Reference);
                cmm.Parameters.Add(childNameType, "childName");
                cmm.Parameters.Add(arrayType, "childs");

                {
                    var methodRefExp = new CodeMethodReferenceExpression(null, "SetParent");
                    //methodRefExp.TypeArguments.Add(tableInfo.GetRowCodeType());
                    //methodRefExp.TypeArguments.Add(item.GetRowCodeType());

                    var methodInvokeExp = new CodeMethodInvokeExpression(methodRefExp, new CodeVariableReferenceExpression("target"), new CodeVariableReferenceExpression("childs"));

                    cmm.Statements.AddExpression(methodInvokeExp);
                }

                {
                    var target = new CodeVariablePointerExpression("target");
                    var childName = new CodeVariablePointerExpression("childName");
                    var childs = new CodeVariableReferenceExpression("childs");
                    var targetField = item.GetFieldExpression(target);
                    var targetInstance = new CodeObjectCreateExpression(item.GetCodeType(CodeType.None), childName, childs);
                    cmm.Statements.AddAssign(targetField, targetInstance);
                }

                classType.Members.Add(cmm);
            }
        }

        private static void CreateChildFields(CodeTypeDeclaration classType, TableInfo tableInfo, CodeGenerationInfo generationInfo)
        {
            foreach (var item in generationInfo.GetChilds(tableInfo))
            {
                var cmf = new CodeMemberField();
                cmf.Attributes = MemberAttributes.Public;
                cmf.Name = item.GetFieldName();
                cmf.Type = item.GetCodeType(CodeType.Pointer | CodeType.Const);

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
                cmf.Type = item.GetCodeType(CodeType.None);
                //cmf.InitExpression = new CodeObjectCreateExpression(item.GetCodeType(CodeType.None));

                classType.Members.Add(cmf);
            }
        }

        private static void CreateParentField(CodeTypeDeclaration classType, TableInfo tableInfo)
        {
            if (string.IsNullOrEmpty(tableInfo.ParentName) == true)
                return;

            var cmp = new CodeMemberField();
            cmp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmp.Name = "Parent";
            cmp.Type = tableInfo.GetParentRowCodeType(CodeType.Pointer | CodeType.Const);

            // statement
            //{
            //    var parentExp = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "ParentInternal");
            //    var castExp = new CodeCastExpression(tableInfo.GetParentRowCodeType(CodeType.Const | CodeType.Pointer), parentExp);
            //    cmp.GetStatements.AddMethodReturn(castExp);
            //}

            classType.Members.Add(cmp);
        }
    }
}
