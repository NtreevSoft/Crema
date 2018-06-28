using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public interface IRepositoryMigrator
    {
        string Migrate(string sourcePath);

        IRepositoryProvider RepositoryProvider { get; }

        string Name { get; }
    }
}
