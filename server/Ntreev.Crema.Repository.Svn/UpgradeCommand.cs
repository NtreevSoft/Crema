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
using Ntreev.Library.Commands;
using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using Ntreev.Crema.Commands.Consoles;
using System.Collections.Generic;
using Ntreev.Library.IO;
using Ntreev.Crema.Services;
using Ntreev.Crema.Services.Users.Serializations;

namespace Ntreev.Crema.Repository.Svn
{
    [Export(typeof(ICommand))]
    [Export]
    [ResourceDescription]
    class UpgradeCommand : CommandBase
    {
        private readonly CremaBootstrapper boot;

        [ImportingConstructor]
        public UpgradeCommand(CremaBootstrapper boot)
            : base("upgrade")
        {
            this.boot = boot;
        }

        [CommandProperty("path", IsRequired = true)]
        public string BasePath
        {
            get;
            set;
        }

        //[CommandProperty("repo-module")]
        //public string RepositoryModule
        //{
        //    get => this.repositoryModule;
        //    set => this.repositoryModule = value;
        //}

        //[CommandProperty("file-type")]
        //public string FileType
        //{
        //    get;
        //    set;
        //}

        //[CommandPropertyArray]
        //[Description("database list to migrate")]
        //public string[] DataBaseList
        //{
        //    get; set;
        //}

        protected override void OnExecute()
        {
            this.UpgradeDataBases();
            //var svnPath = Path.Combine(this.BasePath, "svn");
            //var repositoryPath = Path.Combine(this.BasePath, ".repository");
            //var dataBasesPath = Path.Combine(repositoryPath, "databases");
            //var usersPath = Path.Combine(repositoryPath, "users");

            //var repositoryProvider = CremaBootstrapper.GetRepositoryProvider(this.boot, "svn");
            //var serializer = CremaBootstrapper.GetSerializer(this.boot, "xml");

            //DirectoryUtility.Copy(svnPath, dataBasesPath);

            //var svnUri = new Uri(svnPath);
            //var userUri = UriUtility.Combine(svnUri, "users.xml");

            //SvnClientHost.Run("export", userUri, this.BasePath.ToSvnPath(), "--force");

            //var userPath = Path.Combine(this.BasePath, "users.xml");
            //var userContext = Ntreev.Library.Serialization.DataContractSerializerUtility.Read<UserContextSerializationInfo>(userPath);

            //var tempPath = PathUtility.GetTempPath(true);
            //userContext.WriteToDirectory(tempPath);
            //repositoryProvider.InitializeRepository(usersPath, tempPath);
        }

        private void UpgradeUsers()
        {
            var repositoryProvider = CremaBootstrapper.GetRepositoryProvider(this.boot, "svn");
            var serializer = CremaBootstrapper.GetSerializer(this.boot, "xml");

            var svnPath = Path.Combine(this.BasePath, "svn");
            var repositoryPath = Path.Combine(this.BasePath, ".repository");
            var usersPath = Path.Combine(repositoryPath, "users");
            var svnUri = new Uri(svnPath);
            var userUri = UriUtility.Combine(svnUri, "users.xml");
            var userPath = Path.Combine(this.BasePath, "users.xml");

            SvnClientHost.Run("export", userUri, this.BasePath.WrapQuot(), "--force");

            var userContext = Ntreev.Library.Serialization.DataContractSerializerUtility.Read<UserContextSerializationInfo>(userPath);
            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                userContext.WriteToDirectory(tempPath);
                repositoryProvider.InitializeRepository(usersPath, tempPath);
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        private void UpgradeDataBases()
        {
            var svnPath = Path.Combine(this.BasePath, "svn");
            var repositoryPath = Path.Combine(this.BasePath, ".repository");
            var dataBasesPath = Path.Combine(repositoryPath, "databases");

            var repositoryProvider = CremaBootstrapper.GetRepositoryProvider(this.boot, "svn");
            var serializer = CremaBootstrapper.GetSerializer(this.boot, "xml");

            var dataBasesUri = new Uri(dataBasesPath);
            //var svnUri = new Uri(svnPath);

            //var tagsUri = UriUtility.Combine(svnUri, "tags");
            //var branchesUri = UriUtility.Combine(svnUri, "branches");
            DirectoryUtility.Copy(svnPath, dataBasesPath);
            this.PrepareBranches(dataBasesUri);
            this.MoveTagsToBranches(dataBasesUri);
            //SvnClientHost.Run("export", userUri, this.BasePath.ToSvnPath(), "--force");


        }

        private void MoveTagsToBranches(Uri dataBasesUri)
        {
            var tagsUri = UriUtility.Combine(dataBasesUri, "tags");
            var branchesUri = UriUtility.Combine(dataBasesUri, "branches");
            var text = SvnClientHost.Run("list", tagsUri);
            var list = this.GetLines(text);

            foreach (var item in list)
            {
                if (item.EndsWith(PathUtility.Separator) == true)
                {
                    var name = item.Remove(item.Length - PathUtility.Separator.Length);

                    var sourceUri = UriUtility.Combine(tagsUri, name);
                    var destUri = UriUtility.Combine(branchesUri, name);

                    SvnClientHost.Run("mv", sourceUri, destUri, "-m", $"\"Upgrade: move {name} from tags to branches\"");
                }
            }
        }

        private void PrepareBranches(Uri dataBasesUri)
        {
            //var branchesUri = UriUtility.Combine(svnUri, "branches");
            var text = SvnClientHost.Run("list", dataBasesUri);
            var list = this.GetLines(text);

            if (list.Contains("branches/") == false)
            {
                var branchesUri = UriUtility.Combine(dataBasesUri, "branches");
                SvnClientHost.Run("mkdir", branchesUri, "-m", "\"Upgrade: create branches\"");
            }

        }


        private string[] GetLines(string text)
        {
            var sr = new StringReader(text);
            var line = null as string;
            var lineList = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                lineList.Add(line);
            }
            return lineList.ToArray();
        }

    }
}
