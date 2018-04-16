//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Ntreev.Crema.Presentation.Controls
{
    [TemplatePart(Name = "PART_Button", Type = typeof(ButtonBase))]
    public class FlowExpander : Expander
    {
        private double actualWidth;
        private double actualHeight;
        private double minWidth = double.NaN;
        private double minHeight = double.NaN;

        private ButtonBase button;

        public FlowExpander()
        {
            this.actualWidth = double.NaN;
            this.actualHeight = double.NaN;
            this.Loaded += FlowExpander_Loaded;
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.button = this.Template.FindName("PART_Button", this) as ButtonBase;
        }

        protected override void OnCollapsed()
        {
            base.OnCollapsed();
            this.CollapseSize();
        }

        protected override void OnExpanded()
        {
            base.OnExpanded();
            this.ExpandSize();
        }

        private void FlowExpander_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsExpanded == false)
            {
                this.CollapseSize();
            }
            else
            {
                this.ExpandSize();
            }
        }

        private void CollapseSize()
        {
            this.actualWidth = this.ActualWidth;
            this.actualHeight = this.ActualHeight;

            if (this.ExpandDirection == ExpandDirection.Left || this.ExpandDirection == ExpandDirection.Right)
            {
                this.minWidth = this.MinWidth;
                this.MinWidth = 0;
                if (this.button != null)
                    this.Width = this.button.ActualWidth;
            }
            else
            {
                this.minHeight = this.MinHeight;
                this.MinHeight = 0;
                if (this.button != null)
                    this.Height = this.button.ActualHeight;
            }
        }

        private void ExpandSize()
        {
            if (this.ExpandDirection == ExpandDirection.Left || this.ExpandDirection == ExpandDirection.Right)
            {
                if (double.IsNaN(this.minWidth) == false)
                {
                    this.MinWidth = this.minWidth;
                    this.Width = this.actualWidth;
                }
            }
            else
            {
                if (double.IsNaN(this.minHeight) == false)
                {
                    this.MinHeight = this.minHeight;
                    this.Height = this.actualHeight;
                }
            }
        }
    }
}
