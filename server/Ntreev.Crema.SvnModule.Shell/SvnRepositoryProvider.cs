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
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.SvnModule
{
    [Export(typeof(IRepositoryProvider))]
    [Export(typeof(IConfigurationPropertyProvider))]
    class SvnRepositoryProvider : IRepositoryProvider, IConfigurationPropertyProvider
    {
        public const string repositoryName = "crema";
        public const string remoteName = "svn";
        public const string trunkName = "trunk";
        public const string tagsName = "tags";

        [ImportMany]
        private IEnumerable<Lazy<ILogService>> logServices = null;

        [ImportingConstructor]
        public SvnRepositoryProvider()
        {

        }

        public string Name
        {
            get { return "svn"; }
        }

        public IRepository CreateInstance(string repositoryPath, string workingPath)
        {
            var logService = this.logServices.FirstOrDefault(item => item.Value.Name == "repository");
            return new SvnRepository(logService.Value, repositoryPath, workingPath);
        }

        public void CreateRepository(string basePath, string repositoryPath)
        {
            var remotePath = Path.Combine(basePath, remoteName);
            var svnUri = new Uri(remotePath);

            DirectoryUtility.Create(repositoryPath);
            DirectoryUtility.Prepare(repositoryPath, "tags");
            DirectoryUtility.Prepare(repositoryPath, "trunk");

            SvnServerHost.Run("create", remotePath.WrapQuot(), "--fs-type", "fsfs");
            SvnClientHost.Run("import", "-m", "first", repositoryPath.WrapQuot(), svnUri.ToString().WrapQuot());
            SvnClientHost.Run("checkout", svnUri.ToString().WrapQuot(), repositoryPath.WrapQuot(), "--force");

            CremaLog.Info("crema repository : \"{0}\"", repositoryPath);
            CremaLog.Info("  svn repository : \"{0}\"", remotePath);
        }

        public void ValidateRepository(string basePath, string repositoryPath)
        {
            if (DirectoryUtility.Exists(basePath) == false)
                throw new DirectoryNotFoundException($"base path does not exists :\"{basePath}\"");
            if (DirectoryUtility.Exists(repositoryPath) == false)
                throw new DirectoryNotFoundException($"repository path does not exists :\"{repositoryPath}\"");
        }

        //[ConfigurationProperty]
        //[DefaultValue("utf-8")]
        //public Encoding Encoding
        //{
        //    get; set;
        //}
    }
}
