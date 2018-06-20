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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Tables.Dialogs.ViewModels
{
    public class LogInfoViewModel : ListBoxItemViewModel, IInfoProvider
    {
        private readonly Authentication authentication;
        private readonly ITableItem tableItem;
        private readonly LogInfo logInfo;

        public LogInfoViewModel(Authentication authentication, ITableItem tableItem, LogInfo logInfo)
        {
            this.authentication = authentication;
            this.tableItem = tableItem;
            this.logInfo = logInfo;
            this.Target = tableItem;
        }

        public void Preview()
        {
            if (this.tableItem is ITable table)
            {
                var dialog = new PreviewTableViewModel(this.authentication, table, this.logInfo.Revision);
                dialog.ShowDialog();
            }
            else if (this.tableItem is ITableCategory category)
            {
                var dialog = new PreviewTableCategoryViewModel(this.authentication, category, this.logInfo.Revision);
                dialog.ShowDialog();
            }
        }

        public LogInfo LogInfo
        {
            get { return this.logInfo; }
        }

        public string UserID
        {
            get { return this.logInfo.UserID; }
        }

        public string Revision
        {
            get { return this.logInfo.Revision; }
        }

        public string Message
        {
            get { return this.logInfo.Comment; }
        }

        public DateTime DateTime
        {
            get { return this.logInfo.DateTime; }
        }

        #region IInfoProvider

        IDictionary<string, object> IInfoProvider.Info
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { nameof(UserID), this.UserID },
                    { nameof(Revision), this.Revision },
                    { nameof(Message), this.Message },
                    { nameof(DateTime), this.DateTime }
                };
            }
        }

        #endregion
    }
}
