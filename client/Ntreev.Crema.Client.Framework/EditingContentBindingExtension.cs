using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Xceed.Wpf.DataGrid;

namespace Ntreev.Crema.Client.Framework.Markup
{
    [MarkupExtensionReturnType(typeof(object))]
    public class EditingContentBindingExtension : MarkupExtension
    {
        private readonly Binding binding = new Binding();

        public EditingContentBindingExtension()
        {
            this.binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) { AncestorType = typeof(Cell), };
            this.binding.Mode = BindingMode.TwoWay;
            this.binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.binding.Path = new PropertyPath("EditingContent");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.binding.ProvideValue(serviceProvider);
        }

        public object TargetNullValue
        {
            get { return this.binding.TargetNullValue; }
            set { this.binding.TargetNullValue = value; }
        }
    }
}
