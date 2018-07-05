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
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    class CremaSettings
    {
        private string[] dataBaseList;

        public CremaSettings()
        {

        }

        public string BasePath
        {
            get;
            set;
        }

        public bool MultiThreading
        {
            get;
            set;
        }

        public string RepositoryModule
        {
            get => FileUtility.ReadAllText(this.BasePath, CremaString.Repository, CremaString.Repo);
        }

        public string FileType
        {
            get => FileUtility.ReadAllText(this.BasePath, CremaString.Repository, CremaString.File);
        }

        public string RepositoryDataBasesUrl
        {
            get
            {
                var urlPath = Path.Combine(this.BasePath, CremaString.Repository, "databasesUrl");
                if (File.Exists(urlPath) == true)
                {
                    var text = File.ReadAllText(urlPath).Trim();
                    if (text != string.Empty)
                    {
                        var url = new Uri(File.ReadAllText(urlPath), UriKind.RelativeOrAbsolute);
                        if (url.IsAbsoluteUri)
                        {
                            return url.ToString();
                        }
                        else
                        {
                            return UriUtility.Combine(this.BasePath, CremaString.Repository, CremaString.DataBases, text);
                        }
                    }
                }
                return Path.Combine(this.BasePath, CremaString.Repository, CremaString.DataBases);
            }
        }

        public string RepositoryUsersUrl
        {
            get
            {
                var urlPath = Path.Combine(this.BasePath, CremaString.Repository, "usersUrl");
                if (File.Exists(urlPath) == true)
                {
                    var text = File.ReadAllText(urlPath).Trim();
                    if (text != string.Empty)
                    {
                        var url = new Uri(File.ReadAllText(urlPath), UriKind.RelativeOrAbsolute);
                        if (url.IsAbsoluteUri)
                        {
                            return url.ToString();
                        }
                        else
                        {
                            return UriUtility.Combine(this.BasePath, CremaString.Repository, CremaString.Users, text);
                        }
                    }
                }
                return Path.Combine(this.BasePath, CremaString.Repository, CremaString.Users);
            }
        }

        public LogVerbose Verbose
        {
            get;
            set;
        }

        public bool NoCache
        {
            get;
            set;
        }

        public string[] DataBaseList
        {
            get => this.dataBaseList ?? new string[] { };
            set => this.dataBaseList = value;
        }

#if DEBUG
        public bool ValidationMode
        {
            get;
            set;
        }
#endif
    }
}
