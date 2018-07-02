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

using Ntreev.Crema.Commands.Properties;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands
{
    [Export(typeof(ICommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class InitializeCommand : CommandBase
    {
        private readonly CremaBootstrapper boot;

        [ImportingConstructor]
        public InitializeCommand(CremaBootstrapper boot)
            : base("init")
        {
            this.boot = boot;
        }

        [CommandProperty("path", IsRequired = true)]
        public string Path
        {
            get;
            set;
        }

        [CommandProperty]
        public bool Force
        {
            get; set;
        }

        [CommandProperty("repo-module")]
        public string RepositoryModule
        {
            get;
            set;
        }

        [CommandProperty("file-type")]
        public string FileType
        {
            get;
            set;
        }

        protected override void OnExecute()
        {
            var directoryInfo = new DirectoryInfo(this.Path);
            if (directoryInfo.Exists == false && this.Force == true)
            {
                DirectoryUtility.Create(directoryInfo.FullName);
            }
            if (DirectoryUtility.IsEmpty(directoryInfo.FullName) == false && this.Force == false)
            {
                throw new ArgumentException("Path is not an empty directory.", nameof(this.Path));
            }
            CremaBootstrapper.CreateRepository(this.boot, this.Path, this.RepositoryModule, this.FileType, this.Force);
        }
    }
}
