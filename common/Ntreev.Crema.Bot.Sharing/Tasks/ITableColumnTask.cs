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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Ntreev.Crema.Services.Random;
using System.Threading.Tasks;

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(ITableColumnTask))]
    [TaskClass]
    public class ITableColumnTask : ITaskProvider
    {
        public void InvokeTask(TaskContext context)
        {
            var column = context.Target as ITableColumn;
            if (context.IsCompleted(column) == true)
            {
                column.Dispatcher.Invoke(() =>
                {
                    var template = column.Template;
                    if (object.Equals(context.State, System.Data.DataRowState.Detached) == true)
                    {
                        try
                        {
                            template.EndNew(context.Authentication, column);
                        }
                        catch
                        {

                        }
                    }
                });
                context.State = null;
                context.Pop(column);
            }
        }

        public Type TargetType
        {
            get { return typeof(ITableColumn); }
        }

        public bool IsEnabled
        {
            get { return false; }
        }

        [TaskMethod(Weight = 1)]
        public void Delete(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                if (object.Equals(context.State, System.Data.DataRowState.Detached) == false)
                {
                    column.Delete(context.Authentication);
                    context.State = System.Data.DataRowState.Deleted;
                    context.Complete(column);
                }
            });
        }

        [TaskMethod]
        public void SetIndex(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var index = RandomUtility.Next(column.Template.Count);
                column.SetIndex(context.Authentication, index);
            });
        }

        [TaskMethod(Weight = 20)]
        public void SetIsKey(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var isKey = RandomUtility.NextBoolean();
                column.SetIsKey(context.Authentication, isKey);
            });
        }

        [TaskMethod]
        public void SetIsUnique(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var isUnique = RandomUtility.NextBoolean();
                if (Verify(isUnique) == false)
                    return;
                column.SetIsUnique(context.Authentication, isUnique);
            });

            bool Verify(bool isUnique)
            {
                if (context.AllowException == true)
                    return true;
                if (isUnique == true && column.DataType == typeof(bool).GetTypeName())
                    return false;
                var template = column.Template;
                if (isUnique == false && column.IsKey == true && template.Count(item => item.IsKey) == 1)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void SetName(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var columnName = RandomUtility.NextIdentifier();
                column.SetName(context.Authentication, columnName);
            });
        }

        [TaskMethod]
        public void SetDataType(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var template = column.Template;
                if (RandomUtility.Within(75) == true)
                {
                    var dataType = CremaDataTypeUtility.GetBaseTypeNames().Random();
                    if (Verify(dataType) == false)
                        return;
                    column.SetDataType(context.Authentication, dataType);
                }
                else
                {
                    var dataType = template.SelectableTypes.Random();
                    if (Verify(dataType) == false)
                        return;
                    column.SetDataType(context.Authentication, dataType);
                }
            });

            bool Verify(string dataType)
            {
                if (context.AllowException == true)
                    return true;
                if (column.AutoIncrement == true && CremaDataTypeUtility.CanUseAutoIncrement(column.DataType) == false)
                    return false;
                return true;
            }
        }

        //[TaskMethod]
        public void SetDefaultValue(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var defaultValue = column.GetRandomString();
                column.SetDefaultValue(context.Authentication, defaultValue);
            });
        }

        [TaskMethod]
        public void SetComment(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var comment = RandomUtility.NextString();
                column.SetComment(context.Authentication, comment);
            });
        }

        [TaskMethod]
        public void SetAutoIncrement(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var autoIncrement = RandomUtility.NextBoolean();
                if (Verify(autoIncrement) == false)
                    return;
                column.SetAutoIncrement(context.Authentication, autoIncrement);
            });

            bool Verify(bool autoIncrement)
            {
                if (context.AllowException == true)
                    return true;
                if (autoIncrement == true && CremaDataTypeUtility.CanUseAutoIncrement(column.DataType) == false)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void SetTags(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var tags = (TagInfo)TagInfoUtility.Names.Random();
                column.SetTags(context.Authentication, tags);
            });
        }

        [TaskMethod]
        public void SetIsReadOnly(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                if (column.IsKey == false || RandomUtility.Within(55) == true)
                {
                    var isReadOnly = RandomUtility.NextBoolean();
                    column.SetIsReadOnly(context.Authentication, isReadOnly);
                }
            });
        }

        [TaskMethod]
        public void SetAllowNull(ITableColumn column, TaskContext context)
        {
            column.Dispatcher.Invoke(() =>
            {
                var allowNull = RandomUtility.NextBoolean();
                column.SetAllowNull(context.Authentication, allowNull);
            });
        }
    }
}
