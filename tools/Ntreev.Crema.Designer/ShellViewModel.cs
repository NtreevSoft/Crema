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

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ntreev.Crema.Presentation;
using Ntreev.Crema.Data;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Designer
{
    [Export(typeof(IShell))]
    [Export]
    public class ShellViewModel : ScreenBase, IShell
    {
        private CremaDataSet dataSet = new CremaDataSet();
        private bool canRead = true;
        private bool isLoaded = true;
        //private ObservableCollection<object> tables;
        //private ObservableCollection<object> types;
        private string filename;

        private readonly ObservableCollection<IContent> contents;
        private IContent selectedContent;

        [ImportingConstructor]
        public ShellViewModel([ImportMany]IEnumerable<IContent> contents)
        {
            this.contents = new ObservableCollection<IContent>();
            foreach (var item in contents)
            {
                this.contents.Add(item);
            }

            this.selectedContent = this.contents.First();
            this.DisplayName = "Crema Designer";
            this.dataSet.ExtendedProperties[CremaSchema.TableDirectory] = new string[] { PathUtility.Separator };
            this.dataSet.ExtendedProperties[CremaSchema.TypeDirectory] = new string[] { PathUtility.Separator };
        }

        public void Close()
        {
            this.OnUnloaded(EventArgs.Empty);
        }

        public void Open()
        {
            this.CanOpen = false;
            this.IsLoaded = false;

            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.Open(dialog.FileName);
            }

            this.IsLoaded = true;
            this.CanOpen = true;
        }

        public void Save()
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.Save(dialog.FileName);
            }
        }

        public async void Open(string path)
        {
            try
            {
                this.BeginProgress();
                this.dataSet = await Task.Run(() => this.ReadDataSet(path));
                this.dataSet.SignatureDateProvider = new SignatureDateProvider("CremaInternal");
                this.EndProgress();
                this.OnLoaded(EventArgs.Empty);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e.Message);
                this.EndProgress();
                return;
            }
            this.Initialize();

            this.filename = path;
            this.NotifyOfPropertyChange(() => this.Filename);
        }

        public void Save(string path)
        {
            DirectoryUtility.Backup(path);
            try
            {
                this.dataSet.WriteToDirectory(path);
                DirectoryUtility.Clean(path);
            }
            catch (Exception e)
            {
                DirectoryUtility.Restore(path);
                AppMessageBox.ShowError(e);
            }
        }

        private void Initialize()
        {
            //this.IsProgressing = true;
            //this.NotifyOfPropertyChange(() => this.IsProgressing);

            //this.tables = new ObservableCollection<object>();
            //this.types = new ObservableCollection<object>();

            ////foreach (var item in this.dataSet.Tables.OrderBy(i => i.TableName).Where(i => i.Parent == null))
            ////{
            ////    this.tables.Add(new TableItemViewModel(item));
            ////}

            ////foreach (var item in this.dataSet.Types.OrderBy(i => i.Name))
            ////{
            ////    this.types.Add(new TypeItemViewModel(item));
            ////}
            //this.IsProgressing = false;
            //this.NotifyOfPropertyChange(() => this.IsProgressing);
        }

        public bool CanOpen
        {
            get { return this.canRead; }
            private set
            {
                this.canRead = value;
            }
        }

        public bool IsLoaded
        {
            get { return this.isLoaded; }
            private set
            {
                this.isLoaded = value;
                this.NotifyOfPropertyChange(() => this.IsLoaded);
            }
        }

        public string DataSetPath
        {
            get { return this.filename; }
        }

        public string Filename
        {
            get { return this.filename; }
            set
            {
                if (this.filename == value)
                    return;

                this.Open(value);
            }
        }

        public string Title
        {
            get { return this.ToString(); }
        }

        public IContent SelectedContent
        {
            get { return this.selectedContent; }
            set
            {
                if (this.selectedContent == value)
                    return;
                this.selectedContent = this.contents.SingleOrDefault(item => item == value);
                this.NotifyOfPropertyChange(() => this.SelectedContent);
            }
        }

        public ObservableCollection<IContent> Contents
        {
            get { return this.contents; }
        }

        public CremaDataSet DataSet
        {
            get { return this.dataSet; }
        }

        public event EventHandler Loaded;

        public event EventHandler Unloaded;

        public event EventHandler Closing;

        public event EventHandler Closed;

        protected virtual void OnLoaded(EventArgs e)
        {
            this.Loaded?.Invoke(this, e);
        }

        protected virtual void OnUnloaded(EventArgs e)
        {
            this.Unloaded?.Invoke(this, e);
        }

        protected virtual void OnClosing(EventArgs e)
        {
            this.Closing?.Invoke(this, e);
        }

        protected virtual void OnClosed(EventArgs e)
        {
            this.Closed?.Invoke(this, e);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }

        protected override void OnClose()
        {
            base.OnClose();
            this.OnClosing(EventArgs.Empty);
        }

        private CremaDataSet ReadDataSet(string path)
        {
            var dataSet = CremaDataSet.ReadFromDirectory(path);

            var dirs = DirectoryUtility.GetAllDirectories(path);

            {
                var typesPath = System.IO.Path.Combine(path, CremaSchema.TypeDirectory);
                var query = from item in dirs
                            where item.StartsWith(typesPath)
                            let localPath = item.Substring(typesPath.Length)
                            let segments = localPath.Split(new char[] { System.IO.Path.DirectorySeparatorChar, }, StringSplitOptions.RemoveEmptyEntries)
                            select CategoryName.Create(segments).Path;
                dataSet.ExtendedProperties[CremaSchema.TypeDirectory] = query.ToArray();
            }
            {
                var tablesPath = System.IO.Path.Combine(path, CremaSchema.TableDirectory);
                var query = from item in dirs
                            where item.StartsWith(tablesPath)
                            let localPath = item.Substring(tablesPath.Length)
                            let segments = localPath.Split(new char[] { System.IO.Path.DirectorySeparatorChar, }, StringSplitOptions.RemoveEmptyEntries)
                            select CategoryName.Create(segments).Path;
                dataSet.ExtendedProperties[CremaSchema.TableDirectory] = query.ToArray();
            }

            return dataSet;
        }
    }
}
