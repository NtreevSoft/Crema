using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    class GitPath
    {
        private string path;

        /// <summary>
        /// "C:\test\" 와 같은 문자열을 Process의 인수로 넘겨질때 마지막의 \"가 escape가 되어서 "C:\test\ 처럼 넘겨진다.
        /// 결과적으로 잘못된 문자열로 인해 에러가 발생함.
        /// </summary>
        public GitPath(string path)
        {
            this.path = path;
            if (this.path.EndsWith($"{Path.DirectorySeparatorChar}") == true)
                this.path = this.path.TrimEnd(Path.DirectorySeparatorChar);
            if (this.path.EndsWith($"{Path.AltDirectorySeparatorChar}") == true)
                this.path = this.path.TrimEnd(Path.AltDirectorySeparatorChar);
        }

        public GitPath(Uri uri)
            : this(uri.ToString())
        {

        }

        public override string ToString()
        {
            return $"\"{this.path}\"";
        }

        public static explicit operator GitPath(string path)
        {
            return new GitPath(path);
        }

        public static explicit operator GitPath(Uri uri)
        {
            return new GitPath(uri);
        }
    }
}
