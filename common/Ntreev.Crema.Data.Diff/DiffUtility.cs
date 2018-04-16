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

using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Diff
{
    public static class DiffUtility
    {
        public const string DiffStateKey = "__DiffState__";
        public const string DiffFieldsKey = "__DiffFields__";
        public const string DiffIDKey = "__DiffID__";

        public const string DiffEnabledKey = "__DiffEnabled__";
        public const string DiffDummyKey = "__DiffDummy__";

        public static DiffState GetDiffState(object item)
        {
            if (item is DataRowView dataRowView)
            {
                return GetDiffState(dataRowView.Row);
            }
            else if (item is DataRow dataRow)
            {
                var text = dataRow.Field<string>(DiffStateKey);
                if (Enum.TryParse(text, out DiffState d) == true)
                    return d;
            }
            else if (item is PropertyCollection props)
            {
                if (props.ContainsKey(DiffStateKey) && props[DiffStateKey] is string text)
                {
                    if (Enum.TryParse(text, out DiffState d) == true)
                        return d;
                }
            }
            else if (item is DataColumn dataColumn)
            {
                return GetDiffState(dataColumn.ExtendedProperties);
            }
            else if (item is DataTable dataTable)
            {
                return GetDiffState(dataTable.ExtendedProperties);
            }
            else if (item is CremaDataRow cremaDataRow)
            {
                return GetDiffState(cremaDataRow.InternalObject);
            }
            else if (item is CremaDataColumn cremaDataColumn)
            {
                return GetDiffState(cremaDataColumn.InternalObject);
            }
            else if (item is CremaDataTypeMember cremaTypeMember)
            {
                return GetDiffState(cremaTypeMember.InternalObject);
            }
            else if (item is CremaDataTable cremaDataTable)
            {
                return GetDiffState(cremaDataTable.InternalObject);
            }
            else if (item is CremaDataType cremaDataType)
            {
                return GetDiffState(cremaDataType.InternalObject);
            }
            else if (item is CremaTemplate cremaTemplate)
            {
                return GetDiffState(cremaTemplate.InternalObject);
            }
            else if (item is CremaTemplateColumn cremaTemplateColumn)
            {
                return GetDiffState(cremaTemplateColumn.InternalObject);
            }

            return DiffState.Unchanged;
        }

        public static string[] GetDiffFields(object item)
        {
            if (item is DataRowView dataRowView)
            {
                return GetDiffFields(dataRowView.Row);
            }
            else if (item is DataRow dataRow)
            {
                if (dataRow.Field<string>(DiffFieldsKey) is string text)
                    return text.Split(',');
                return new string[] { };
            }
            else if (item is CremaDataRow cremaDataRow)
            {
                return GetDiffFields(cremaDataRow.InternalObject);
            }
            else if (item is CremaDataTypeMember cremaTypeMember)
            {
                return GetDiffFields(cremaTypeMember.InternalObject);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static void Copy(object sourceItem, object destItem)
        {
            Copy(sourceItem, destItem, false);
        }

        public static void Copy(object sourceItem, object destItem, bool isBeingEdited)
        {
            if (HasError(sourceItem) == true)
                throw new ArgumentException();

            if (destItem is IEditableObject editable)
            {
                try
                {
                    if (isBeingEdited == false)
                        editable.BeginEdit();

                    if (destItem is ICustomTypeDescriptor descriptor1 && sourceItem is ICustomTypeDescriptor descriptor2)
                    {
                        var props1 = descriptor1.GetProperties();
                        var props2 = descriptor2.GetProperties();

                        foreach (PropertyDescriptor prop2 in props2)
                        {
                            var prop1 = props1[prop2.Name];
                            if (prop1 == null)
                                continue;

                            if (prop1.Name == CremaSchema.__RelationID__ || prop1.Name == CremaSchema.__ParentID__)
                                continue;

                            if (prop1.IsReadOnly == false && CremaDataTypeUtility.IsBaseType(prop1.PropertyType))
                            {
                                prop1.SetValue(destItem, prop2.GetValue(sourceItem));
                            }
                        }
                    }
                    if (isBeingEdited == false)
                        editable.EndEdit();
                }
                catch
                {
                    if (isBeingEdited == false)
                        editable.CancelEdit();
                    throw;
                }
            }
        }

        public static void Empty(object dataItem)
        {
            Empty(dataItem, false);
        }

        public static void Empty(object dataItem, bool isBeingEdited)
        {
            if (dataItem is IEditableObject editable)
            {
                try
                {
                    if (isBeingEdited == false)
                        editable.BeginEdit();

                    if (dataItem is ICustomTypeDescriptor descriptor)
                    {
                        var props = descriptor.GetProperties();
                        for (var i = 0; i < props.Count; i++)
                        {
                            var prop = props[i];
                            if (prop.IsBrowsable == false || prop.Name == CremaSchema.Index)
                                continue;
                            if (prop.Name == CremaSchema.__RelationID__ || prop.Name == CremaSchema.__ParentID__)
                                continue;
                            if (CremaDataTypeUtility.IsBaseType(prop.PropertyType) == false)
                                continue;
                            prop.ResetValue(dataItem);
                        }

                        SetItemEnabled(dataItem, false);
                    }

                    if (isBeingEdited == false)
                        editable.EndEdit();
                }
                catch
                {
                    if (isBeingEdited == false)
                        editable.CancelEdit();
                    throw;
                }
            }
        }

        public static IEnumerable<object> GetChilds(object dataItem)
        {
            if (dataItem is ICustomTypeDescriptor descriptor)
            {
                var props = descriptor.GetProperties();
                for (var i = 0; i < props.Count; i++)
                {
                    var prop = props[i];
                    if (typeof(IBindingList).IsAssignableFrom(prop.PropertyType) == true)
                    {
                        if (prop.GetValue(dataItem) is IBindingList bindingList)
                        {
                            foreach (var item in bindingList)
                            {
                                yield return item;
                            }
                        }
                    }
                }
            }
        }

        public static object[] GetFields(object dataItem)
        {
            if (dataItem is ICustomTypeDescriptor descriptor)
            {
                var props = descriptor.GetProperties();
                var itemArray = new object[props.Count];
                for (var i = 0; i < props.Count; i++)
                {
                    var prop = props[i];
                    itemArray[i] = prop.GetValue(dataItem);
                }
                return itemArray;
            }
            throw new ArgumentException(nameof(dataItem));
        }

        public static object GetField(object dataItem, string fieldName)
        {
            if (dataItem is ICustomTypeDescriptor descriptor)
            {
                var prop = descriptor.GetProperties()[fieldName];
                return prop.GetValue(dataItem);

            }
            throw new ArgumentException(nameof(dataItem));
        }

        public static void Copy(object dataItem, object[] itemArray)
        {
            if (dataItem is IEditableObject editable)
            {
                try
                {
                    editable.BeginEdit();

                    if (dataItem is ICustomTypeDescriptor descriptor)
                    {
                        var props = descriptor.GetProperties();
                        for (var i = 0; i < props.Count; i++)
                        {
                            var prop = props[i];
                            if (prop.IsReadOnly == false)
                            {
                                prop.SetValue(dataItem, itemArray[i]);
                            }
                        }
                    }

                    editable.EndEdit();
                }
                catch
                {
                    editable.CancelEdit();
                    throw;
                }
            }
        }

        public static void SetField(object dataItem, string fieldName, object field)
        {
            if (dataItem is IEditableObject editable)
            {
                try
                {
                    editable.BeginEdit();
                    if (dataItem is ICustomTypeDescriptor descriptor)
                    {
                        var prop = descriptor.GetProperties()[fieldName];
                        prop.SetValue(dataItem, field);
                    }
                    editable.EndEdit();
                }
                catch
                {
                    editable.CancelEdit();
                    throw;
                }
            }
        }

        public static object GetItemKey(object item)
        {
            if (item is CremaDataTypeMember dataTypeMember)
            {
                return dataTypeMember.MemberID;
            }
            else if (item is DataRowView dataRowView)
            {
                if (dataRowView.Row is InternalDataRow dataRow)
                {
                    return dataRow.Table.Rows.IndexOf(dataRow);
                }
                else if (dataRowView.Row is InternalDataTypeMember typeMember)
                {
                    if (typeMember[CremaSchema.ID] is Guid id)
                        return id;
                    return typeMember[CremaSchema.Index];
                }
                else if (dataRowView.Row is InternalTemplateColumn templateColumn)
                {
                    if (templateColumn[CremaSchema.ID] is Guid id)
                        return id;
                    return templateColumn[CremaSchema.Index];
                }
            }
            throw new InvalidOperationException();
        }

        public static bool GetItemEnabled(object item)
        {
            if (item is DataRowView dataRowView)
            {
                return GetItemEnabled(dataRowView.Row);
            }
            else if (item is DataRow dataRow)
            {
                return (bool)dataRow[DiffEnabledKey];
            }
            throw new NotImplementedException();
        }

        public static string GetListName(object item)
        {
            if (item is DataRowView rowView)
            {
                return GetListName(rowView.Row);
            }
            else if (item is DataRow dataRow)
            {
                if (dataRow.Table is InternalTableBase tableBase)
                {
                    return tableBase.LocalName;
                }
                return dataRow.Table.TableName;
            }
            throw new NotImplementedException();
        }

        public static bool HasDiffState(object item)
        {
            if (item is DataRowView)
            {
                return true;
            }
            else if (item is DataRow)
            {
                return true;
            }
            else if (item is CremaDataRow)
            {
                return true;
            }
            else if (item is CremaDataColumn)
            {
                return true;
            }
            else if (item is CremaDataTypeMember)
            {
                return true;
            }
            else if (item is CremaDataTable)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool HasError(object item)
        {
            if (item is IDataErrorInfo errorInfo)
            {
                if (errorInfo.Error != string.Empty)
                    return true;

                if (item is ICustomTypeDescriptor descriptor)
                {
                    foreach (PropertyDescriptor prop in descriptor.GetProperties())
                    {
                        if (CremaDataTypeUtility.IsBaseType(prop.PropertyType) == true && errorInfo[prop.Name] != string.Empty)
                            return true;
                    }
                }
            }
            return false;
        }

        internal static string GetOriginalName(string name)
        {
            return Regex.Replace(name, $"(^|[.])(?:{DiffUtility.DiffDummyKey})([^.]+)", "$1$2");
        }

        internal static void SetDiffState(object item, DiffState state)
        {
            if (item is DataRowView dataRowView)
            {
                SetDiffState(dataRowView.Row, state);
            }
            else if (item is DataRow dataRow)
            {
                var omitSignatureDate = false;
                var readOnly = false;
                var table = dataRow.Table as InternalTableBase;
                if (table != null)
                {
                    omitSignatureDate = table.OmitSignatureDate;
                    table.OmitSignatureDate = true;
                    readOnly = table.ReadOnly;
                    table.ReadOnly = false;
                }
                var oldField = $"{dataRow[DiffStateKey]}";
                var newField = $"{state}";
                if (oldField != newField)
                {
                    dataRow.SetField(DiffStateKey, newField);
                }
                if (table != null)
                {
                    table.OmitSignatureDate = omitSignatureDate;
                    table.ReadOnly = readOnly;
                }
            }
            else if (item is DataColumn dataColumn)
            {
                dataColumn.ExtendedProperties[DiffStateKey] = $"{state}";
            }
            else if (item is DataTable dataTable)
            {
                dataTable.ExtendedProperties[DiffStateKey] = $"{state}";
            }
            else if (item is CremaDataRow cremaDataRow)
            {
                SetDiffState(cremaDataRow.InternalObject, state);
            }
            else if (item is CremaDataColumn cremaDataColumn)
            {
                SetDiffState(cremaDataColumn.InternalObject, state);
            }
            else if (item is CremaDataTypeMember cremaTypeMember)
            {
                SetDiffState(cremaTypeMember.InternalObject, state);
            }
            else if (item is CremaDataTable cremaDataTable)
            {
                SetDiffState(cremaDataTable.InternalObject, state);
            }
            else if (item is CremaDataType cremaDataType)
            {
                SetDiffState(cremaDataType.InternalObject, state);
            }
            else if (item is CremaTemplate cremaTemplate)
            {
                SetDiffState(cremaTemplate.InternalObject, state);
            }
            else if (item is CremaTemplateColumn cremaTemplateColumn)
            {
                SetDiffState(cremaTemplateColumn.InternalObject, state);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        internal static void SetDiffFields(object item, IEnumerable<string> fields)
        {
            if (item is DataRowView dataRowView)
            {
                SetDiffFields(dataRowView.Row, fields);
            }
            else if (item is DataRow dataRow)
            {
                var omitSignatureDate = false;
                var readOnly = false;
                var table = dataRow.Table as InternalTableBase;
                if (table != null)
                {
                    omitSignatureDate = table.OmitSignatureDate;
                    table.OmitSignatureDate = true;
                    readOnly = table.ReadOnly;
                    table.ReadOnly = false;
                }

                var oldField = $"{dataRow[DiffFieldsKey]}";
                var newField = string.Join(",", fields ?? Enumerable.Empty<string>());
                if (oldField != newField)
                {
                    if (newField != string.Empty)
                        dataRow.SetField(DiffFieldsKey, newField);
                    else
                        dataRow.SetField(DiffFieldsKey, DBNull.Value);
                }

                if (table != null)
                {
                    table.OmitSignatureDate = omitSignatureDate;
                    table.ReadOnly = readOnly;
                }
            }
        }

        internal static void SetItemEnabled(object item, bool isEnabled)
        {
            if (item is DataRowView dataRowView)
            {
                SetItemEnabled(dataRowView.Row, isEnabled);
            }
            else if (item is DataRow dataRow)
            {
                var omitSignatureDate = false;
                var readOnly = false;
                var table = dataRow.Table as InternalTableBase;
                if (table != null)
                {
                    omitSignatureDate = table.OmitSignatureDate;
                    table.OmitSignatureDate = true;
                    readOnly = table.ReadOnly;
                    table.ReadOnly = false;
                }

                if (object.Equals(dataRow[DiffEnabledKey], isEnabled) == false)
                {
                    dataRow.SetField(DiffEnabledKey, isEnabled);
                }

                if (table != null)
                {
                    table.OmitSignatureDate = omitSignatureDate;
                    table.ReadOnly = readOnly;
                }
            }
        }
    }
}
