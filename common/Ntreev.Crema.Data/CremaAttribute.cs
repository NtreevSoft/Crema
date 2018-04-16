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

using Ntreev.Crema.Data.Properties;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data
{
    public class CremaAttribute
    {
        private readonly InternalAttribute attribute;

        public CremaAttribute()
            : this(string.Empty, typeof(string))
        {

        }

        public CremaAttribute(string attributeName)
            : this(attributeName, typeof(string))
        {

        }

        public CremaAttribute(string attributeName, Type dataType)
        {
            this.attribute = new InternalAttribute(this, attributeName, dataType);
        }

        internal CremaAttribute(InternalAttribute attribute)
        {
            this.attribute = attribute;
        }

        public override string ToString()
        {
            return this.attribute.ToString();
        }

        [DefaultValue("")]
        public string AttributeName
        {
            get { return this.attribute.AttributeName; }
            set { this.attribute.AttributeName = value; }
        }

        public bool ReadOnly
        {
            get { return this.attribute.ReadOnly; }
            set { this.attribute.ReadOnly = value; }
        }

        public bool IsVisible
        {
            get { return this.attribute.IsVisible; }
            set { this.attribute.IsVisible = value; }
        }

        public Type DataType
        {
            get { return this.attribute.DataType; }
            set { this.attribute.DataType = value; }
        }

        public object DefaultValue
        {
            get { return this.attribute.DefaultValue; }
            set { this.attribute.DefaultValue = value; }
        }

        public bool AllowDBNull
        {
            get { return this.attribute.AllowDBNull; }
            set { this.attribute.AllowDBNull = value; }
        }

        public bool Unique
        {
            get { return this.attribute.Unique; }
            set { this.attribute.Unique = value; }
        }

        public bool AutoIncrement
        {
            get { return this.attribute.AutoIncrement; }
            set { this.attribute.AutoIncrement = value; }
        }

        public string Comment
        {
            get { return this.attribute.Comment; }
            set { this.attribute.Comment = value; }
        }

        public PropertyCollection ExtendedProperties
        {
            get { return this.attribute.ExtendedProperties; }
        }

        internal InternalAttribute InternalAttribute
        {
            get { return this.attribute; }
        }
    }
}
