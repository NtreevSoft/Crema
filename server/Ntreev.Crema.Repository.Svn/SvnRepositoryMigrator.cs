using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    [Export(typeof(IRepositoryMigrator))]
    class SvnRepositoryMigrator : IRepositoryMigrator
    {
        private readonly SvnRepositoryProvider repositoryProvider;

        [ImportingConstructor]
        public SvnRepositoryMigrator(SvnRepositoryProvider repositoryProvider)
        {
            this.repositoryProvider = repositoryProvider;
        }

        public IRepositoryProvider RepositoryProvider => this.repositoryProvider;

        public string Name => this.repositoryProvider.Name;

        public string Migrate(string sourcePath)
        {
            return null;
        }
    }
}
