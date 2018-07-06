using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    class GitAuthor
    {
        private const string authorPattern = "(?<name>.+)\\s<(?<email>.*)>";
        private readonly string name;
        private readonly string email;

        public GitAuthor(string author)
        {
            if (author == null)
                throw new ArgumentNullException(nameof(author));
            var match = Regex.Match(author, authorPattern, RegexOptions.ExplicitCapture);
            if (match.Success == true)
            {
                this.name = match.Groups["name"].Value;
                this.email = match.Groups["email"].Value;
            }
            else
            {
                this.name = author;
            }
        }

        public string Name { get => this.name; }

        public string Email { get => this.email ?? string.Empty; }

        public override string ToString()
        {
            return $"{this.Name} <{this.Email}>";
        }

        public static explicit operator GitAuthor(string author)
        {
            return new GitAuthor(author);
        }
    }
}
