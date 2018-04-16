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
    public class CremaCodeGenViewModel : ModalDialogBase
    {
        private static readonly string generatorPath = new FileInfo(Path.Combine(AppInfo.StartupPath, "..\\cremacodegen\\cremacodegen.exe")).FullName;
        private static readonly string[] languageTypes = new string[] { "c#", "TypeScript", "c++", };
        private readonly ICremaHost cremaHost;
        //private readonly ICremaConfigService configs;
        private readonly Dictionary<string, string> languageArgs = new Dictionary<string, string>();
        private string address;
        private string outputPath;
        private TagInfo tags = TagInfo.All;
        private string languageType = languageTypes.First();
        private bool isCompilable;
        
        public CremaCodeGenViewModel(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            //this.configs = this.cremaHost.GetService(typeof(ICremaConfigService)) as ICremaConfigService;
            this.DisplayName = "코드 생성하기";
            this.address = cremaHost.Address;

            if (this.cremaHost.Configs.TryParse<string>(this.GetType(), "outputPath", out var outputPath) == true)
            {
                this.outputPath = outputPath;
            }
            if (this.cremaHost.Configs.TryParse<TagInfo>(this.GetType(), "tags", out var tags) == true)
            {
                this.tags = tags;
            }
            if (this.cremaHost.Configs.TryParse<bool>(this.GetType(), "isCompilable", out var isCompilable) == true)
            {
                this.isCompilable = isCompilable;
            }
            if (this.cremaHost.Configs.TryParse<string>(this.GetType(), "languageType", out var languageType) == true)
            {
                this.languageType = languageType;
            }
        }

        public void SelectPath()
        {
            var dialog = new CommonOpenFileDialog() { IsFolderPicker = true, };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.OutputPath = dialog.FileName;
            }
        }

        public void Generate()
        {
            try
            {
                this.BeginProgress();
                var languageArg = this.languageType == "TypeScript" ? "ts" : this.languageType;
                var args = string.Join(" ", this.address, "\"" + this.outputPath + "\"", languageArg, "/dl", this.tags);

                if(this.isCompilable == true)
                    args = string.Join(" ", args, "/c");

                System.Diagnostics.Process.Start(generatorPath, args);
                this.cremaHost.Configs[this.GetType(), "outputPath"] = this.outputPath;
                this.cremaHost.Configs[this.GetType(), "tags"] = this.tags;
                this.cremaHost.Configs[this.GetType(), "isCompilable"] = this.isCompilable;
                this.cremaHost.Configs[this.GetType(), "languageType"] = this.languageType;
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

        public string OutputPath
        {
            get { return this.outputPath; }
            set
            {
                this.outputPath = value;
                this.NotifyOfPropertyChange(nameof(this.OutputPath));
                this.NotifyOfPropertyChange(nameof(this.CanGenerate));
            }
        }

        public TagInfo Tags
        {
            get { return this.tags; }
            set
            {
                this.tags = value;
                this.NotifyOfPropertyChange(nameof(this.Tags));
                this.NotifyOfPropertyChange(nameof(this.CanGenerate));
            }
        }

        public bool CanGenerate
        {
            get
            {
                if (string.IsNullOrEmpty(this.languageType) == true)
                    return false;
                if (this.IsProgressing == true)
                    return false;
                if (File.Exists(generatorPath) == false)
                    return false;
                if (Directory.Exists(this.OutputPath) == false)
                    return false;
                if (this.tags == TagInfo.Unused)
                    return false;
                return string.IsNullOrEmpty(this.outputPath) == false;
            }
        }

        public bool IsCompilable
        {
            get { return this.isCompilable; }
            set
            {
                this.isCompilable = value;
                this.NotifyOfPropertyChange(nameof(this.IsCompilable));
            }
        }

        public string LanguageType
        {
            get { return this.languageType; }
            set
            {
                this.languageType = value;
                this.NotifyOfPropertyChange(nameof(this.LanguageType));
            }
        }

        public string[] LanguageTypes
        {
            get { return languageTypes; }
        }
    }
}
