using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    static class SvnPathUtility
    {
        public static string ToSvnPath(this string path)
        {
            if (path.EndsWith($"{Path.DirectorySeparatorChar}") == true)
                path = path.TrimEnd(Path.DirectorySeparatorChar);
            if (path.EndsWith($"{Path.AltDirectorySeparatorChar}") == true)
                path = path.TrimEnd(Path.AltDirectorySeparatorChar);
            return path.WrapQuot();
        }
    }
}
