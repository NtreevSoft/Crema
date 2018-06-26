using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    [Export(typeof(IRepositoryUpgrader))]
    class SvnRepositoryUpgrader : IRepositoryUpgrader
    {
        private readonly SvnRepositoryProvider repositoryProvider;

        [ImportingConstructor]
        public SvnRepositoryUpgrader(SvnRepositoryProvider repositoryProvider)
        {
            this.repositoryProvider = repositoryProvider;
        }

        public IRepositoryProvider RepositoryProvider => this.repositoryProvider;

        public string Name => this.repositoryProvider.Name;

        public string Upgrade(string sourcePath)
        {
            return null;
        }
    }
}
