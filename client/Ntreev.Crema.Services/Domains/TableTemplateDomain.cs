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
using Ntreev.Crema.Services.Data;
using Ntreev.Library;
using Ntreev.Crema.Services.Properties;

namespace Ntreev.Crema.Services.Domains
{
    class TableTemplateDomain : Domain
    {
        private CremaTemplate template;
        private DataView view;

        public TableTemplateDomain(DomainInfo domainInfo, CremaDispatcher dispatcher)
            : base(domainInfo, dispatcher)
        {
            if (domainInfo.ItemType == nameof(NewChildTableTemplate))
                this.IsNew = true;
            else if (domainInfo.ItemType == nameof(NewTableTemplate))
                this.IsNew = true;
        }

        public bool IsNew
        {
            get;
            set;
        }

        public override object Source
        {
            get { return this.template; }
        }

        public CremaTemplate TemplateSource
        {
            get { return this.template; }
        }

        protected override byte[] SerializeSource()
        {
            var xml = XmlSerializerUtility.GetString(this.template);
            return Encoding.UTF8.GetBytes(xml.Compress());
        }

        protected override void DerializeSource(byte[] data)
        {
            var xml = Encoding.UTF8.GetString(data).Decompress();
            this.template = XmlSerializerUtility.ReadString<CremaTemplate>(xml);
        }

        protected override void OnInitialize(DomainMetaData metaData)
        {
            base.OnInitialize(metaData);

            var xml = Encoding.UTF8.GetString(metaData.Data).Decompress();
            this.template = XmlSerializerUtility.ReadString<CremaTemplate>(xml);
            this.view = this.template.View;
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            this.template = null;
            this.view = null;
        }

        protected override void OnDeleted(EventArgs e)
        {
            base.OnDeleted(e);
        }

        protected override void OnNewRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {
            this.template.SignatureDateProvider = new InternalSignatureDateProvider(signatureDate);
            foreach (var item in rows)
            {
                CremaDomainUtility.AddNew(this.view, item.Fields);
            }
            this.template.AcceptChanges();
        }

        protected override void OnRemoveRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {
            this.template.SignatureDateProvider = new InternalSignatureDateProvider(signatureDate);
            foreach (var item in rows)
            {
                CremaDomainUtility.Delete(this.view, item.Keys);
            }
            this.template.AcceptChanges();
        }

        protected override void OnSetRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {
            this.template.SignatureDateProvider = new InternalSignatureDateProvider(signatureDate);
            foreach (var item in rows)
            {
                CremaDomainUtility.SetFieldsForce(this.view, item.Keys, item.Fields);
            }
            this.template.AcceptChanges();
        }

        protected override void OnSetProperty(DomainUser domainUser, string propertyName, object value, SignatureDate signatureDate)
        {
            this.template.SignatureDateProvider = new InternalSignatureDateProvider(signatureDate);
            if (propertyName == "TableName")
            {
                if (this.IsNew == false)
                    throw new InvalidOperationException(Resources.Exception_CannotRename);
                this.template.TableName = (string)value;
            }
            else if (propertyName == "Comment")
            {
                this.template.Comment = (string)value;
            }
            else if (propertyName == "Tags")
            {
                this.template.Tags = (TagInfo)((string)value);
            }
            else if (propertyName == "IgnoreCaseSensitive")
            {
                this.template.IgnoreCaseSensitive = (bool)value;
            }
            else
            {
                if (propertyName == null)
                    throw new ArgumentNullException(nameof(propertyName));
                throw new ArgumentException(string.Format(Resources.Exception_InvalidProperty_Format, propertyName), nameof(propertyName));
            }
        }
    }
}
