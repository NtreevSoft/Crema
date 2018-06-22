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
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Data;
using Ntreev.Crema.Services.Domains.Serializations;
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace Ntreev.Crema.Services.Domains
{
    [Serializable]
    class TypeDomain : Domain
    {
        private CremaDataType dataType;
        private DataView view;

        public TypeDomain(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsNew = info.GetBoolean(nameof(this.IsNew));
            this.dataType = this.Source as CremaDataType;
            this.view = this.dataType.View;
        }

        public TypeDomain(DomainSerializationInfo serializationInfo, object source)
            : base(serializationInfo, source)
        {
            this.IsNew = (bool)serializationInfo.GetProperty(nameof(IsNew));
            this.dataType = source as CremaDataType;
            this.view = this.dataType.View;
        }

        public TypeDomain(Authentication authentication, CremaDataType dataType, DataBase dataBase, string itemPath, string itemType)
            : base(authentication.ID, dataType, dataBase.ID, itemPath, itemType)
        {
            this.dataType = dataType;
            this.view = this.dataType.View;
        }

        public bool IsNew { get; set; }

        protected override byte[] SerializeSource(object source)
        {
            if (source is CremaDataType dataType)
            {
                var text = dataType.Path + ";" + XmlSerializerUtility.GetString(dataType.DataSet);
                return Encoding.UTF8.GetBytes(text.Compress());
            }
            throw new NotImplementedException();
        }

        protected override object DerializeSource(byte[] data)
        {
            var text = Encoding.UTF8.GetString(data).Decompress();
            var index = text.IndexOf(";");
            var path = text.Remove(index);
            var itemName = new ItemName(path);
            var xml = text.Substring(index + 1);
            var dataSet = XmlSerializerUtility.ReadString<CremaDataSet>(xml);
            return dataSet.Types[itemName.Name];
        }

        protected override void OnSerializaing(IDictionary<string, object> properties)
        {
            base.OnSerializaing(properties);
            properties.Add(nameof(this.IsNew), this.IsNew);
        }

        protected override DomainRowInfo[] OnNewRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDateProvider signatureProvider)
        {
            this.dataType.SignatureDateProvider = signatureProvider;

            try
            {
                for (var i = 0; i < rows.Length; i++)
                {
                    var rowView = CremaDomainUtility.AddNew(this.view, rows[i].Fields);
                    rows[i].Keys = CremaDomainUtility.GetKeys(rowView);
                    rows[i].Fields = CremaDomainUtility.GetFields(rowView);
                }
                this.dataType.AcceptChanges();

                return rows;
            }
            catch
            {
                this.dataType.RejectChanges();
                throw;
            }
        }

        protected override DomainRowInfo[] OnSetRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDateProvider signatureProvider)
        {
            this.dataType.SignatureDateProvider = signatureProvider;

            try
            {
                for (var i = 0; i < rows.Length; i++)
                {
                    rows[i].Fields = CremaDomainUtility.SetFields(this.view, rows[i].Keys, rows[i].Fields);
                }

                this.dataType.AcceptChanges();

                return rows;
            }
            catch
            {
                this.dataType.RejectChanges();
                throw;
            }
        }

        protected override void OnRemoveRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDateProvider signatureProvider)
        {
            this.dataType.SignatureDateProvider = signatureProvider;

            try
            {
                foreach (var item in rows)
                {
                    CremaDomainUtility.Delete(this.view, item.Keys);
                }

                this.dataType.AcceptChanges();
            }
            catch
            {
                this.dataType.RejectChanges();
                throw;
            }
        }

        protected override void OnSetProperty(DomainUser domainUser, string propertyName, object value, SignatureDateProvider signatureProvider)
        {
            base.OnSetProperty(domainUser, propertyName, value, signatureProvider);

            if (propertyName == CremaSchema.TypeName)
            {
                if (this.IsNew == false)
                    throw new InvalidOperationException(Resources.Exception_CannotRename);
                this.dataType.TypeName = (string)value;
            }
            else if (propertyName == CremaSchema.IsFlag)
            {
                this.dataType.IsFlag = (bool)value;
            }
            else if (propertyName == CremaSchema.Comment)
            {
                this.dataType.Comment = (string)value;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
