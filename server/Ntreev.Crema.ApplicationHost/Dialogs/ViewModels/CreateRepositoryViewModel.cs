using Microsoft.WindowsAPICodePack.Dialogs;
using Ntreev.Library.IO;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ApplicationHost.Dialogs.ViewModels
{
    class CreateRepositoryViewModel : ModalDialogBase
    {
        private string basePath;

        public CreateRepositoryViewModel()
        {
            this.Title = "저장소 생성";
        }

        public void SelectBasePath()
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                InitialDirectory = this.BasePath,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.BasePath = dialog.FileName;
            }
        }

        public void Create()
        {
            this.TryClose(true);
        }

        public string BasePath
        {
            get { return this.basePath; }
            set
            {
                this.basePath = value;
                this.NotifyOfPropertyChange(() => this.BasePath);
                this.NotifyOfPropertyChange(() => this.CanCreate);
            }
        }
         
        public bool CanCreate
        {
            get
            {
                if (Directory.Exists(this.basePath) == false)
                    return false;
                return true;
            }
        }
    }
}
