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

        public static GitCommandItem FromFile(string path)
        {
            return new GitCommandItem("file", (GitPath)path);
        }

        public static GitCommandItem FromAuthor(string author)
        {
            return FromAuthor((GitAuthor)author);
        }

        public static GitCommandItem FromAuthor(GitAuthor author)
        {
			return new GitCommandItem("author", (GitString)$"{author}");
        }

        public static GitCommandItem FromPretty(string format)
        {
            return new GitCommandItem($"pretty={format}");
        }

        public static GitCommandItem FromMaxCount(int count)
        {
            return new GitCommandItem($"max-count={count}");
        }

        public static readonly GitCommandItem Separator = new GitCommandItem(string.Empty);

        public static readonly GitCommandItem Global = new GitCommandItem("global");

        public static readonly GitCommandItem ShowNotes = new GitCommandItem("show-notes");

        public static readonly GitCommandItem NoPatch = new GitCommandItem("no-patch");
    }
}
