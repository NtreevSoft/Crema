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

using Ntreev.Crema.Presentation.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ntreev.Crema.Presentation.Controls
{
    public static class DiffCommands
    {
        public static DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(DiffCommands));

        public static DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(DiffCommands));

        public static ICommand GetCommand(FrameworkElement fe)
        {
            return (ICommand)fe.GetValue(CommandProperty);
        }

        public static void SetCommand(FrameworkElement fe, ICommand value)
        {
            fe.SetValue(CommandProperty, value);
        }

        public static object GetCommandParameter(FrameworkElement fe)
        {
            return (object)fe.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(FrameworkElement fe, object value)
        {
            fe.SetValue(CommandParameterProperty, value);
        }


        public static readonly RoutedCommand PrevDifferenceItem = new RoutedUICommand(Resources.Command_PrevDifferenceItem, nameof(PrevDifferenceItem), typeof(DiffCommands));

        public static readonly RoutedCommand NextDifferenceItem = new RoutedUICommand(Resources.Command_NextDifferenceItem, nameof(NextDifferenceItem), typeof(DiffCommands));

        public static readonly RoutedCommand PrevDifferenceField = new RoutedUICommand(Resources.Command_PrevDifferenceField, nameof(PrevDifferenceField), typeof(DiffCommands));

        public static readonly RoutedCommand NextDifferenceField = new RoutedUICommand(Resources.Command_NextDifferenceField, nameof(NextDifferenceField), typeof(DiffCommands));

        public static readonly RoutedCommand Resolve = new RoutedUICommand(Resources.Command_Resolve, nameof(Resolve), typeof(DiffCommands));

        public static readonly RoutedCommand Merge = new RoutedUICommand(Resources.Command_Merge, nameof(Merge), typeof(DiffCommands));

        public static readonly RoutedCommand IncludeDateTime = new RoutedUICommand("날짜 비교 포함", nameof(IncludeDateTime), typeof(DiffCommands));

        public static readonly RoutedCommand AddItemToRightSide = new RoutedUICommand(Resources.Command_AddItemToRightSide, nameof(AddItemToRightSide), typeof(DiffCommands));

        public static readonly RoutedCommand AddItemToLeftSide = new RoutedUICommand(Resources.Command_AddItemToLeftSide, nameof(AddItemToLeftSide), typeof(DiffCommands));

        public static readonly RoutedCommand CopyItemToRightSide = new RoutedUICommand(Resources.Command_CopyItemToRightSide, nameof(CopyItemToRightSide), typeof(DiffCommands));

        public static readonly RoutedCommand CopyItemToLeftSide = new RoutedUICommand(Resources.Command_CopyItemToLeftSide, nameof(CopyItemToLeftSide), typeof(DiffCommands));

        public static readonly RoutedCommand CopyFieldToRightSide = new RoutedUICommand(Resources.Command_CopyFieldToRightSide, nameof(CopyFieldToRightSide), typeof(DiffCommands));

        public static readonly RoutedCommand CopyFieldToLeftSide = new RoutedUICommand(Resources.Command_CopyFieldToLeftSide, nameof(CopyFieldToLeftSide), typeof(DiffCommands));

        public static readonly RoutedCommand DeleteItemOfRightSide = new RoutedUICommand(Resources.Command_Delete, nameof(DeleteItemOfRightSide), typeof(DiffCommands));

        public static readonly RoutedCommand DeleteItemOfLeftSide = new RoutedUICommand(Resources.Command_Delete, nameof(DeleteItemOfLeftSide), typeof(DiffCommands));

        public static readonly RoutedCommand CopyPropertyToRightSide = new RoutedCommand(nameof(CopyPropertyToRightSide), typeof(DiffCommands));

        public static readonly RoutedCommand CopyPropertyToLeftSide = new RoutedCommand(nameof(CopyPropertyToLeftSide), typeof(DiffCommands));


        public static readonly RoutedCommand ExportLeft = new RoutedCommand(nameof(ExportLeft), typeof(DiffCommands));

        public static readonly RoutedCommand ExportRight = new RoutedCommand(nameof(ExportRight), typeof(DiffCommands));

        static DiffCommands()
        {

        }
    }
}
