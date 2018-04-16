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

using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using System.Windows;
using System.Windows.Media;
using Ntreev.ModernUI.Framework.ViewModels;

namespace Ntreev.Crema.Client.Base.Dialogs.ViewModels
{
    public class DataBaseItemViewModel : DataBaseListItemBase
    {
        public DataBaseItemViewModel(Authentication authentication, DataBaseInfo dataBaseInfo, object owner)
            : base(authentication, new DataBaseDescriptor(authentication, dataBaseInfo), owner)
        {

        }

        public DataBaseItemViewModel(Authentication authentication, IDataBaseDescriptor descriptor, object owner)
            : base(authentication, new DataBaseDescriptor(authentication, descriptor, false, owner), owner)
        {

        }

        public IConnectionItem ConnectionItem { get; internal set; }

        public Color Color
        {
            get
            {
                if (this.ConnectionItem == null)
                    return Colors.Transparent;
                return this.ConnectionItem.ThemeColor;
            }
        }
    }
}