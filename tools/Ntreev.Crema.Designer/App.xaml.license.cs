using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Designer
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App
    {
        static void RegisterLicense()
        {
            Xceed.Wpf.DataGrid.Licenser.LicenseKey = "DGP579UKDUB7A1J3B5A";
            Xceed.Wpf.Toolkit.Licenser.LicenseKey = "WTK34-D145B-UKKTY-0UUA";
        }
    }
}
