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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    class InternalAttribute : InternalColumnBase
    {
        public InternalAttribute()
        {
            base.Target = new CremaAttribute(this);
            this.ColumnMapping = MappingType.Attribute;
        }

        public InternalAttribute(CremaAttribute target, string attributeName, Type dataType)
            : this(attributeName, dataType)
        {
            base.Target = target;
        }

        public InternalAttribute(string attributeName, Type dataType)
            : base(attributeName, dataType)
        {
            this.ValidateConstructor(attributeName, dataType);
            this.ColumnMapping = MappingType.Attribute;
        }

        public static explicit operator CremaAttribute(InternalAttribute attribute)
        {
            if (attribute == null)
                return null;
            return attribute.Target;
        }

        public static explicit operator InternalAttribute(CremaAttribute attribute)
        {
            if (attribute == null)
                return null;
            return attribute.InternalAttribute;
        }

        public void CopyTo(InternalAttribute dest)
        {
            dest.InternalAllowDBNull = this.InternalAllowDBNull;
            dest.InternalAutoIncrement = this.InternalAutoIncrement;
            dest.InternalColumnName = this.InternalColumnName;
            dest.InternalDataType = this.InternalDataType;
            dest.InternalDefaultValue = this.InternalDefaultValue;
            dest.InternalExpression = this.InternalExpression;
            dest.InternalUnique = this.InternalUnique;
            dest.InternalReadOnly = this.InternalReadOnly;
            dest.InternalComment = this.InternalComment;
        }

        public void ValidateSetIsVisible(bool value)
        {
            this.ValidateSetProperty(nameof(this.IsVisible));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetIsVisible(value);
            }
        }

        public void ValidateSetComment(string value)
        {
            this.ValidateSetProperty(nameof(this.Comment));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetComment(value);
            }
        }

        public new CremaAttribute Target
        {
            get
            {
                if (base.Target == null)
                    base.Target = new CremaAttribute(this);
                return base.Target as CremaAttribute;
            }
        }

        [Obsolete]
        public new string ColumnName => base.ColumnName;

        public string AttributeName
        {
            get => base.ColumnName;
            set => base.ColumnName = value;
        }

        public bool IsVisible
        {
            get => this.InternalIsVisible;
            set
            {
                if (this.InternalIsVisible == value)
                    return;
                this.ValidateSetIsVisible(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalIsVisible = value;
                }
            }
        }

        public string Comment
        {
            get => this.InternalComment ?? string.Empty;
            set
            {
                if (this.InternalComment == value)
                    return;
                this.ValidateSetComment(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalComment = value;
                }
            }
        }

        public bool InternalIsVisible
        {
            get => this.ColumnMapping == MappingType.Attribute;
            set => this.ColumnMapping = value == true ? MappingType.Attribute : MappingType.Hidden;
        }

        public string InternalComment { get; set; }

        public new IEnumerable<InternalAttribute> DerivedColumns
        {
            get
            {
                foreach (var item in base.DerivedColumns)
                {
                    if (item is InternalAttribute attributeItem)
                        yield return attributeItem;
                }
            }
        }

        public new IEnumerable<InternalAttribute> SiblingColumns
        {
            get
            {
                foreach (var item in base.SiblingColumns)
                {
                    if (item is InternalAttribute attributeItem)
                        yield return attributeItem;
                }
            }
        }

        public static void ValidateAttributeName(string attributeName)
        {
            CremaDataSet.ValidateName(attributeName);

            foreach (var item in CremaSchema.ReservedNames)
            {
                if (item == attributeName)
                    throw new ArgumentException(string.Format(Resources.Exception_ReservedAttributeName_Format, attributeName));
            }
        }

        protected virtual void OnValidateSetIsVisible(bool value)
        {

        }

        protected virtual void OnValidateSetComment(string value)
        {

        }

        private void ValidateConstructor(string attributeName, Type dataType)
        {
            if (string.IsNullOrEmpty(attributeName) == false)
                InternalAttribute.ValidateAttributeName(attributeName);
            if (dataType == null)
                throw new ArgumentNullException(nameof(dataType));
            if (CremaDataTypeUtility.IsBaseType(dataType) == false)
                throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, dataType.Name), nameof(dataType));
        }
    }
}
