using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    class GitString
    {
        private string text;

        public GitString(string text)
        {
            this.text = text;
        }

        public override string ToString()
        {
            return $"\"{this.text}\"";
        }

        public static explicit operator GitString(string path)
        {
            return new GitString(path);
        }
    }
}
