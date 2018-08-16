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

using Ntreev.Crema.Commands;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ConsoleHost
{
    [Export(typeof(IConsoleConfiguration))]
    [Export]
    class ConsoleConfiguration : ConfigurationBase, IConsoleConfiguration
    {
        private readonly string xsdPath;
        private readonly string xmlPath;
        private readonly string schemaLocation;

        [ImportingConstructor]
        public ConsoleConfiguration([ImportMany]IEnumerable<IConfigurationPropertyProvider> propertiesProvider)
            : base(propertiesProvider)
        {
            this.xmlPath = AppUtility.GetDocumentFilename("configs") + ".xml";
            this.xsdPath = AppUtility.GetDocumentFilename("configs") + ".xsd";
            this.schemaLocation = UriUtility.MakeRelative(this.xmlPath, this.xsdPath);
            this.Read(this.xmlPath);
        }

        public void Write()
        {
            FileUtility.Backup(this.xsdPath);
            FileUtility.Backup(this.xmlPath);
            this.WriteSchema(this.xsdPath);
            this.Write(this.xmlPath, this.schemaLocation);
        }

        public override string Name => "ConsoleConfigs";
    }
}
