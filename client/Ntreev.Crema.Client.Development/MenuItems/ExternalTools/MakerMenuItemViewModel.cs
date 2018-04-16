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

using Microsoft.WindowsAPICodePack.Dialogs;
using Ntreev.Crema.Client.Development.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Development.MenuItems.ExternalTools
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(ExternalToolsMenuItemViewModel))]
    class MakerMenuItemViewModel : MenuItemBase
    {
        private static readonly string makerPath = new FileInfo(Path.Combine(AppInfo.StartupPath, "..\\cremamaker\\cremamaker.exe")).FullName;
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public MakerMenuItemViewModel(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.DisplayName = "바이너리 만들기...";
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.cremaHost.IsOpened == true && File.Exists(makerPath) == true;
        }

        protected override void OnExecute(object parameter)
        {
            var dialog = new CremaMakerViewModel(this.cremaHost);
            dialog.ShowDialog();
        }

        public static void Make(string address, int port, string tags)
        {
            var dialog = new CommonSaveFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("crema data", "*.dat"));

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                MakerMenuItemViewModel.Make(dialog.FileName, address, port, tags);
            }
        }

        public static void Make(string filename, string address, int port, string tags)
        {
            var args = string.Join(" ", address, port, "\"" + filename + "\"", "/dl", tags);
            System.Diagnostics.Process.Start(makerPath, args);
        }
    }
}
