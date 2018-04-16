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
using System.IO;
using System.Data;
using Ntreev.Library.Serialization;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Services.Data;
using Ntreev.Library;
using Ntreev.Crema.Services.Properties;

namespace Ntreev.Crema.Services.Domains
{
    class TypeDomain : Domain
    {
        private CremaDataType dataType;
        private DataView view;

        public TypeDomain(DomainInfo domainInfo, CremaDispatcher dispatcher)
            : base(domainInfo, dispatcher)
        {
            if (domainInfo.ItemType == nameof(NewTypeTemplate))
                this.IsNew = true;
        }

        public override object Source
        {
            get { return this.dataType; }
        }

        public bool IsNew { get; set; }

        protected override byte[] SerializeSource()
        {
            var text = this.dataType.Path + ";" + XmlSerializerUtility.GetString(this.dataType.DataSet);
            return Encoding.UTF8.GetBytes(text.Compress());
        }

        protected override void DerializeSource(byte[] data)
        {
            var text = Encoding.UTF8.GetString(data).Decompress();
            var index = text.IndexOf(";");
            var path = text.Remove(index);
            var itemName = new ItemName(path);
            var xml = text.Substring(index + 1);
            var dataSet = XmlSerializerUtility.ReadString<CremaDataSet>(xml);
            this.dataType = dataSet.Types[itemName.Name];
        }

        protected override void OnInitialize(DomainMetaData metaData)
        {
            base.OnInitialize(metaData);

            var text = Encoding.UTF8.GetString(metaData.Data).Decompress();
            var index = text.IndexOf(";");
            var path = text.Remove(index);
            var itemName = new ItemName(path);
            var xml = text.Substring(index + 1);
            var dataSet = XmlSerializerUtility.ReadString<CremaDataSet>(xml);
            this.dataType = dataSet.Types[itemName.Name];
            this.view = this.dataType.AsDataView();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            this.dataType = null;
            this.view = null;
        }

        protected override void OnDeleted(EventArgs e)
        {
            base.OnDeleted(e);
        }

        protected override void OnNewRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {
            this.dataType.BeginLoadData();
            try
            {
                foreach (var item in rows)
                {
                    CremaDomainUtility.AddNew(this.view, item.Fields);
                }
            }
            finally
            {
                this.dataType.EndLoadData();
            }
            this.dataType.ModificationInfo = signatureDate;
            this.dataType.AcceptChanges();
        }

        protected override void OnRemoveRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {
            this.dataType.BeginLoadData();
            try
            {
                foreach (var item in rows)
                {
                    CremaDomainUtility.Delete(this.view, item.Keys);
                }
            }
            finally
            {
                this.dataType.EndLoadData();
            }
            this.dataType.ModificationInfo = signatureDate;
            this.dataType.AcceptChanges();
        }

        protected override void OnSetRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {
            this.dataType.BeginLoadData();
            try
            {
                foreach (var item in rows)
                {
                    CremaDomainUtility.SetFieldsForce(this.view, item.Keys, item.Fields);
                }
            }
            finally
            {
                this.dataType.EndLoadData();
            }
            this.dataType.ModificationInfo = signatureDate;
            this.dataType.AcceptChanges();
        }

        protected override void OnSetProperty(DomainUser domainUser, string propertyName, object value, SignatureDate signatureDate)
        {
            if (propertyName == "TypeName")
            {
                if (this.IsNew == false)
                    throw new InvalidOperationException(Resources.Exception_CannotRename);
                this.dataType.TypeName = (string)value;
            }
            else if (propertyName == "IsFlag")
            {
                this.dataType.IsFlag = (bool)value;
            }
            else if (propertyName == "Comment")
            {
                this.dataType.Comment = (string)value;
            }
            else
            {
                if (propertyName == null)
                    throw new ArgumentNullException(nameof(propertyName));
                throw new ArgumentException(string.Format(Resources.Exception_InvalidProperty_Format, propertyName), nameof(propertyName));
            }
            this.dataType.ModificationInfo = signatureDate;
        }
    }
}
