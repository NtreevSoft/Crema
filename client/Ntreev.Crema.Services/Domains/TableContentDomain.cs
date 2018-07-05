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
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using Ntreev.Library.Serialization;
using Ntreev.Crema.Services.Data;
using Ntreev.Library;

namespace Ntreev.Crema.Services.Domains
{
    class TableContentDomain : Domain
    {
        private readonly Dictionary<string, DataView> views = new Dictionary<string, DataView>();
        private readonly Dictionary<DataView, CremaDataTable> tables = new Dictionary<DataView, CremaDataTable>();

        public TableContentDomain(DomainInfo domainInfo, CremaDispatcher dispatcher)
            : base(domainInfo, dispatcher)
        {
            
        }

        public override object Source => this.DataSet;

        public CremaDataSet DataSet { get; private set; }

        protected override byte[] SerializeSource()
        {
            var xml = XmlSerializerUtility.GetString(this.DataSet);
            return Encoding.UTF8.GetBytes(xml.Compress());
        }

        protected override void DerializeSource(byte[] data)
        {
            var xml = Encoding.UTF8.GetString(data).Decompress();
            this.DataSet = XmlSerializerUtility.ReadString<CremaDataSet>(xml);

            foreach (var item in this.DataSet.Tables)
            {
                var view = item.AsDataView();
                this.views.Add(item.Name, view);
                this.tables.Add(view, item);
            }
        }

        protected override void OnInitialize(DomainMetaData metaData)
        {
            base.OnInitialize(metaData);

            var xml = Encoding.UTF8.GetString(metaData.Data).Decompress();
            this.DataSet = XmlSerializerUtility.ReadString<CremaDataSet>(xml);
            this.DataSet.AcceptChanges();
            this.views.Clear();
            foreach (var item in this.DataSet.Tables)
            {
                var view = item.AsDataView();
                //view.AllowDelete = false;
                //view.AllowEdit = false;
                //view.AllowNew = false;
                this.views.Add(item.Name, view);
                this.tables.Add(view, item);
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            this.DataSet = null;
            this.views.Clear();
        }

        protected override void OnDeleted(EventArgs e)
        {
            base.OnDeleted(e);
            if (this.DataSet != null)
            {
                this.DataSet.Dispose();
                this.DataSet = null;
            }
        }

        protected override void OnNewRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {
            this.DataSet.BeginLoad();
            try
            {
                foreach (var item in rows)
                {
                    var view = this.views[item.TableName];
                    CremaDomainUtility.AddNew(view, item.Fields);
                    this.tables[view].ContentsInfo = signatureDate;
                }
            }
            finally
            {
                this.DataSet.EndLoad();
            }
            this.DataSet.AcceptChanges();
        }

        protected override void OnRemoveRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {
            foreach (var item in rows.Reverse())
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
                this.tables[view].ContentsInfo = signatureDate;
            }
            this.DataSet.AcceptChanges();
        }

        protected override void OnSetRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {
            this.DataSet.BeginLoad();
            try
            {
                foreach (var item in rows)
                {
                    var view = this.views[item.TableName];
                    CremaDomainUtility.SetFieldsForce(view, item.Keys, item.Fields);
                    this.tables[view].ContentsInfo = signatureDate;
                }
            }
            finally
            {
                this.DataSet.EndLoad();
            }
            this.DataSet.AcceptChanges();
        }
    }
}
