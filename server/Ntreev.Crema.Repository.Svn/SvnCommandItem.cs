using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    class SvnCommandItem : CommandOption
    {
        public SvnCommandItem(string name)
            : base(name)
        {
            
        }

        public SvnCommandItem(char name)
            : base(name)
        {
            
        }

        public SvnCommandItem(string name, object value)
            : base(name, value)
        {
            
        }

        public SvnCommandItem(char name, object value)
            : base(name, value)
        {
            
        }

        public static SvnCommandItem FromMessage(string message)
        {
            return new SvnCommandItem('m', (SvnString)message);
        }

        public static SvnCommandItem FromFile(string path)
        {
            return new SvnCommandItem("file", (SvnPath)path);
        }

        public static SvnCommandItem FromUsername(string username)
        {
            return new SvnCommandItem("username", username);
        }

        public static SvnCommandItem FromEncoding(Encoding encoding)
        {
            return new SvnCommandItem("encoding", encoding.HeaderName);
        }

        public static SvnCommandItem FromRevision(string revision)
        {
            return new SvnCommandItem('r', revision);
        }

        public static SvnCommandItem FromMaxCount(int maxCount)
        {
            return new SvnCommandItem('l', maxCount);
        }

        public readonly static SvnCommandItem Force = new SvnCommandItem("force");

        public readonly static SvnCommandItem Recursive = new SvnCommandItem("recursive");

        public readonly static SvnCommandItem Quiet = new SvnCommandItem("quiet");

        public readonly static SvnCommandItem Xml = new SvnCommandItem("xml");

        public readonly static SvnCommandItem Verbose = new SvnCommandItem("verbose");

        public readonly static SvnCommandItem WithAllRevprops = new SvnCommandItem("with-all-revprops");
    }
}
