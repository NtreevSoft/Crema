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
    class CodeGenMenuItemViewModel : MenuItemBase
    {
        private static string generatorPath = new FileInfo(Path.Combine(AppInfo.StartupPath, "..\\cremacodegen\\cremacodegen.exe")).FullName;
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public CodeGenMenuItemViewModel(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.DisplayName = "코드 생성하기...";
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.cremaHost.IsOpened == true && File.Exists(generatorPath) == true;
        }

        protected override void OnExecute(object parameter)
        {
            var dialog = new CremaCodeGenViewModel(this.cremaHost);
            dialog.ShowDialog();
        }

        public static void Generate(string address, int port, string languageType, string tags)
        {
            var dialog = new CommonOpenFileDialog() { IsFolderPicker = true, };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Generate(dialog.FileName, address, port, languageType, tags);
            }
        }

        public static void Generate(string path, string address, int port, string languageType, string tags)
        {
            var args = string.Join(" ", address, port, "\"" + path + "\"", languageType, "/dl", tags);
            System.Diagnostics.Process.Start(generatorPath, args);
        }
    }
}
