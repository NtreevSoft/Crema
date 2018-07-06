using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    static class GitConfig
    {
        public static void SetValue(string repositoryPath, string name, object value)
        {
            if (repositoryPath == null)
                throw new ArgumentNullException(nameof(repositoryPath));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var configCommand = new GitCommand(repositoryPath, "config")
            {
                name,
                value
            };
            configCommand.Run();
        }

        public static void UnsetValue(string repositoryPath, string name)
        {
            if (repositoryPath == null)
                throw new ArgumentNullException(nameof(repositoryPath));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (HasValue(repositoryPath, name) == false)
                return;
            var configCommand = new GitCommand(repositoryPath, "config")
            {
                new GitCommandItem("unset"),
                name,
            };
            configCommand.Run();
        }

        public static bool HasValue(string repositoryPath, string name)
        {
            if (repositoryPath == null)
                throw new ArgumentNullException(nameof(repositoryPath));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var configCommand = new GitCommand(repositoryPath, "config")
            {
                new GitCommandItem("get"),
                name,
            };
            configCommand.ThrowOnError = false;
            return configCommand.ReadLine() != null;
        }

        public static object GetValue(string repositoryPath, string name)
        {
            if (repositoryPath == null)
                throw new ArgumentNullException(nameof(repositoryPath));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var configCommand = new GitCommand(repositoryPath, "config")
            {
                new GitCommandItem("get"),
                name,
            };
            return configCommand.Run();
        }

        public static void SetValue(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var configCommand = new GitCommand(null, "config")
            {
                GitCommandItem.Global,
                name,
                value
            };
            configCommand.Run();
        }

        public static void UnsetValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (HasValue(name) == false)
                return;
            var configCommand = new GitCommand(null, "config")
            {
                GitCommandItem.Global,
                new GitCommandItem("unset"),
                name,
            };
            configCommand.Run();
        }

        public static bool HasValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var configCommand = new GitCommand(null, "config")
            {
                GitCommandItem.Global,
                new GitCommandItem("get"),
                name,
            };
            configCommand.ThrowOnError = false;
            return configCommand.ReadLine() != null;
        }

        public static object GetValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var configCommand = new GitCommand(null, "config")
            {
                GitCommandItem.Global,
                new GitCommandItem("get"),
                name,
            };
            return configCommand.Run();
        }
    }
}
