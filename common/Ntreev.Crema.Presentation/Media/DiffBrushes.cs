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

using Ntreev.Crema.Presentation.Controls;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ntreev.Crema.Presentation.Media
{
    public static class DiffBrushes
    {
        static DiffBrushes()
        {
            Refresh();
        }

        public static Brush UnchangedBackground
        {
            get; set;
        }

        public static Brush DeletedBackground
        {
            get; set;
        }

        public static Brush InsertedBackground
        {
            get; set;
        }

        public static Brush ImaginaryBackground
        {
            get; set;
        }

        public static Brush ModifiedBackground
        {
            get; set;
        }

        public static Brush UnchangedForeground
        {
            get; set;
        }

        public static Brush DeletedForeground
        {
            get; set;
        }

        public static Brush InsertedForeground
        {
            get; set;
        }

        public static Brush ImaginaryForeground
        {
            get; set;
        }

        public static Brush ModifiedForeground
        {
            get; set;
        }

        public static Brush GetBackgroundBrush(DiffState diffState)
        {
            if (diffState == DiffState.Modified)
                return DiffBrushes.ModifiedBackground;
            else if (diffState == DiffState.Deleted)
                return DiffBrushes.DeletedBackground;
            else if (diffState == DiffState.Inserted)
                return DiffBrushes.InsertedBackground;
            else if (diffState == DiffState.Imaginary)
                return DiffBrushes.ImaginaryBackground;
            return DiffBrushes.UnchangedBackground;
        }

        public static Brush GetForegroundBrush(DiffState diffState)
        {
            if (diffState == DiffState.Modified)
                return DiffBrushes.ModifiedForeground;
            else if (diffState == DiffState.Deleted)
                return DiffBrushes.DeletedForeground;
            else if (diffState == DiffState.Inserted)
                return DiffBrushes.InsertedForeground;
            else if (diffState == DiffState.Imaginary)
                return DiffBrushes.ImaginaryForeground;
            return DiffBrushes.UnchangedForeground;
        }

        internal static void Refresh()
        {
            UnchangedBackground = null;
            DeletedBackground = (Application.Current.Resources["DeletedBackground"] as Brush) ?? new SolidColorBrush(Color.FromRgb(235, 204, 204));
            InsertedBackground = (Application.Current.Resources["InsertedBackground"] as Brush) ?? new SolidColorBrush(Color.FromRgb(204, 230, 196));
            ImaginaryBackground = (Application.Current.Resources["ImaginaryBackground"] as Brush) ?? new SolidColorBrush(Colors.Gray) { Opacity = 0.5f, };
            ModifiedBackground = (Application.Current.Resources["ModifiedBackground"] as Brush) ?? new SolidColorBrush(Color.FromRgb(176, 196, 221));

            UnchangedForeground = null;
            DeletedForeground = (Application.Current.Resources["DeletedForeground"] as Brush) ?? new SolidColorBrush(Color.FromRgb(235, 204, 204));
            InsertedForeground = (Application.Current.Resources["InsertedForeground"] as Brush) ?? new SolidColorBrush(Color.FromRgb(204, 230, 196));
            ImaginaryForeground = (Application.Current.Resources["ImaginaryForeground"] as Brush) ?? new SolidColorBrush(Colors.Gray) { Opacity = 0.5f, };
            ModifiedForeground = (Application.Current.Resources["ModifiedForeground"] as Brush) ?? new SolidColorBrush(Color.FromRgb(176, 196, 221));
        }
    }
}
