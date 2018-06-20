using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    static class GitPathUtility
    {
        /// <summary>
        /// "C:\test\" 와 같은 문자열을 Process의 인수로 넘겨질때 마지막의 \"가 escape가 되어서 "C:\test\ 처럼 넘겨진다.
        /// 결과적으로 잘못된 문자열로 인해 에러가 발생함.
        /// </summary>
        public static string ToGitPath(this string path)
        {
            if (path.EndsWith($"{Path.DirectorySeparatorChar}") == true)
                path = path.TrimEnd(Path.DirectorySeparatorChar);
            if (path.EndsWith($"{Path.AltDirectorySeparatorChar}") == true)
                path = path.TrimEnd(Path.AltDirectorySeparatorChar);
            return path.WrapQuot();
        }
    }
}
