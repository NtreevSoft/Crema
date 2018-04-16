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

using Ntreev.ModernUI.Framework.Properties;
using Ntreev.Library.ObjectModel;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Ntreev.Library.Linq;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Tables;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Tables.BrowserItems.ViewModels;
using Ntreev.Library.IO;
using Ntreev.Crema.Client.SmartSet.BrowserItems.ViewModels;

namespace Ntreev.Crema.Client.SmartSet.Dialogs.ViewModels
{
    public class AddBookmarkItemViewModel : ModalDialogAppBase
    {
        private readonly Authentication authentication;
        private readonly string[] items;
        private string targetPath;

        public AddBookmarkItemViewModel(Authentication authentication, string name, string[] targetPaths)
        {
            this.Validate(name, targetPaths);
            this.authentication = authentication;
            this.ItemName = name;
            this.items = TreeViewItemViewModelBuilder.MakeItemList(targetPaths);
            this.TargetPaths = TreeViewItemViewModelBuilder.MakeItemList(targetPaths, true);
            this.targetPath = this.TargetPaths.First();
            this.DisplayName = Properties.Resources.Title_AddBookmark;
        }

        public void Add()
        {
            try
            {
                this.TryClose(true);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }

        public string[] TargetPaths { get; }

        public string TargetPath
        {
            get { return this.targetPath ?? string.Empty; }
            set
            {
                this.targetPath = value;
                this.NotifyOfPropertyChange(nameof(this.TargetPath));
                this.NotifyOfPropertyChange(nameof(this.CanAdd));
            }
        }

        public string ItemName { get; }

        public bool CanAdd
        {
            get
            {
                if (this.IsProgressing == true)
                    return false;

                return this.VerifyAdd(this.TargetPath);
            }
        }

        protected virtual bool VerifyAdd(string targetPath)
        {
            return this.items.Contains(targetPath + this.ItemName) == false;
        }

        private void Validate(string name, string[] targetPaths)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (targetPaths == null)
                throw new ArgumentNullException(nameof(targetPaths));
        }
    }
}
