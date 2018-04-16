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

using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Javascript.Methods
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class GetDataBaseLogInfoMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public GetDataBaseLogInfoMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, IDictionary<string, object>[]>(GetDataBaseLogInfo);
        }

        private IDictionary<string, object>[] GetDataBaseLogInfo(string dataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                var authentication = this.Context.GetAuthentication(this);
                var logInfos = dataBase.GetLog(authentication);

                return this.GetLogInfo(logInfos);
            });
        }

        private IDictionary<string, object>[] GetLogInfo(LogInfo[] logInfos)
        {
            var props = new IDictionary<string, object>[logInfos.Length];
            for (var i = 0; i < logInfos.Length; i++)
            {
                props[i] = this.GetLogInfo(logInfos[i]);
            }
            return props;
        }

        private IDictionary<string, object> GetLogInfo(LogInfo columnInfo)
        {
            var props = new Dictionary<string, object>
            {
                { nameof(columnInfo.UserID), columnInfo.UserID },
                { nameof(columnInfo.Revision), columnInfo.Revision },
                { nameof(columnInfo.Comment), columnInfo.Comment },
                { nameof(columnInfo.DateTime), columnInfo.DateTime }
            };
            return props;
        }
    }
}
