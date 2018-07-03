using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    class SvnPath
    {
        private string path;

        public SvnPath(string path)
        {
            this.path = path;
            if (this.path.EndsWith($"{Path.DirectorySeparatorChar}") == true)
                this.path = this.path.TrimEnd(Path.DirectorySeparatorChar);
            if (this.path.EndsWith($"{Path.AltDirectorySeparatorChar}") == true)
                this.path = this.path.TrimEnd(Path.AltDirectorySeparatorChar);
        }

        public SvnPath(Uri uri)
            : this(uri.ToString())
        {

        }

        public override string ToString()
        {
            return $"\"{this.path}\"";
        }

        public static explicit operator SvnPath(string path)
        {
            return new SvnPath(path);
        }

        public static explicit operator SvnPath(Uri uri)
        {
            return new SvnPath(uri);
        }
    }
}
