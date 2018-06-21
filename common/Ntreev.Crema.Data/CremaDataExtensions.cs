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

using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data
{
    public static class CremaDataExtensions
    {
        // mac의 mono 환경에서 DataTable.AsDataView 메소드가 없다고 예외가 발생함.
        // 따라서 플랫폼에 따라서 다르게 호출되도록 구현을 해놓음.
        // 특히 보기 좋게 하려고 아래와 같이 한줄로 표현할 경우 아예 메소드 호출이 되질않음
        // var view = Environment.OSVersion.Platform == PlatformID.Unix ? new DataView(template.InternalObject) : template.InternalObject.AsDataView();
        public static DataView AsDataView(this CremaDataTable table)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                var view = AsDataViewUnix(table.InternalObject);
                view.Sort = string.Join(",", table.Columns.Select(item => item.ColumnName));
                return view;
            }
            else
            {
                var view = AsDataViewNormal(table.InternalObject);
                view.Sort = string.Join(",", table.Columns.Select(item => item.ColumnName));
                return view;
            }
        }

        public static DataView AsDataView(this CremaDataType type)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                var view = AsDataViewUnix(type.InternalObject);
                view.Sort = type.InternalObject.columnName.ColumnName;
                return view;
            }
            else
            {
                var view = AsDataViewNormal(type.InternalObject);
                view.Sort = type.InternalObject.columnName.ColumnName;
                return view;
            }
        }

        public static DataView AsDataView(this CremaTemplate template)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                var view = AsDataViewUnix(template.InternalObject);
                return view;
            }
            else
            {
                var view = AsDataViewNormal(template.InternalObject);
                return view;
            }
        }

        private static DataView AsDataViewUnix(InternalTableBase dataTable)
        {
            return new DataView(dataTable);
        }

        private static DataView AsDataViewNormal(InternalTableBase dataTable)
        {
            return dataTable.AsDataView();
        }
    }
}
