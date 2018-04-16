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

//using Ntreev.Crema.Services;
//using Ntreev.Library;
//using Ntreev.Library.Commands;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.Composition;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Ntreev.Crema.Commands
//{
//    [Export(typeof(ICommand))]
//    class CacheCommand : CommandBase
//    {
//        private readonly CremaBootstrapper boot;

//        [ImportingConstructor]
//        public CacheCommand(CremaBootstrapper boot)
//            : base("cache")
//        {
//            this.RepositoryModule = CremaBootstrapper.DefaultRepositoryModule;
//            this.boot = boot;
//        }

//        [CommandProperty("path", IsRequired = true)]
//        public string Path
//        {
//            get;
//            set;
//        }

//        [CommandProperty]
//        public bool Force
//        {
//            get; set;
//        }

//        [CommandProperty("repo-module")]
//        public string RepositoryModule
//        {
//            get;
//            set;
//        }

//        [CommandProperty("repo-name")]
//        [DefaultValue("crema")]
//        public string RepositoryName
//        {
//            get;
//            set;
//        }

//        protected override void OnExecute()
//        {
//            this.boot.RepositoryModule = this.RepositoryModule;
//            this.boot.CreateRepository(this.Path, this.RepositoryName, this.Force);
//        }
//    }
//}
