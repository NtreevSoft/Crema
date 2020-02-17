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
using System.Linq;
using System.Runtime.Serialization;
using Ntreev.Library;

namespace Ntreev.Crema.Data
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    [Serializable]
    public class TableDetailInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CategoryPath { get; set; }

        [DataMember]
        public int TableRowsCount { get; set; }

        [DataMember]
        public int TableEnabledRowsCount { get; set; }

        [DataMember]
        public int TableDisabledRowsCount { get; set; }

        [DataMember]
        public int TableAllTagRowsCount { get; set; }

        [DataMember]
        public int TableServerTagRowsCount { get; set; }

        [DataMember]
        public int TableClientTagRowsCount { get; set; }

        [DataMember]
        public int TableUnusedTagRowsCount { get; set; }

        [DataMember]
        public int ColumnsCount { get; set; }

        [DataMember]
        public int ColumnsAllTagCount { get; set; }

        [DataMember]
        public int ColumnsServerTagCount { get; set; }

        [DataMember]
        public int ColumnsClientTagCount { get; set; }

        [DataMember]
        public int ColumnsUnusedTagCount { get; set; }

        public string TableName
        {
            get
            {
                if (this.Name.Contains('.') == true)
                {
                    return StringUtility.Split(this.Name, '.')[1];
                }
                return this.Name;
            }
        }

        public bool Equals(TableDetailInfo other)
        {
            return Name == other.Name &&
                   CategoryPath == other.CategoryPath &&
                   TableRowsCount == other.TableRowsCount && 
                   TableEnabledRowsCount == other.TableEnabledRowsCount &&
                   TableDisabledRowsCount == other.TableDisabledRowsCount && 
                   TableAllTagRowsCount == other.TableAllTagRowsCount &&
                   TableServerTagRowsCount == other.TableServerTagRowsCount && 
                   TableClientTagRowsCount == other.TableClientTagRowsCount &&
                   TableUnusedTagRowsCount == other.TableUnusedTagRowsCount && 
                   ColumnsCount == other.ColumnsCount &&
                   ColumnsAllTagCount == other.ColumnsAllTagCount &&
                   ColumnsServerTagCount == other.ColumnsServerTagCount &&
                   ColumnsClientTagCount == other.ColumnsClientTagCount &&
                   ColumnsUnusedTagCount == other.ColumnsUnusedTagCount;
        }

        public override bool Equals(object obj)
        {
            return obj is TableDetailInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(Name) ^
                   HashUtility.GetHashCode(CategoryPath) ^
                   HashUtility.GetHashCode(TableRowsCount) ^
                   HashUtility.GetHashCode(TableEnabledRowsCount) ^
                   HashUtility.GetHashCode(TableDisabledRowsCount) ^
                   HashUtility.GetHashCode(TableAllTagRowsCount) ^
                   HashUtility.GetHashCode(TableServerTagRowsCount) ^
                   HashUtility.GetHashCode(TableClientTagRowsCount) ^
                   HashUtility.GetHashCode(TableUnusedTagRowsCount) ^
                   HashUtility.GetHashCode(ColumnsCount) ^
                   HashUtility.GetHashCode(ColumnsAllTagCount) ^
                   HashUtility.GetHashCode(ColumnsServerTagCount) ^
                   HashUtility.GetHashCode(ColumnsClientTagCount) ^
                   HashUtility.GetHashCode(ColumnsUnusedTagCount);
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(TableRowsCount)}: {TableRowsCount}";
        }
    }
}
