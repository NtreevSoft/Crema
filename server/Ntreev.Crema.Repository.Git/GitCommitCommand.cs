using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    class GitCommitCommand : GitCommand
    {
        private readonly GitAuthor author;
        private readonly string basePath;

        public GitCommitCommand(string basePath, string author, string message)
            : base(basePath, "commit")
        {
            this.author = new GitAuthor(author);
            this.basePath = basePath;
            this.Add(new GitCommandItem('a'));
            this.Add(GitCommandItem.FromMessage(message));
        }

        protected override void OnRun()
        {
            GitConfig.SetValue(this.basePath, "user.email", this.author.Email == string.Empty ? "<>" : this.author.Email);
            GitConfig.SetValue(this.basePath, "user.name", this.author.Name);
            base.OnRun();
        }
    }
}
