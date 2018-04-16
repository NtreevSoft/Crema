using System;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Windows;
using System.ComponentModel.Composition;
using Ntreev.ModernUI.Framework;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using Ntreev.Library.IO;
using Ntreev.Crema.ServiceHosts;
using Ntreev.Crema.Services;
using System.Collections.Generic;
using Ntreev.Crema.ApplicationHost.Dialogs.ViewModels;
using System.Configuration;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.ApplicationHost
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Caliburn.Micro.Screen, IShell
    {
        private readonly CremaService service;
        private readonly IRepositoryProvider[] repoProviders;
        private string basePath;
        private int port = AddressUtility.DefaultPort;
        private bool isOpened;
        private bool isProgressing;
        private string message;
        private string repositoryModule;

        [ImportingConstructor]
        public ShellViewModel(CremaService service, [ImportMany]IEnumerable<IRepositoryProvider> repoProviders)
        {
            this.service = service;
            this.repoProviders = repoProviders.ToArray();
            this.repositoryModule = CremaBootstrapper.DefaultRepositoryModule;
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

        public async void CreateRepository()
        {
            var dialog = new CreateRepositoryViewModel()
            {
                BasePath = this.BasePath,
            };

            if (dialog.ShowDialog() != true)
                return;

            if (DirectoryUtility.IsEmpty(dialog.BasePath) == false)
            {
                if (AppMessageBox.ShowQuestion("대상 폴더는 비어있지 않습니다. 비우고 저장소를 생성하시겠습니까?") == false)
                    return;
            }

            this.IsProgressing = true;
            
            try
            {
                var basePath = dialog.BasePath;
                this.service.RepositoryModule = this.RepositoryModule;
                await Task.Run(() =>
                {
                    this.Message = "대상 폴더를 비우는 중입니다.";
                    DirectoryUtility.Empty(basePath);
                    this.Message = "저장소를 생성중입니다.";
                    this.service.CreateRepository(basePath, false);
                });
                AppMessageBox.Show("새로운 저장소를 생성했습니다.");
                this.BasePath = basePath;
            }
            catch(Exception e)
            {
                AppMessageBox.ShowError(e);
            }
            finally
            {
                this.Message = string.Empty;
                this.IsProgressing = false;
            }
        }

        public async void Start()
        {
            this.IsProgressing = true;
            this.service.RepositoryModule = this.RepositoryModule;
            this.service.BasePath = this.BasePath;
            this.service.Port = this.Port;
            try
            {
                this.Message = "서버를 구동중입니다.";
                await Task.Run(() => this.service.Open());
                this.IsOpened = true;
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                this.IsOpened = false;
            }
            finally
            {
                this.IsProgressing = false;
            }

            this.Message = "서버를 시작하였습니다.";
            this.NotifyOfPropertyChange(() => this.CanStart);
            this.NotifyOfPropertyChange(() => this.CanStop);
            this.NotifyOfPropertyChange(() => this.Title);
            this.OnOpened(EventArgs.Empty);
        }

        public async void Stop()
        {
            this.IsProgressing = true;
            this.Message = "서버를 닫는중입니다.";
            try
            {
                await Task.Run(() => this.service.Close());
            }
            catch
            {
            }
            finally
            {
                this.IsProgressing = false;
                this.Message = string.Empty;
                this.IsOpened = false;
            }
            this.NotifyOfPropertyChange(() => this.CanStart);
            this.NotifyOfPropertyChange(() => this.CanStop);
            this.NotifyOfPropertyChange(() => this.Title);
            this.OnClosed(EventArgs.Empty);
        }

        public override void CanClose(Action<bool> callback)
        {
            if (this.IsOpened == true)
            {
                if (AppMessageBox.ShowProceed("서버가 실행중입니다. 종료하시겠습니까?") == false)
                    return;

                this.Closed += (s, e) => callback(true);
                this.Stop();
                return;
            }
            base.CanClose(callback);
        }

        [ConfigurationProperty("port", DefaultValue = AddressUtility.DefaultPort)]
        public int Port
        {
            get { return this.port; }
            set
            {
                this.port = value;
                this.NotifyOfPropertyChange(() => this.Port);
            }
        }

        [ConfigurationProperty("basePath")]
        public string BasePath
        {
            get { return this.basePath; }
            set
            {
                this.basePath = value;
                this.NotifyOfPropertyChange(() => this.BasePath);
                this.NotifyOfPropertyChange(() => this.CanStart);
            }
        }

        public string Message
        {
            get { return this.message; }
            set
            {
                if (this.message == value)
                    return;

                this.message = value;
                this.NotifyOfPropertyChange(() => this.Message);
            }
        }

        public string Title
        {
            get
            {
                if (this.IsOpened == false)
                    return "Crema Server";
                return string.Format("Crema Server({0})", this.Port);
            }
        }

        public bool CanStart
        {
            get
            {
                if (this.isOpened == true)
                    return false;
                if (DirectoryUtility.Exists(this.basePath) == false)
                    return false;
                return true;
            }
        }

        public bool CanStop
        {
            get { return this.isOpened == true; }
        }

        public bool IsOpened
        {
            get { return this.isOpened; }
            set
            {
                this.isOpened = value;
                this.NotifyOfPropertyChange(() => this.IsOpened);
            }
        }

        public bool IsProgressing
        {
            get { return this.isProgressing; }
            set
            {
                this.isProgressing = value;
                this.NotifyOfPropertyChange(() => this.IsProgressing);
            }
        }

        public string[] RepositoryModules
        {
            get
            {
                return this.repoProviders.Select(item => item.Name).ToArray();
            }
        }

        public string RepositoryModule
        {
            get { return this.repositoryModule; }
            set
            {
                this.repositoryModule = value;
                this.NotifyOfPropertyChange(() => this.RepositoryModule);
            }
        }

        public event EventHandler Opened;

        public event EventHandler Closed;

        protected virtual void OnOpened(EventArgs e)
        {
            if (this.Opened != null)
            {
                this.Opened(this, e);
            }
        }

        protected virtual void OnClosed(EventArgs e)
        {
            if (this.Closed != null)
            {
                this.Closed(this, e);
            }
        }
    }
}