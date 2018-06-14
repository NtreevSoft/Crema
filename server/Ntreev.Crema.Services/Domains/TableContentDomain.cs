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

using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Data;
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Ntreev.Crema.Services.Domains
{
    [Serializable]
    class TableContentDomain : Domain
    {
        public const string TypeName = "Table";

        private CremaDataSet dataSet;
        private List<FindResultInfo> findResults = new List<FindResultInfo>(100);
        private Dictionary<string, DataView> views = new Dictionary<string, DataView>();

        private TableContentDomain(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public TableContentDomain(Authentication authentication, CremaDataSet dataSet, DataBase dataBase, string itemPath, string itemType)
            : base(authentication.ID, dataBase.ID, itemPath, itemType)
        {
            if (dataSet.HasChanges() == true)
                throw new ArgumentException(Resources.Exception_UnsavedDataCannotEdit, nameof(dataSet));
            this.dataSet = dataSet;

            foreach (var item in this.dataSet.Tables)
            {
                var view = item.AsDataView();
                this.views.Add(item.TableName, view);
            }
        }

        public override object Source => this.dataSet;

        protected override byte[] SerializeSource()
        {
            var xml = XmlSerializerUtility.GetString(this.dataSet);
            return Encoding.UTF8.GetBytes(xml.Compress());
        }

        protected override void DerializeSource(byte[] data)
        {
            var xml = Encoding.UTF8.GetString(data).Decompress();
            this.dataSet = XmlSerializerUtility.ReadString<CremaDataSet>(xml);

            foreach (var item in this.dataSet.Tables)
            {
                var view = item.AsDataView();
                this.views.Add(item.TableName, view);
            }
        }

        protected override DomainRowInfo[] OnNewRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDateProvider signatureProvider)
        {
            this.dataSet.SignatureDateProvider = signatureProvider;

            try
            {
                for (var i = 0; i < rows.Length; i++)
                {
                    var view = this.views[rows[i].TableName];
                    var rowView = CremaDomainUtility.AddNew(view, rows[i].Fields);
                    rows[i].Keys = CremaDomainUtility.GetKeys(rowView);
                    rows[i].Fields = CremaDomainUtility.GetFields(rowView);
                }

                this.dataSet.AcceptChanges();

                return rows;
            }
            catch
            {
                this.dataSet.RejectChanges();
                throw;
            }
        }

        protected override DomainRowInfo[] OnSetRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDateProvider signatureProvider)
        {
            this.dataSet.SignatureDateProvider = signatureProvider;

            try
            {
                for (var i = 0; i < rows.Length; i++)
                {
                    var view = this.views[rows[i].TableName];
                    rows[i].Fields = CremaDomainUtility.SetFields(view, rows[i].Keys, rows[i].Fields);
                }

                this.dataSet.AcceptChanges();
                return rows;
            }
            catch
            {
                this.dataSet.RejectChanges();
                throw;
            }
        }

        protected override void OnRemoveRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDateProvider signatureProvider)
        {
            this.dataSet.SignatureDateProvider = signatureProvider;

            try
            {
                foreach (var item in rows)
                {
                    var view = this.views[item.TableName];
                    if (DomainRowInfo.ClearKey.SequenceEqual(item.Keys) == true)
                    {
                        view.Table.Clear();
                    }
                    else
                    {
                        CremaDomainUtility.Delete(view, item.Keys);
                    }
                }
                this.dataSet.AcceptChanges();
            }
            catch
            {
                this.dataSet.RejectChanges();
                throw;
            }
        }
    }
}
