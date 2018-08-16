using Ntreev.Crema.Client.Base.Dialogs.ViewModels;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Base.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(LogInfoViewModel))]
    class RevertDataBaseMenuItem : MenuItemBase
    {
        [Import]
        private Authenticator authenticator = null;

        public RevertDataBaseMenuItem()
        {
            this.DisplayName = "Revert";
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (parameter is LogInfoViewModel viewModel)
            {
                return viewModel.Target is IDataBase;
            }
            return false;
        }

        protected override async void OnExecute(object parameter)
        {
            try
            {
                if (parameter is LogInfoViewModel viewModel && viewModel.Target is IDataBase dataBase)
                {
                    await dataBase.Dispatcher.InvokeAsync(() => dataBase.Revert(this.authenticator, viewModel.Revision));
                }
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }
    }
}
