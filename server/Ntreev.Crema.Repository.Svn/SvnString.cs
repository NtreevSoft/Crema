using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    class SvnString
    {
        public const string Name = "svn";
        public const string Trunk = "trunk";
        public const string Tags = "tags";
        public const string Branches = "branches";
        public const string Default = "default";

        private string text;

        public SvnString(string text)
        {
            this.text = text;
        }

        public override string ToString()
        {
            return $"\"{this.text}\"";
        }

        public static explicit operator SvnString(string path)
        {
            return new SvnString(path);
        }
    }
}
