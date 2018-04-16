using System;

namespace Ntreev.Crema.ApplicationHost
{
    public interface IShell 
    {
        string BasePath { get; set; }

        int Port { get; set; }

        bool IsOpened { get; }

        event EventHandler Opened;

        event EventHandler Closed;
    }
}