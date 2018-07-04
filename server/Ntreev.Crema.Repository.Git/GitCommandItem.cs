using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    class GitCommandItem : CommandOption
    {
        public GitCommandItem(string name)
            : base(name)
        {

        }

        public GitCommandItem(char name)
            : base(name)
        {

        }

        public GitCommandItem(string name, object value)
            : base(name, value)
        {

        }

        public GitCommandItem(char name, object value)
            : base(name, value)
        {

        }

        public static GitCommandItem FromMessage(string message)
        {
            return new GitCommandItem('m', (GitString)message);
        }

        //public static GitCommandItem FromUsername(string username)
        //{
        //    return new GitCommandItem("username", username);
        //}

        //public static GitCommandItem FromEncoding(Encoding encoding)
        //{
        //    return new GitCommandItem("encoding", encoding.ToString());
        //}

        //public static GitCommandItem FromRevision(string revision)
        //{
        //    return new GitCommandItem('r', revision);
        //}

        //public readonly static GitCommandItem Force = new GitCommandItem("force");

        //public readonly static GitCommandItem Recursive = new GitCommandItem("recursive");

        //public readonly static GitCommandItem Quiet = new GitCommandItem("quiet");

        //public readonly static GitCommandItem Xml = new GitCommandItem("xml");

        //public readonly static GitCommandItem Verbose = new GitCommandItem("verbose");
    }
}
