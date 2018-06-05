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
using System.ComponentModel;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Javascript.Methods.DataBase
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(DataBase))]
    class GetTypeItemDataMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public GetTypeItemDataMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string, IDictionary<string, IDictionary<int, object>>>(GetTypeItemData);
        }

        private IDictionary<string, IDictionary<int, object>> GetTypeItemData(string dataBaseName, string typeItemPath, string revision)
        {
            var typeItem = this.GetTypeItem(dataBaseName, typeItemPath);
            var authentication = this.Context.GetAuthentication(this);

            return typeItem.Dispatcher.Invoke(() =>
            {
                var dataSet = typeItem.GetDataSet(authentication, revision);
                var types = new Dictionary<string, IDictionary<int, object>>(dataSet.Types.Count);
                foreach (var item in dataSet.Types)
                {
                    var members = this.GetTypeMembers(item);
                    types.Add(item.Name, members);
                }
                return types;
            });
        }

        private IDictionary<int, object> GetTypeMembers(CremaDataType dataType)
        {
            var props = new Dictionary<int, object>();
            for (var i = 0; i < dataType.Members.Count; i++)
            {
                var typeMember = dataType.Members[i];
                props.Add(i, this.GetTypeMember(typeMember));
            }
            return props;
        }

        private IDictionary<string, object> GetTypeMember(CremaDataTypeMember typeMember)
        {
            var props = new Dictionary<string, object>
            {
                { nameof(typeMember.Name), typeMember.Name },
                { nameof(typeMember.Value), typeMember.Value },
                { nameof(typeMember.Comment), typeMember.Comment }
            };
            return props;
        }
    }
}
