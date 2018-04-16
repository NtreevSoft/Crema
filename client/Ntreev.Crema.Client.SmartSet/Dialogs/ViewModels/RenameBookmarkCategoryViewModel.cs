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

using System.Linq;
using Ntreev.ModernUI.Framework.Properties;
using System;
using System.Threading.Tasks;
using Ntreev.Library.ObjectModel;
using System.Windows.Threading;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.Library.IO;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;

namespace Ntreev.Crema.Client.SmartSet.Dialogs.ViewModels
{
    class RenameBookmarkCategoryViewModel : RenameAppViewModel
    {
        private readonly string parentPath;
        private readonly string[] categoryPaths;

        public RenameBookmarkCategoryViewModel(string categoryPath, string[] categoryPaths)
            : base(new CategoryName(categoryPath).Name)
        {
            this.parentPath = new CategoryName(categoryPath).ParentPath;
            this.categoryPaths = categoryPaths;
        }

        protected override bool VerifyRename(string newName)
        {
            if (this.categoryPaths.Contains($"{this.parentPath}{this.NewName}{PathUtility.SeparatorChar}") == true)
                return false;
            return true;
        }
    }
}