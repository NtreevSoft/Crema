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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Interfaces;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Manager;
using Ntreev.Crema.Client.Framework.Dialogs.Views.CodeEditorThemes;
using Ntreev.Crema.Client.Framework.Properties;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Xceed.Wpf.AvalonDock.Controls;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CodeEditorViewModel : ModalDialogAppBase
    {
        private const string LightThemeName = "Light";
        private const string DarkThemeName = "Dark";
        private const string DefaultLanguageName = "C#";

        private readonly IAppConfiguration config;
        private readonly Action<string> saveAction;
        private readonly string subTitle;
        private readonly IThemedHighlightingManager manager;
        private IHighlightingDefinition highlightingDefinition;
        private string selectedTheme;
        private bool isReadOnly;
        private TextEditor textEditor;
        private bool isDirty;
        
        public ICommand SaveCommand { get; }
        public TextDocument Document { get; } = new TextDocument();

        public CodeEditorViewModel(IAppConfiguration config, Action<string> saveAction, string subTitle = "")
        {
            this.config = config;
            this.saveAction = saveAction;
            this.subTitle = subTitle;
            this.manager = DefaultHighlightingManager.Instance;
            this.SetDisplayName();
            this.SaveCommand = new RelayCommand((_) => this.Save(), (_ => this.isDirty));
        }

        public bool IsDirty
        {
            get => isDirty;
            set
            {
                if (value == isDirty) return;
                isDirty = value;
                NotifyOfPropertyChange(() => IsDirty);
                this.SetDisplayName();
            }
        }

        public string Text
        {
            get => this.Document.Text;
            set
            {
                this.Document.Text = value;
                this.IsDirty = false;
            }
        }

        public bool IsReadOnly
        {
            get => isReadOnly;
            set
            {
                if (value == isReadOnly) return;
                isReadOnly = value;
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }

        [ConfigurationProperty]
        public string SelectedLanguage { get; set; }

        [ConfigurationProperty]
        public string SelectedTheme
        {
            get => selectedTheme;
            set
            {
                if (value == selectedTheme) return;
                selectedTheme = value;
                NotifyOfPropertyChange(() => SelectedTheme);
            }
        }

        [ConfigurationProperty]
        public bool IsShowLineNumbers
        {
            get => this.textEditor?.ShowLineNumbers ?? true;
            set
            {
                if (this.textEditor?.ShowLineNumbers == null) return;
                this.textEditor.ShowLineNumbers = value;
            }
        }

        [ConfigurationProperty]
        public bool IsWordWrap
        {
            get => this.textEditor?.WordWrap ?? false;
            set
            {
                if (this.textEditor?.WordWrap == null) return;
                this.textEditor.WordWrap = value;
            }
        }

        [ConfigurationProperty] 
        public bool IsShowEndOfLine
        {
            get => this.textEditor?.Options?.ShowEndOfLine ?? false;
            set
            {
                if (this.textEditor?.Options?.ShowEndOfLine == null) return;
                this.textEditor.Options.ShowEndOfLine = value;
            }
        }

        [ConfigurationProperty]
        public bool IsShowSpaces
        {
            get => this.textEditor?.Options?.ShowSpaces ?? false;
            set
            {
                if (this.textEditor?.Options?.ShowSpaces == null) return;
                this.textEditor.Options.ShowSpaces = value;
            }
        }

        [ConfigurationProperty]
        public bool IsShowTabs
        {
            get => this.textEditor?.Options.ShowTabs ?? false;
            set
            {
                if (this.textEditor?.Options?.ShowTabs == null) return;
                this.textEditor.Options.ShowTabs = value;
            }
        }

        [ConfigurationProperty]
        public bool IsConvertTabsToSpaces
        {
            get => this.textEditor?.Options?.ConvertTabsToSpaces ?? false;
            set
            {
                if (this.textEditor?.Options?.ConvertTabsToSpaces == null) return;
                this.textEditor.Options.ConvertTabsToSpaces = value;
            }
        }

        [ConfigurationProperty]
        public bool IsHighlightCurrentLine
        {
            get => this.textEditor?.Options?.HighlightCurrentLine ?? false;
            set
            {
                if (this.textEditor?.Options?.HighlightCurrentLine == null) return;
                this.textEditor.Options.HighlightCurrentLine = value;
            }
        }

        public IEnumerable<string> ThemeNames => this.manager.Themes.Keys;

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            
            this.textEditor = (this.GetView() as Window)?.FindVisualChildren<TextEditor>().FirstOrDefault();
            LoadConfig();
            this.SetTheme(this.SelectedTheme);
        }

        private void SetDisplayName()
        {
            var dirtyString = this.IsDirty ? "*" : "";
            var titleString = string.IsNullOrWhiteSpace(this.subTitle) ? "" : $"- {this.subTitle}";
            this.DisplayName = $"{Resources.Title_CodeEditor} {titleString} {dirtyString}";
        }

        public void Save()
        {
            this.saveAction?.Invoke(this.textEditor.Text);
            this.IsDirty = false;
        }

        public void Exit()
        {
            this.TryClose();
        }

        public void ChangeThemeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is ComboBox comboBox)
            {
                var themeName = comboBox.SelectedItem.ToString();

                SetTheme(themeName);
                SaveConfig();
            }
        }
        private void LoadConfig()
        {
            this.config.Update(this);

            this.HighlightingDefinition = manager.GetDefinition(this.SelectedLanguage) ??
                                          manager.HighlightingDefinitions.FirstOrDefault();
            this.SelectedTheme = this.SelectedTheme ?? DarkThemeName;
            this.SelectedLanguage = this.HighlightingDefinition?.Name ?? DefaultLanguageName;
        }

        private void SaveConfig()
        {
            this.SelectedLanguage = this.HighlightingDefinition.Name;
            this.SelectedTheme = manager.CurrentTheme.DisplayName;
            this.config.Commit(this);
        }

        public void TextEditor_TextChanged(object sender, EventArgs e)
        {
            this.IsDirty = true;
        }

        public override void CanClose(Action<bool> callback)
        {
            this.SaveConfig();

            bool? result = null;
            if (this.IsDirty)
            {
                result = AppMessageBox.ConfirmSaveOnClosing();
                if (result == null) return;
            }

            this.DialogResult = result;
            base.CanClose(callback);
        }

        private void SetTheme(string themeName)
        {
            switch (themeName)
            {
                case LightThemeName: break;
                default:
                    themeName = DarkThemeName;
                    break;
            }

            var resourceDic = new ResourceDictionary()
            {
                Source = new Uri(
                    $"/Ntreev.Crema.Client.Framework;component/Dialogs/Views/CodeEditorThemes/{themeName}Brushs.xaml",
                    UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Add(resourceDic);
            SetAccentColor(SystemParameters.WindowGlassColor);
            manager.SetCurrentTheme(themeName);
            OnAppThemeChanged();
        }

        public IHighlightingDefinition HighlightingDefinition
        {
            get => highlightingDefinition;

            set
            {
                if (highlightingDefinition == value) return;

                highlightingDefinition = value;
                NotifyOfPropertyChange(() => HighlightingDefinition);
            }
        }

        public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions => manager?.HighlightingDefinitions;

        public void SetAccentColor(Color accentColor)
        {
            try
            {
                var bColorChanged = Application.Current.Resources[ResourceKeys.ControlAccentColorKey] == null;

                if (Application.Current.Resources[ResourceKeys.ControlAccentColorKey] != null)
                {
                    bColorChanged = bColorChanged || (Color)Application.Current.Resources[ResourceKeys.ControlAccentColorKey] != accentColor;
                }

                if (bColorChanged)
                {
                    Application.Current.Resources[ResourceKeys.ControlAccentColorKey] = accentColor;
                    Application.Current.Resources[ResourceKeys.ControlAccentBrushKey] = new SolidColorBrush(accentColor);
                }
            }
            catch { }
        }

        internal void OnAppThemeChanged()
        {
            if (manager == null) return;

            if (manager.CurrentTheme.HlTheme != null)
            {
                foreach (var item in manager.CurrentTheme.HlTheme.GlobalStyles)
                {
                    switch (item.TypeName)
                    {
                        case "DefaultStyle":
                            ApplyToDynamicResource(ResourceKeys.EditorBackground, item.backgroundcolor);
                            ApplyToDynamicResource(ResourceKeys.EditorForeground, item.foregroundcolor);
                            break;

                        case "CurrentLineBackground":
                            ApplyToDynamicResource(ResourceKeys.EditorCurrentLineBackgroundBrushKey, item.backgroundcolor);
                            ApplyToDynamicResource(ResourceKeys.EditorCurrentLineBorderBrushKey, item.bordercolor);
                            break;

                        case "LineNumbersForeground":
                            ApplyToDynamicResource(ResourceKeys.EditorLineNumbersForeground, item.foregroundcolor);
                            break;

                        case "Selection":
                            ApplyToDynamicResource(ResourceKeys.EditorSelectionBrush, item.backgroundcolor);
                            ApplyToDynamicResource(ResourceKeys.EditorSelectionBorder, item.bordercolor);
                            break;

                        case "Hyperlink":
                            ApplyToDynamicResource(ResourceKeys.EditorLinkTextBackgroundBrush, item.backgroundcolor);
                            ApplyToDynamicResource(ResourceKeys.EditorLinkTextForegroundBrush, item.foregroundcolor);
                            break;

                        case "NonPrintableCharacter":
                            ApplyToDynamicResource(ResourceKeys.EditorNonPrintableCharacterBrush, item.foregroundcolor);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("GlobalStyle named '{0}' is not supported.", item.TypeName);
                    }
                }
            }

            if (HighlightingDefinition != null)
            {
                HighlightingDefinition = manager.GetDefinition(HighlightingDefinition.Name);
                NotifyOfPropertyChange(() => this.HighlightingDefinitions);
                
                if (HighlightingDefinition != null)
                    return;
            }
        }

        private void ApplyToDynamicResource(ComponentResourceKey key, Color? newColor)
        {
            if (Application.Current.Resources[key] == null || newColor == null)
                return;

            if (Application.Current.Resources[key] is SolidColorBrush)
            {
                var newColorBrush = new SolidColorBrush((Color)newColor);
                newColorBrush.Freeze();

                Application.Current.Resources[key] = newColorBrush;
            }
        }
    }
}
