using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    class SvnCommandItem
    {
        private object key;
        private object value;

        public SvnCommandItem(string name)
        {
            this.key = name;
        }

        public SvnCommandItem(char name)
        {
            this.key = name;
        }

        public SvnCommandItem(string name, object value)
        {
            this.key = name;
            this.value = value;
        }

        public SvnCommandItem(char name, object value)
        {
            this.key = name;
            this.value = value;
        }

        public override string ToString()
        {
            if (this.key is string s)
            {
                if (this.value == null)
                    return $"--{this.key}";
                else
                    return $"--{this.key} {this.value}";
            }
            else if (this.key is char c)
            {
                if (this.value == null)
                    return $"-{this.key}";
                else
                    return $"-{this.key} {this.value}";
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static SvnCommandItem FromMessage(string message)
        {
            return new SvnCommandItem('m', (SvnString)message);
        }

        public static SvnCommandItem FromUsername(string username)
        {
            return new SvnCommandItem("username", username);
        }

        public static SvnCommandItem FromEncoding(Encoding encoding)
        {
            return new SvnCommandItem("encoding", encoding.ToString());
        }

        public static SvnCommandItem FromRevision(string revision)
        {
            return new SvnCommandItem('r', revision);
        }

        public readonly static SvnCommandItem Force = new SvnCommandItem("force");

        public readonly static SvnCommandItem Recursive = new SvnCommandItem("recursive");

        public readonly static SvnCommandItem Quiet = new SvnCommandItem("quiet");

        public readonly static SvnCommandItem Xml = new SvnCommandItem("xml");

        public readonly static SvnCommandItem Verbose = new SvnCommandItem("verbose");
    }
}
