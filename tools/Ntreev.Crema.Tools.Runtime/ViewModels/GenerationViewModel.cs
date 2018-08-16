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
using Ntreev.Crema.Runtime.Generation;
using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Crema.RuntimeService;
using Ntreev.Crema.Tools.Framework;
using Ntreev.Crema.Tools.Framework.Dialogs.ViewModels;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Tools.Runtime.ViewModels
{
    [Export(typeof(IContent))]
    class GenerationViewModel : ContentBase
    {
        private readonly IRuntimeService service;
        private readonly IEnumerable<ICodeGenerator> generators;
        private readonly IEnumerable<IDataSerializer> serializers;
        private readonly IAppConfiguration configs;
        private readonly ObservableCollection<string> dataBases = new ObservableCollection<string>();
        private readonly string[] languageTypes;
        private readonly GenerationItemCollection settingsList;
        private GenerationItemViewModel selectedItem;
        private GenerationItemViewModel settings = new GenerationItemViewModel();
        
        private bool openAfterGenerate;

        [ImportingConstructor]
        public GenerationViewModel(IRuntimeService service, [ImportMany]IEnumerable<ICodeGenerator> generators, [ImportMany]IEnumerable<IDataSerializer> serializers, IAppConfiguration configs)
        {
            this.GroupName = "Runtime";
            this.DisplayName = string.Empty;
            this.service = service;
            this.configs = configs;
            this.generators = generators;
            this.serializers = serializers;
            this.languageTypes = generators.Select(item => item.Name).ToArray();
            this.settingsList = new GenerationItemCollection(configs);
            this.SelectedItem = this.settingsList.FirstOrDefault();
            this.configs.Update(this);
        }

        private void OnAddSettings()
        {
            this.settingsList.Insert(0, this.settings);
        }

        public void SelectPath()
        {
            var dialog = new CommonOpenFileDialog() { IsFolderPicker = true, };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.OutputPath = dialog.FileName;
            }
        }

        public void SelectDataBase()
        {
            var dialog = new DataBaseListViewModel(this.Address);

            if (dialog.ShowDialog() == true)
            {
                this.DataBase = dialog.SelectedItem.Value.Name;
            }
        }

        public async void Generate()
        {
            try
            {
                this.BeginProgress();
                var metaData = await Task.Run(() => this.service.GetMetaData(this.Address, this.DataBase, this.Tags, this.FilterExpression, this.IsDevmode, null));
                var generator = this.generators.FirstOrDefault(item => item.Name == this.LanguageType);
                generator.Generate(this.OutputPath, metaData.Item1, CodeGenerationSettings.Default);
                var serializer = this.serializers.First();
                serializer.Serialize(System.IO.Path.Combine(this.OutputPath, "crema.dat"), metaData.Item2);

                this.settingsList.Add(this.settings);
                this.configs.Commit(this);
                if (this.OpenAfterGenerate == true)
                {
                    Process.Start("explorer", this.OutputPath);
                }
                this.EndProgress();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                this.EndProgress();
            }
        }

        public void EditFilterExpression()
        {
            var dialog = new EditFilterExpressionViewModel()
            {
                FilterExpression = this.FilterExpression,
            };
            if (dialog.ShowDialog() == true)
            {
                this.FilterExpression = dialog.FilterExpression;
            }
        }

        public string SettingsName
        {
            get { return this.settings.Name; }
            set
            {
                this.settings.Name = value;
                this.NotifyOfPropertyChange(() => this.SettingsName);
                this.NotifyOfPropertyChange(() => this.CanGenerate);
            }
        }

        public string Address
        {
            get { return this.settings.Address; }
            set
            {
                this.settings.Address = value;
                this.NotifyOfPropertyChange(() => this.Address);
                this.NotifyOfPropertyChange(() => this.CanGenerate);
            }
        }

        public string DataBase
        {
            get { return this.settings.DataBase; }
            set
            {
                this.settings.DataBase = value;
                this.NotifyOfPropertyChange(() => this.DataBase);
                this.NotifyOfPropertyChange(() => this.CanGenerate);
            }
        }

        public string Tags
        {
            get { return this.settings.Tags; }
            set
            {
                this.settings.Tags = value;
                this.NotifyOfPropertyChange(() => this.Tags);
                this.NotifyOfPropertyChange(() => this.CanGenerate);
            }
        }

        public string OutputPath
        {
            get { return this.settings.OutputPath; }
            set
            {
                this.settings.OutputPath = value;
                this.NotifyOfPropertyChange(() => this.OutputPath);
                this.NotifyOfPropertyChange(() => this.CanGenerate);
            }
        }

        public IEnumerable<string> LanguageTypes
        {
            get { return this.languageTypes; }
        }

        public string LanguageType
        {
            get { return this.settings.LanguageType; }
            set
            {
                this.settings.LanguageType = value;
                this.NotifyOfPropertyChange(() => this.LanguageType);
                this.NotifyOfPropertyChange(() => this.CanGenerate);
            }
        }

        public bool IsDevmode
        {
            get { return this.settings.IsDevmode; }
            set
            {
                this.settings.IsDevmode = value;
                this.NotifyOfPropertyChange(nameof(this.IsDevmode));
                this.NotifyOfPropertyChange(() => this.CanGenerate);
            }
        }

        [ConfigurationProperty("openAfterGenerate")]
        public bool OpenAfterGenerate
        {
            get { return this.openAfterGenerate; }
            set
            {
                this.openAfterGenerate = value;
                this.NotifyOfPropertyChange(() => this.OpenAfterGenerate);
            }
        }

        public string FilterExpression
        {
            get { return this.settings.FilterExpression ?? string.Empty; }
            set
            {
                this.settings.FilterExpression = value;
                this.NotifyOfPropertyChange(() => this.FilterExpression);
            }
        }

        public bool CanGenerate
        {
            get
            {
                if (this.IsProgressing == true)
                    return false;
                if (this.languageTypes.Any(item => item == this.LanguageType) == false)
                    return false;
                if (this.Tags == string.Empty)
                    return false;
                if (this.Tags == TagInfo.Unused.ToString())
                    return false;
                if (this.DataBase == string.Empty)
                    return false;
                if (this.Address == string.Empty)
                    return false;
                if (DirectoryUtility.Exists(this.OutputPath) == false)
                    return false;
                return string.IsNullOrEmpty(this.OutputPath) == false;
            }
        }

        public IEnumerable<GenerationItemViewModel> ItemsSource
        {
            get { return this.settingsList; }
        }

        public GenerationItemViewModel SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                this.selectedItem = value;
                if (this.selectedItem != null)
                    this.settings = value.Clone();
                else
                    this.settings = GenerationItemViewModel.Empty;
                this.Refresh();
            }
        }

        public event EventHandler Generated;

        protected virtual void OnGenerated(EventArgs e)
        {
            this.Generated?.Invoke(this, e);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.configs.Update(this);
        }
    }
}
