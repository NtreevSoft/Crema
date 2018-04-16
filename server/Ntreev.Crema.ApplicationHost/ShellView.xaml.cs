using FirstFloor.ModernUI.Windows.Controls;
using Ntreev.ModernUI.Framework.Controls;
using Ntreev.ModernUI.Framework.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ntreev.Crema.ApplicationHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView : SimpleWindow
    {
        public ShellView()
        {
            this.InitializeComponent();
            this.ContentLoader = new ModernContentLoader();
        }

        private void Filename_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}