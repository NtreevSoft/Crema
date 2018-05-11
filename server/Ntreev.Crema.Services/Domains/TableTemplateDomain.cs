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

using Ntreev.Crema.Services.Data;
using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library;
using Ntreev.Crema.Services.Properties;

namespace Ntreev.Crema.Services.Domains
{
    [Serializable]
    class TableTemplateDomain : Domain
    {
        public const string TypeName = "TableTemplate";

        private CremaTemplate template;
        private DataView view;

        public TableTemplateDomain(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsNew = info.GetBoolean("IsNew");
            this.view = this.template.View;
        }

        public TableTemplateDomain(Authentication authentication, CremaTemplate templateSource, DataBase dataBase, string itemPath, string itemType)
            : base(authentication.ID, dataBase.ID, itemPath, itemType)
        {
            this.template = templateSource;
            this.view = this.template.View;
        }

        public override object Source
        {
            get { return this.template; }
        }

        public bool IsNew { get; set; }

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

        protected override void OnSerializaing(SerializationInfo info, StreamingContext context)
        {
            base.OnSerializaing(info, context);
            info.AddValue("IsNew", this.IsNew);
        }

        protected override DomainRowInfo[] OnNewRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDateProvider signatureProvider)
        {
            this.template.SignatureDateProvider = signatureProvider;

            for (int i = 0; i < rows.Length; i++)
            {
                var rowView = CremaDomainUtility.AddNew(this.view, rows[i].Fields);
                rows[i].Keys = CremaDomainUtility.GetKeys(rowView);
                rows[i].Fields = CremaDomainUtility.GetFields(rowView);
            }

            return rows;
        }

        protected override DomainRowInfo[] OnSetRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDateProvider signatureProvider)
        {
            this.template.SignatureDateProvider = signatureProvider;

            for (int i = 0; i < rows.Length; i++)
            {
                rows[i].Fields = CremaDomainUtility.SetFields(this.view, rows[i].Keys, rows[i].Fields);
            }

            return rows;
        }

        protected override void OnRemoveRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDateProvider signatureProvider)
        {
            this.template.SignatureDateProvider = signatureProvider;

            foreach (var item in rows)
            {
                CremaDomainUtility.Delete(this.view, item.Keys);
            }
        }

        protected override void OnSetProperty(DomainUser domainUser, string propertyName, object value, SignatureDateProvider signatureProvider)
        {
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
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
