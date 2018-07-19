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

using System;
using System.ComponentModel;
using System.Data;

namespace Ntreev.Crema.Data
{
    public class CremaAttribute
    {
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
            this.InternalAttribute = new InternalAttribute(this, attributeName, dataType);
        }

        internal CremaAttribute(InternalAttribute attribute)
        {
            this.InternalAttribute = attribute;
        }

        public override string ToString()
        {
            return this.InternalAttribute.ToString();
        }

        [DefaultValue("")]
        public string AttributeName
        {
            get => this.InternalAttribute.AttributeName;
            set => this.InternalAttribute.AttributeName = value;
        }

        public bool ReadOnly
        {
            get => this.InternalAttribute.ReadOnly;
            set => this.InternalAttribute.ReadOnly = value;
        }

        public bool IsVisible
        {
            get => this.InternalAttribute.IsVisible;
            set => this.InternalAttribute.IsVisible = value;
        }

        public Type DataType
        {
            get => this.InternalAttribute.DataType;
            set => this.InternalAttribute.DataType = value;
        }

        public object DefaultValue
        {
            get => this.InternalAttribute.DefaultValue;
            set => this.InternalAttribute.DefaultValue = value;
        }

        public bool AllowDBNull
        {
            get => this.InternalAttribute.AllowDBNull;
            set => this.InternalAttribute.AllowDBNull = value;
        }

        public bool Unique
        {
            get => this.InternalAttribute.Unique;
            set => this.InternalAttribute.Unique = value;
        }

        public bool AutoIncrement
        {
            get => this.InternalAttribute.AutoIncrement;
            set => this.InternalAttribute.AutoIncrement = value;
        }

        public string Comment
        {
            get => this.InternalAttribute.Comment;
            set => this.InternalAttribute.Comment = value;
        }

        public PropertyCollection ExtendedProperties => this.InternalAttribute.ExtendedProperties;

        internal InternalAttribute InternalAttribute { get; }
    }
}
