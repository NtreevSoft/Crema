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

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(ITableRowTask))]
    [TaskClass]
    class ITableRowTask : ITaskProvider
    {
        public void InvokeTask(TaskContext context)
        {
            var row = context.Target as ITableRow;
            row.Dispatcher.Invoke(() =>
            {
                var content = row.Content;
                if (context.IsCompleted(row) == true)
                {
                    if (object.Equals(context.State, System.Data.DataRowState.Detached) == true)
                    {
                        try
                        {
                            if (Verify() == true)
                                content.EndNew(context.Authentication, row);
                        }
                        catch
                        {
                            
                        }
                    }

                    context.State = null;
                    context.Pop(row);
                }
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;

                var content = row.Content;
                var domain = content.Domain;
                var dataSet = domain.Source as CremaDataSet;
                var dataTable = dataSet.Tables[content.Table.Name];

                foreach(var item in dataTable.Columns)
                {
                    if (item.AllowDBNull == false && row[item.ColumnName] == null)
                        return false;
                }

                return true;
            }
        }

        public Type TargetType
        {
            get { return typeof(ITableRow); }
        }

        public bool IsEnabled
        {
            get { return false; }
        }

        [TaskMethod(Weight = 1)]
        public void Delete(ITableRow row, TaskContext context)
        {
            row.Dispatcher.Invoke(() =>
            {
                if (object.Equals(context.State, System.Data.DataRowState.Detached) == false)
                {
                    row.Delete(context.Authentication);
                    context.State = System.Data.DataRowState.Deleted;
                    context.Complete(row);
                }
            });
        }

        [TaskMethod(Weight = 5)]
        public void SetIsEnabled(ITableRow row, TaskContext context)
        {
            if (object.Equals(context.State, System.Data.DataRowState.Detached) == true)
                return;
            row.Dispatcher.Invoke(() =>
            {
                var isEnabled = RandomUtility.NextBoolean();
                row.SetIsEnabled(context.Authentication, isEnabled);
            });
        }

        [TaskMethod(Weight = 5)]
        public void SetTags(ITableRow row, TaskContext context)
        {
            if (object.Equals(context.State, System.Data.DataRowState.Detached) == true)
                return;
            row.Dispatcher.Invoke(() =>
            {
                var tags = TagInfoUtility.Names.Random();
                row.SetTags(context.Authentication, (TagInfo)tags);
            });
        }

        [TaskMethod]
        public void SetField(ITableRow row, TaskContext context)
        {
            if (object.Equals(context.State, System.Data.DataRowState.Detached) == true)
                return;
        }

        [TaskMethod]
        public void MoveLeft(ITableRow row, TaskContext context)
        {
            if (object.Equals(context.State, System.Data.DataRowState.Detached) == true)
                return;
        }
    }
}
