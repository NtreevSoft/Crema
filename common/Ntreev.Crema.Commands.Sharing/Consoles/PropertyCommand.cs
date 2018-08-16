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

using Ntreev.Library;
using Ntreev.Crema.Services;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ntreev.Crema.ServiceModel;
using System.ComponentModel;
using Ntreev.Library.IO;
using System.Diagnostics;
using Ntreev.Crema.Commands.Consoles.Properties;
using Ntreev.Crema.Commands.Consoles.Serializations;
using Ntreev.Crema.Data.Xml;
using Ntreev.Library.Serialization;
using Ntreev.Crema.Commands.Consoles;
using Ntreev.Crema.Commands;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class PropertyCommand : ConsoleCommandMethodBase
    {
        private readonly ICremaHost cremaHost;

        [Import]
        private Lazy<IConfigurationProperties> properties = null;

        [ImportingConstructor]
        public PropertyCommand(ICremaHost cremaHost)
            : base("prop")
        {
            this.cremaHost = cremaHost;
        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            switch (methodDescriptor.DescriptorName)
            {
                case nameof(Set):
                case nameof(Reset):
                    {
                        var query = from item in this.Properties
                                    let propertyName = item.PropertyName
                                    select propertyName;
                        return query.ToArray();
                    }
            }
            return null;
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        public void List()
        {
            var props = new Dictionary<string, object>();
            foreach (var item in this.Properties)
            {
                if (StringUtility.GlobMany(item.PropertyName, FilterProperties.FilterExpression) == true)
                {
                    props.Add(item.PropertyName, item.Value);
                }
            }
            this.CommandContext.WriteObject(props, FormatProperties.Format);
        }

        [CommandMethod]
        public void Set(string propertyName, string value)
        {
            this.GetProperty(propertyName).Value = value;
        }

        [CommandMethod]
        public void Reset(string propertyName)
        {
            this.GetProperty(propertyName).Reset();
        }

        [CommandMethod]
        public void Edit()
        {
            var generator = new JSchemaGenerator();
            var schema = new JSchema();
            var propertiesInfo = new JsonPropertiesInfo();

            foreach (var item in this.Properties)
            {
                var itemSchema = generator.Generate(item.PropertyType);
                schema.Properties.Add(item.PropertyName, itemSchema);
                propertiesInfo.Add(item.PropertyName, item.Value);
            }

            var result = GetResult();
            if (result == null)
                return;

            foreach (var item in result)
            {
                if (this.Properties.ContainsKey(item.Key) == true)
                {
                    var value1 = item.Value;
                    var value2 = this.Properties[item.Key].Value;

                    if (object.Equals(value1, value2) == false)
                    {
                        this.Properties[item.Key].Value = value1;
                        this.CommandContext.WriteLine($"{item.Key} : {value2} => {value1}");
                    }
                }
            }

            JsonPropertiesInfo GetResult()
            {
                using (var editor = new JsonEditorHost(propertiesInfo, schema))
                {
                    if (editor.Execute() == false)
                        return null;

                    return editor.Read<JsonPropertiesInfo>();
                }
            }
        }

        private ConfigurationPropertyDescriptorCollection Properties
        {
            get => this.properties.Value.Properties;
        }

        private ConfigurationPropertyDescriptor GetProperty(string propertyName)
        {
            if (this.Properties.ContainsKey(propertyName) == false)
                throw new ArgumentException($"{propertyName} is not existed property.");
            return this.Properties[propertyName];
        }
    }
}