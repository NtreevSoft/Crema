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

using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Tables.Documents.ViewModels
{
    class FindResultItemViewModel : PropertyChangedBase
    {
        private readonly FindResultInfo resultInfo;
        private readonly ItemName itemName;

        public FindResultItemViewModel(FindResultInfo resultInfo)
        {
            this.resultInfo = resultInfo;
            this.itemName = new ItemName(resultInfo.Path);
        }

        public string CategoryName
        {
            get { return this.itemName.CategoryPath.Trim(PathUtility.SeparatorChar); }
        }

        public string TableName
        {
            get { return this.itemName.Name; }
        }

        public string ColumnName
        {
            get { return this.resultInfo.ColumnName; }
        }

        public int Row
        {
            get { return this.resultInfo.Row; }
        }

        public string Value
        {
            get { return this.resultInfo.Value; }
        }
    }
}
