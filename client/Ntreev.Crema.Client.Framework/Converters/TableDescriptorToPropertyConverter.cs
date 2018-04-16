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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ntreev.Crema.Client.Framework.Converters
{
    public class TableDescriptorToPropertyConverter : IValueConverter
    {
        public readonly static string IsBeingEdited = nameof(IsBeingEdited);
        public readonly static string IsBeingEditedClient = nameof(IsBeingEditedClient);
        public readonly static string IsBeingSetup = nameof(IsBeingSetup);
        public readonly static string IsBeingSetupClient = nameof(IsBeingSetupClient);
        public readonly static string IsInherited = nameof(IsInherited);
        public readonly static string IsBaseTemplate = nameof(IsBaseTemplate);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Authenticator.Current == null)
            {
                return value;
            }

            if (parameter == null)
            {
                parameter = this.PropertyName;
            }

            if (value != null && parameter is string propertyName)
            {
                if (value is ITableDescriptor descriptor)
                {
                    if (propertyName == IsBeingEdited)
                    {
                        return TableDescriptorUtility.IsBeingEdited(Authenticator.Current, descriptor);
                    }
                    else if (propertyName == IsBeingEditedClient)
                    {
                        return TableDescriptorUtility.IsBeingEditedClient(Authenticator.Current, descriptor);
                    }
                    else if (propertyName == IsBeingSetup)
                    {
                        return TableDescriptorUtility.IsBeingSetup(Authenticator.Current, descriptor);
                    }
                    else if (propertyName == IsBeingSetupClient)
                    {
                        return TableDescriptorUtility.IsBeingSetupClient(Authenticator.Current, descriptor);
                    }
                    else if (propertyName == IsInherited)
                    {
                        return TableDescriptorUtility.IsInherited(Authenticator.Current, descriptor);
                    }
                    else if (propertyName == IsBaseTemplate)
                    {
                        return TableDescriptorUtility.IsBaseTemplate(Authenticator.Current, descriptor);
                    }
                }
                else
                {
                    var prop = value.GetType().GetProperty(propertyName);
                    if (prop != null)
                    {
                        return prop.GetValue(value);
                    }
                }
            }
            return value;
        }

        public string PropertyName
        {
            get; set;
        }

        #region IValueConverter

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
