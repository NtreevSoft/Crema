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

using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    class RepositoryHost
    {
        private readonly IRepository repository;
        private readonly CremaDispatcher dispatcher;
        private readonly string path;
        private string revision;
        private SignatureDate signatureDate;

        public RepositoryHost(IRepository repository, CremaDispatcher dispatcher, string path)
        {
            this.repository = repository;
            this.dispatcher = dispatcher;
            this.path = path;
            this.revision = this.repository.GetRevision(path);
        }

        public void Add(string path)
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.Add(path);
            });
        }

        public void Add(string path, string contents)
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.Add(path, contents);
            });
        }

        public void Modify(string path, string contents)
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.Modify(path, contents);
            });
        }

        public void Move(string srcPath, string toPath)
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.Move(srcPath, toPath);
            });
        }

        public void Delete(params string[] paths)
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.Delete(paths);
            });
        }

        public void Copy(string srcPath, string toPath)
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.Copy(srcPath, toPath);
            });
        }

        public void Revert()
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.Revert(this.path);
            });
        }

        public void Revert(string revision)
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.Revert(this.path, revision);
            });
        }

        public void BeginTransaction(string name)
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.BeginTransaction(this.path, name);
            });
        }

        public void EndTransaction()
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.EndTransaction(this.path);
            });
        }

        public void CancelTransaction()
        {
            this.dispatcher.Invoke(() =>
            {
                this.repository.CancelTransaction(this.path);
            });
        }

        public string GetRevision(string path)
        {
            return this.dispatcher.Invoke(() => this.repository.GetRevision(path));
        }

        public Uri GetUri(string path, string revision)
        {
            return this.dispatcher.Invoke(() => this.repository.GetUri(path, revision));
        }

        public string Export(Uri uri, string exportPath)
        {
            return this.dispatcher.Invoke(() => this.repository.Export(uri, exportPath));
        }

        public void Commit(Authentication authentication, string comment, string eventLog)
        {
            this.Commit(authentication, null, comment, eventLog);
        }

        public void Commit(Authentication authentication, IEnumerable<LogPropertyInfo> properties, string comment, string eventLog)
        {
            var propList = new List<LogPropertyInfo>
            {
                new LogPropertyInfo() { Key = LogPropertyInfo.EventLogKey, Value = eventLog},
                new LogPropertyInfo() { Key = LogPropertyInfo.VersionKey, Value = AppUtility.ProductVersion},
                new LogPropertyInfo() { Key = LogPropertyInfo.UserIDKey, Value = authentication.ID}
            };

            if (properties != null)
                propList.AddRange(properties);

            var dateTime = this.dispatcher.Invoke(() => this.repository.Commit(this.path, comment, propList));
            this.revision = this.repository.Revision;
            this.signatureDate = new SignatureDate(authentication.ID, dateTime);
            this.OnChanged(EventArgs.Empty);
        }

        public LogInfo[] GetLog(string path, string revision, int count)
        {
            return this.dispatcher.Invoke(() => this.repository.GetLog(path, revision, count));
        }

        public string GetDataBaseUri(string repoUri, string itemUri)
        {
            var pattern = "(@\\d+)$";
            var pureRepoUri = Regex.Replace(repoUri, pattern, string.Empty);
            var pureItemUri = Regex.Replace(itemUri, pattern, string.Empty);
            var relativeUri = UriUtility.MakeRelativeOfDirectory(pureRepoUri, pureItemUri);
            var segments = relativeUri.Split(PathUtility.SeparatorChar);
            if (segments[0] == "trunk")
            {
                pureRepoUri = $"{UriUtility.Combine(pureRepoUri, segments.Take(1).ToArray())}";
            }
            else if (segments[0] == "tags")
            {
                pureRepoUri = $"{UriUtility.Combine(pureRepoUri, segments.Take(2).ToArray())}";
            }

            return pureRepoUri;
        }

        public void Dispose()
        {
            this.repository.Dispose();
        }

        public string Revision
        {
            get { return this.revision; }
        }

        public SignatureDate SignatureDate
        {
            get { return this.signatureDate; }
        }

        public event EventHandler Changed;

        protected CremaDispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        protected IRepository Repository
        {
            get { return this.repository; }
        }

        protected void OnChanged(EventArgs e)
        {
            this.Changed?.Invoke(this, e);
        }
    }
}
