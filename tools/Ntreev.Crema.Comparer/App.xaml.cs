﻿//Released under the MIT License.
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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ntreev.Crema.Comparer
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            RegisterLicense("Xceed_Wpf_DataGrid", item => Xceed.Wpf.DataGrid.Licenser.LicenseKey = item);
            RegisterLicense("Xceed_Wpf_Toolkit", item => Xceed.Wpf.Toolkit.Licenser.LicenseKey = item);

            void RegisterLicense(string licenseName, Action<string> action)
            {
                var type = typeof(App);
                var rm = new ResourceManager($"{type.Namespace}.Properties.Resources", type.Assembly);
                var obj = (byte[])rm.GetObject(licenseName);
                if (obj != null)
                {
                    var license = Encoding.UTF8.GetString(obj);
                    if (string.IsNullOrEmpty(license) == false)
                    {
                        action(license);
                    }
                }
            }
        }
    }
}
