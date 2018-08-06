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
using Ntreev.Crema.ServiceModel;
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
using YamlDotNet.Serialization;

namespace Ntreev.Crema.Repository.Svn
{
    [Export]
    [Export(typeof(IRepositoryProvider))]
    [Export(typeof(IConfigurationPropertyProvider))]
    class SvnRepositoryProvider : IRepositoryProvider, IConfigurationPropertyProvider
    {
        private const string propertyPrefix = "prop:";

        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        [ImportingConstructor]
        public SvnRepositoryProvider()
        {

        }

        public string Name => SvnString.Name;

        public IRepository CreateInstance(RepositorySettings settings)
        {
            var baseUri = new Uri(settings.BasePath);
            var repositoryName = settings.RepositoryName == string.Empty ? SvnString.Default : settings.RepositoryName;
            var url = repositoryName == SvnString.Default ? UriUtility.Combine(baseUri, SvnString.Trunk) : UriUtility.Combine(baseUri, SvnString.Branches, settings.RepositoryName);

            if (Directory.Exists(settings.WorkingPath) == false)
            {
                var checkoutCommand = new SvnCommand("checkout")
                {
                    (SvnPath)url,
                    (SvnPath)settings.WorkingPath,
                };
                checkoutCommand.Run();
            }
            else
            {
                var updateCommand = new SvnCommand("update")
                {
                    (SvnPath)settings.WorkingPath,
                };
                updateCommand.Run();
            }

            var repositoryInfo = this.GetRepositoryInfo(settings.BasePath, repositoryName);
            return new SvnRepository(this, settings.LogService, settings.WorkingPath, settings.TransactionPath, repositoryInfo);
        }

        public void InitializeRepository(string basePath, string initPath)
        {
            var baseUri = new Uri(basePath);
            var tempPath = PathUtility.GetTempPath(true);
            var tagsPath = DirectoryUtility.Prepare(tempPath, SvnString.Tags);
            var branchesPath = DirectoryUtility.Prepare(tempPath, SvnString.Branches);
            var trunkPath = DirectoryUtility.Prepare(tempPath, SvnString.Trunk);

            if (baseUri.Scheme == Uri.UriSchemeFile)
            {
                var createCommand = new SvnAdminCommand("create")
                {
                    (SvnPath)basePath,
                    "--fs-type",
                    "fsfs"
                };
                createCommand.Run();
            }

            DirectoryUtility.Copy(initPath, trunkPath);

            var importCommand = new SvnCommand("import")
            {
                SvnCommandItem.FromMessage("first"),
                (SvnPath)tempPath,
                (SvnPath)baseUri,
            };
            importCommand.Run();
        }

        public void CreateRepository(string author, string basePath, string initPath, string comment, params LogPropertyInfo[] properties)
        {
            var repositoryName = Path.GetFileName(initPath);
            var uri = UriUtility.Combine(new Uri(basePath), SvnString.Branches, repositoryName);
            var props = GeneratePropertiesArgument(properties);
            var importCommand = new SvnCommand("import")
            {
                (SvnPath)initPath,
                (SvnPath)uri,
                SvnCommandItem.FromMessage(comment),
                SvnCommandItem.Force,
                props,
                SvnCommandItem.FromUsername(author),
            };
            importCommand.Run();
        }

        public void CopyRepository(string author, string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var newUri = this.GenerateUrl(basePath, newRepositoryName);
            var props = GeneratePropertiesArgument(properties);
            var copyCommand = new SvnCommand("copy")
            {
                SvnCommandItem.FromMessage(comment),
                (SvnPath)uri,
                (SvnPath)newUri,
                props,
                SvnCommandItem.FromUsername(author),
            };
            copyCommand.Run();
        }

        public void RenameRepository(string author, string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var newUri = this.GenerateUrl(basePath, newRepositoryName);
            var props = GeneratePropertiesArgument(properties);
            var moveCommand = new SvnCommand("move")
            {
                SvnCommandItem.FromMessage(comment),
                (SvnPath)uri,
                (SvnPath)newUri,
                props,
                SvnCommandItem.FromUsername(author),
            };
            moveCommand.Run();
        }

        public void DeleteRepository(string author, string basePath, string repositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var props = GeneratePropertiesArgument(properties);
            var deleteCommand = new SvnCommand("delete")
            {
                SvnCommandItem.FromMessage(comment),
                (SvnPath)uri,
                props,
                SvnCommandItem.FromUsername(author),
            };
            deleteCommand.Run();
        }

        public void RevertRepository(string author, string basePath, string repositoryName, string revision, string comment)
        {
            var baseUri = new Uri(basePath);
            var url = repositoryName == SvnString.Default ? UriUtility.Combine(baseUri, SvnString.Trunk) : UriUtility.Combine(baseUri, SvnString.Branches, repositoryName);
            var tempPath = PathUtility.GetTempPath(false);
            try
            {
                var checkoutCommand = new SvnCommand("checkout")
                {
                    (SvnPath)url,
                    (SvnPath)tempPath,
                };
                checkoutCommand.Run();
                var mergeCommand = new SvnCommand("merge")
                {
                    new SvnCommandItem('r', $"head:{revision}"),
                    (SvnPath)tempPath,
                    (SvnPath)tempPath,
                };
                mergeCommand.Run();
                var commitCommand = new SvnCommand("commit")
                {
                    (SvnPath)tempPath,
                    SvnCommandItem.FromMessage(comment),
                    SvnCommandItem.FromEncoding(Encoding.UTF8),
                    SvnCommandItem.FromUsername(author),
                };
                commitCommand.Run();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        public string[] GetRepositories(string basePath)
        {
            var itemList = new List<string>();
            foreach (var item in this.EnumerateRepositories(basePath))
            {
                itemList.Add(item);
            }
            return itemList.ToArray();
        }

        public string GetRevision(string basePath, string repositoryName)
        {
            var url = this.GetUrl(basePath, repositoryName);
            var infoCommand = new SvnCommand("info")
            {
                (SvnPath)url,
                new SvnCommandItem("show-item", "last-changed-revision"),
            };
            return infoCommand.ReadLine();
        }

        public RepositoryInfo GetRepositoryInfo(string basePath, string repositoryName)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var latestLog = SvnLogInfo.GetLatestLog($"{uri}");
            var firstLog = SvnLogInfo.GetFirstLog($"{uri}");
            var repositoryInfo = new RepositoryInfo()
            {
                ID = GuidUtility.FromName(repositoryName + firstLog.Revision),
                Name = repositoryName,
                Comment = firstLog.Comment,
                Revision = latestLog.Revision,
                CreationInfo = new SignatureDate(firstLog.Author, firstLog.DateTime),
                ModificationInfo = new SignatureDate(latestLog.Author, latestLog.DateTime),
            };
            return repositoryInfo;
        }

        public string[] GetRepositoryItemList(string basePath, string repositoryName)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var listCommand = new SvnCommand("list") { (SvnPath)uri, SvnCommandItem.Recursive };
            var lines = listCommand.ReadLines();
            var query = from item in lines
                        where item.Trim() != string.Empty
                        select PathUtility.Separator + item;
            return query.ToArray();
        }

        public LogInfo[] GetLog(string basePath, string repositoryName, string revision)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var logs = SvnLogInfo.GetLogs(uri.ToString(), revision);
            return logs.Select(item => (LogInfo)item).ToArray();
        }

        public IEnumerable<KeyValuePair<string, Uri>> GetRepositoryPaths(string basePath)
        {
            var uri = new Uri(basePath);
            var listCommand = new SvnCommand("list") { (SvnPath)uri };
            var lines = listCommand.ReadLines();
            foreach (var line in lines)
            {
                if (line.EndsWith(PathUtility.Separator) == true)
                {
                    var name = line.Substring(0, line.Length - PathUtility.Separator.Length);
                    if (name == SvnString.Trunk)
                    {
                        yield return new KeyValuePair<string, Uri>(SvnString.Default, UriUtility.Combine(uri, name));
                    }
                    else if (name == SvnString.Tags || name == SvnString.Branches)
                    {
                        var subPath = Path.Combine(basePath, name);
                        foreach (var item in this.GetRepositoryPaths(subPath))
                        {
                            yield return item;
                        }
                    }
                    else
                    {
                        yield return new KeyValuePair<string, Uri>(name, UriUtility.Combine(uri, name));
                    }
                }
            }
        }

        public static string GeneratePropertiesArgument(LogPropertyInfo[] properties)
        {
            return string.Join(" ", properties.Select(item => $"--with-revprop \"{propertyPrefix}{item.Key}={item.Value}\""));
        }

        private Uri GetUrl(string basePath, string repositoryName)
        {
            var paths = this.GetRepositoryPaths(basePath).ToDictionary(item => item.Key, item => item.Value);
            return repositoryName == string.Empty ? paths[SvnString.Default] : paths[repositoryName];
        }

        private Uri GenerateUrl(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            return UriUtility.Combine(baseUri, SvnString.Branches, repositoryName);
        }

        private ICremaHost CremaHost => this.cremaHost.Value;

        private IEnumerable<string> EnumerateRepositories(string basePath)
        {
            var uri = new Uri(basePath);
            var listCommand = new SvnCommand("list") { (SvnPath)uri };
            var lines = listCommand.ReadLines();
            foreach (var line in lines)
            {
                if (line.EndsWith(PathUtility.Separator) == true)
                {
                    var name = line.Substring(0, line.Length - PathUtility.Separator.Length);
                    if (name == SvnString.Trunk)
                    {
                        yield return "default";
                    }
                    else if (name == SvnString.Tags || name == SvnString.Branches)
                    {
                        var subPath = Path.Combine(basePath, name);
                        foreach (var item in this.EnumerateRepositories(subPath))
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
    }
}
