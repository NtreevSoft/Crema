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

using Ntreev.Crema.Data;
using Ntreev.Crema.Services;
using Ntreev.Crema.Services.Users;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
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

        public IRepository CreateInstance(string basePath, string repositoryName, string workingPath)
        {
            var baseUri = new Uri(basePath);
            var url = repositoryName == "default" ? UriUtility.Combine(baseUri, "trunk") : UriUtility.Combine(baseUri, "branches", repositoryName);
            //throw new NotImplementedException();
            var logService = this.logServices.FirstOrDefault(item => item.Value.Name == "repository");

            if (Directory.Exists(workingPath) == false)
            {
                SvnClientHost.Run("checkout", url.ToString().WrapQuot(), workingPath.WrapQuot());
            }
            else
            {
                SvnClientHost.Run("update", workingPath.WrapQuot());
            }

            return new SvnRepository(logService.Value, workingPath, null);
        }

        public void Initialize(string basePath, string initPath)
        {
            var baseUri = new Uri(basePath);

            var tempPath = PathUtility.GetTempPath(true);
            DirectoryUtility.Create(tempPath);
            var tagsPath = DirectoryUtility.Prepare(tempPath, "tags");
            var branchesPath = DirectoryUtility.Prepare(tempPath, "branches");
            var trunkPath = DirectoryUtility.Prepare(tempPath, "trunk");

            //var tempPath = PathUtility.GetTempPath(true);
            //var usersFilename = Path.Combine(repositoryPath, "users.xml");
            SvnServerHost.Run("create", basePath.WrapQuot(), "--fs-type", "fsfs");

            DirectoryUtility.Copy(initPath, trunkPath);
            //DirectoryUtility.Copy(dataBasesPath, branchesPath);

            //foreach (var item in repositoryNames)
            //{
            //    DirectoryUtility.Prepare(initPath, item);
            //}

            //SvnClientHost.Run("checkout", svnUri.ToString().WrapQuot(), repositoryPath.WrapQuot(), "--force");
            //DataContractSerializerUtility.Write(usersFilename, users, true);
            //DirectoryUtility.Create(repositoryPath);
            //var trunkPath = DirectoryUtility.Prepare(repositoryPath, "databases", "default");
            //dataSet.WriteToDirectory(trunkPath);
            //DirectoryUtility.Prepare(repositoryPath, "users");

            //foreach (var item in users.Users)
            //{
            //    var categoryPath = DirectoryUtility.Prepare(repositoryPath, item.CategoryName);
            //    var userPath = FileUtility.Prepare(repositoryPath, "users", item.ID + ".xml");
            //    DataContractSerializerUtility.Write(userPath, item, true);
            //}

            SvnClientHost.Run("import", "-m", "first", tempPath.WrapQuot(), baseUri.ToString().WrapQuot());
            //SvnClientHost.Run("mkdir", "-m", "create trunk".WrapQuot(), UriUtility.Combine(svnUri, "trunk"));
            //SvnClientHost.Run("mkdir", "-m", "create trunk/types".WrapQuot(), UriUtility.Combine(svnUri, "trunk", "types"));
            //SvnClientHost.Run("mkdir", "-m", "create trunk/tables".WrapQuot(), UriUtility.Combine(svnUri, "trunk", "tables"));
            //SvnClientHost.Run("mkdir", "-m", "create branches".WrapQuot(), UriUtility.Combine(svnUri, "branches"));
            //SvnClientHost.Run("mkdir", "-m", "create tags".WrapQuot(), UriUtility.Combine(svnUri, "tags"));



            //SvnClientHost.Run("checkout", svnUri.ToString().WrapQuot(), repositoryPath.WrapQuot(), "--force");

            //CremaLog.Info("crema repository : \"{0}\"", repositoryPath);
            //CremaLog.Info("  svn repository : \"{0}\"", remotePath);

            //return baseUri;
        }

        public void CreateRepository(string basePath, params string[] repositoryNames)
        {
            ////var remotePath = Path.Combine(basePath, remoteName);
            //var baseUri = new Uri(basePath);

            //var tempPath = PathUtility.GetTempPath(true);
            //DirectoryUtility.Create(tempPath);
            //var branchesPath = DirectoryUtility.Prepare(tempPath, "branches");
            //var trunkPath = DirectoryUtility.Prepare(tempPath, "trunk");

            ////var tempPath = PathUtility.GetTempPath(true);
            ////var usersFilename = Path.Combine(repositoryPath, "users.xml");
            //SvnServerHost.Run("create", basePath.WrapQuot(), "--fs-type", "fsfs");

            //DirectoryUtility.Copy(rootPath, trunkPath);
            //DirectoryUtility.Copy(dataBasesPath, branchesPath);

            ////foreach (var item in repositories)
            ////{
            ////    DirectoryUtility.Prepare(tempPath, item);
            ////}

            ////SvnClientHost.Run("checkout", svnUri.ToString().WrapQuot(), repositoryPath.WrapQuot(), "--force");
            ////DataContractSerializerUtility.Write(usersFilename, users, true);
            ////DirectoryUtility.Create(repositoryPath);
            ////var trunkPath = DirectoryUtility.Prepare(repositoryPath, "databases", "default");
            ////dataSet.WriteToDirectory(trunkPath);
            ////DirectoryUtility.Prepare(repositoryPath, "users");

            ////foreach (var item in users.Users)
            ////{
            ////    var categoryPath = DirectoryUtility.Prepare(repositoryPath, item.CategoryName);
            ////    var userPath = FileUtility.Prepare(repositoryPath, "users", item.ID + ".xml");
            ////    DataContractSerializerUtility.Write(userPath, item, true);
            ////}

            //SvnClientHost.Run("import", "-m", "first", tempPath.WrapQuot(), baseUri.ToString().WrapQuot());
            ////SvnClientHost.Run("mkdir", "-m", "create trunk".WrapQuot(), UriUtility.Combine(svnUri, "trunk"));
            ////SvnClientHost.Run("mkdir", "-m", "create trunk/types".WrapQuot(), UriUtility.Combine(svnUri, "trunk", "types"));
            ////SvnClientHost.Run("mkdir", "-m", "create trunk/tables".WrapQuot(), UriUtility.Combine(svnUri, "trunk", "tables"));
            ////SvnClientHost.Run("mkdir", "-m", "create branches".WrapQuot(), UriUtility.Combine(svnUri, "branches"));
            ////SvnClientHost.Run("mkdir", "-m", "create tags".WrapQuot(), UriUtility.Combine(svnUri, "tags"));



            ////SvnClientHost.Run("checkout", svnUri.ToString().WrapQuot(), repositoryPath.WrapQuot(), "--force");

            ////CremaLog.Info("crema repository : \"{0}\"", repositoryPath);
            ////CremaLog.Info("  svn repository : \"{0}\"", remotePath);

            ////return baseUri;
        }

        public IEnumerable<string> GetRepositories(string basePath)
        {
            var uri = new Uri(basePath);
            var list = SvnClientHost.Run("list", $"{uri}".WrapQuot());
            var sr = new StringReader(list);
            var line = string.Empty;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.EndsWith(PathUtility.Separator) == true)
                {
                    var name = line.Substring(0, line.Length - PathUtility.Separator.Length);
                    if (name == "trunk")
                    {
                        yield return "default";
                    }
                    else if (name == "tags" || name == "branches")
                    {
                        var subPath = Path.Combine(basePath, name);
                        foreach (var item in this.GetRepositories(subPath))
                        {
                            yield return item;
                        }
                    }
                    else
                    {
                        yield return name;
                    }
                }
            }
        }

        public void ValidateRepository(string basePath, string repositoryPath)
        {
            if (DirectoryUtility.Exists(basePath) == false)
                throw new DirectoryNotFoundException($"base path does not exists :\"{basePath}\"");
            if (DirectoryUtility.Exists(repositoryPath) == false)
                throw new DirectoryNotFoundException($"repository path does not exists :\"{repositoryPath}\"");
        }

        public string GetRevision(string basePath)
        {
            var uri = new Uri(basePath);
            var args = SvnInfoEventArgs.Run($"{uri}".WrapQuot());
            return $"{args.Revision}";
        }
    }
}
