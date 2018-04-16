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

using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Ntreev.Library.IO;
using Ntreev.Library;
using System.Text.RegularExpressions;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Commands.Consoles
{
    class DataBasePath
    {
        private readonly string dataBaseName;
        private readonly string context;
        private readonly string itemPath;
        private readonly string path;

        public DataBasePath(string path)
        {
            var items = StringUtility.Split(path, PathUtility.SeparatorChar);
            if (items.Length > 0)
            {
                this.dataBaseName = items[0];
            }
            else
            {
                this.dataBaseName = null;
            }

            if (items.Length > 1)
            {
                this.context = items[1];
                if (items.Length > 2)
                    this.itemPath = PathUtility.Separator + string.Join(PathUtility.Separator, items.Skip(2));
                else
                    this.itemPath = PathUtility.Separator;
            }
            else
            {
                this.context = null;
                this.itemPath = null;
            }
            this.path = path;
        }

        public override string ToString()
        {
            return this.path;
        }

        public string Path
        {
            get { return this.path; }
        }

        public string DataBaseName
        {
            get { return this.dataBaseName ?? string.Empty; }
        }

        public string Context
        {
            get { return this.context ?? string.Empty; }
        }

        public string ItemPath
        {
            get { return this.itemPath ?? string.Empty; }
        }
    }
}
