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
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library;

namespace Ntreev.Crema.Client.Development.Dialogs.ViewModels
{
    public class CremaMakerViewModel : ModalDialogBase
    {
        private static readonly string makerPath = new FileInfo(Path.Combine(AppInfo.StartupPath, "..\\cremamaker\\cremamaker.exe")).FullName;
        private readonly ICremaHost cremaHost;
        //private readonly ICremaConfigService configs;
        private string address;
        private string filename;
        private TagInfo tags = TagInfo.All;

        public CremaMakerViewModel(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            //this.configs = this.cremaHost.GetService(typeof(ICremaConfigService)) as ICremaConfigService;
            this.DisplayName = "바이너리 만들기";
            this.address = cremaHost.Address;

            if (this.cremaHost.Configs.TryParse<string>(this.GetType(), "filename", out var filename) == true)
            {
                this.filename = filename;
            }
            if (this.cremaHost.Configs.TryParse<TagInfo>(this.GetType(), "tags", out var tags) == true)
            {
                this.tags = tags;
            }
        }

        public void SelectFileName()
        {
            var dialog = new CommonSaveFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("crema data", "*.dat"));

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.FileName = dialog.FileName;
            }
        }

        public void Make()
        {
            try
            {
                this.BeginProgress();
                var args = string.Join(" ", this.address, "\"" + this.filename + "\"", "/dl", this.tags);
                System.Diagnostics.Process.Start(makerPath, args);
                this.cremaHost.Configs[this.GetType(), "filename"] = this.filename;
                this.cremaHost.Configs[this.GetType(), "tags"] = this.tags;
                this.TryClose(true);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
            finally
            {
                this.EndProgress();
            }
        }

        public string FileName
        {
            get { return this.filename; }
            set
            {
                this.filename = value;
                this.NotifyOfPropertyChange(nameof(this.FileName));
                this.NotifyOfPropertyChange(nameof(this.CanMake));
            }
        }

        public TagInfo Tags
        {
            get { return this.tags; }
            set
            {
                this.tags = value;
                this.NotifyOfPropertyChange(nameof(this.Tags));
                this.NotifyOfPropertyChange(nameof(this.CanMake));
            }
        }

        public bool CanMake
        {
            get
            {
                if (this.IsProgressing == true)
                    return false;
                if (File.Exists(makerPath) == false)
                    return false;
                if (this.tags == TagInfo.Unused)
                    return false;
                return string.IsNullOrEmpty(this.filename) == false;
            }
        }
    }
}
