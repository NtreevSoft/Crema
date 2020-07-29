using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.Highlighting;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.HighlightingTheme;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Interfaces;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Resources;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Xshtd.interfaces;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Manager
{
    /// <summary>
	/// Implements a Highlighting Manager that associates syntax highlighting definitions with file extentions
	/// (*.cs -> 'C#') with consideration of the current WPF App theme
	/// 
	/// Extension  App Theme   SyntaxHighlighter
	/// (*.cs  +   'Dark')  -> 'C#' (with color definitions for 'Dark')
	/// </summary>
	public class ThemedHighlightingManager : IThemedHighlightingManager
	{
		/// <summary>
		/// Defines the root namespace under which the built-in xshd highlighting
		/// resource files can be found
		/// (eg all highlighting for 'Light' should be located here).
		/// </summary>
		public const string HL_GENERIC_NAMESPACE_ROOT = "HL.Resources.Light";

		/// <summary>
		/// Defines the root namespace under which the built-in additional xshtd highlighting theme
		/// resource files can be found
		/// (eg 'Dark' and 'TrueBlue' themes should be located here).
		/// </summary>
		public const string HL_THEMES_NAMESPACE_ROOT = "HL.Resources.Themes";

		private readonly object lockObj = new object();
		private readonly Dictionary<string, IHLTheme> themedHighlightings;

		/// Class constructor
		/// </summary>
		public ThemedHighlightingManager()
		{
			themedHighlightings = new Dictionary<string, IHLTheme>();
		}

		/// <summary>
		/// Gets the current highlighting theme (eg 'Light' or 'Dark') that should be used as
		/// a base for the syntax highlighting in AvalonEdit.
		/// </summary>
		public IHLTheme CurrentTheme { get; private set; }

        public IDictionary<string, IHLTheme> Themes => this.themedHighlightings;

		/// <summary>
		/// Gets the default HighlightingManager instance.
		/// The default HighlightingManager comes with built-in highlightings.
		/// </summary>
		public static IThemedHighlightingManager Instance => DefaultHighlightingManager.Instance;

        /// <summary>
		/// Gets the highlighting definition by name, or null if it is not found.
		/// </summary>
		IHighlightingDefinition IHighlightingDefinitionReferenceResolver.GetDefinition(string name)
        {
            if (name == null) return null;

			lock (lockObj)
			{
                return CurrentTheme?.GetDefinition(name);
            }
		}

		/// <summary>
		/// Gets an (ordered by Name) list copy of all highlightings defined in this object
		/// or an empty collection if there is no highlighting definition available.
		/// </summary>
		public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
		{
			get
			{
				lock (lockObj)
				{
					if (CurrentTheme != null)
						return CurrentTheme.HighlightingDefinitions;

					return new ReadOnlyCollection<IHighlightingDefinition>(new List<IHighlightingDefinition>());
				}
			}
		}

		/// <summary>
		/// Gets a highlighting definition by extension.
		/// Returns null if the definition is not found.
		/// </summary>
		public IHighlightingDefinition GetDefinitionByExtension(string extension)
		{
			lock (lockObj)
            {
                return themedHighlightings.TryGetValue(CurrentTheme.Key, out var theme) ? theme.GetDefinitionByExtension(extension) : null;
            }
		}

		/// <summary>
		/// Registers a highlighting definition for the <see cref="CurrentTheme"/>.
		/// </summary>
		/// <param name="name">The name to register the definition with.</param>
		/// <param name="extensions">The file extensions to register the definition for.</param>
		/// <param name="highlighting">The highlighting definition.</param>
		public void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting)
		{
			if (highlighting == null)
				throw new ArgumentNullException("highlighting");

			lock (lockObj)
            {
                CurrentTheme?.RegisterHighlighting(name, extensions, highlighting);
            }
		}

		/// <summary>
		/// Registers a highlighting definition.
		/// </summary>
		/// <param name="name">The name to register the definition with.</param>
		/// <param name="extensions">The file extensions to register the definition for.</param>
		/// <param name="lazyLoadedHighlighting">A function that loads the highlighting definition.</param>
		public void RegisterHighlighting(string name, string[] extensions, Func<IHighlightingDefinition> lazyLoadedHighlighting)
		{
			if (lazyLoadedHighlighting == null)
				throw new ArgumentNullException("lazyLoadedHighlighting");

			RegisterHighlighting(name, extensions, new DelayLoadedHighlightingDefinition(name, lazyLoadedHighlighting));
		}

		/// <summary>
		/// Sets the current highlighting based on the name of the given high?ghting theme.
		/// (eg: WPF APP Theme 'TrueBlue' -> Resolve highlighting 'C#' to 'TrueBlue'->'C#')
		/// 
		/// Throws an <see cref="IndexOutOfRangeException"/> if the WPF APP theme is not known.
		/// </summary>
		/// <param name="themeNameKey"></param>
		public void SetCurrentTheme(string themeNameKey)
		{
			SetCurrentThemeInternal(themeNameKey);
			HLResources.RegisterBuiltInHighlightings(DefaultHighlightingManager.Instance, CurrentTheme);
		}

		/// <summary>
		/// Adds another highlighting theme into the current collection of highlighting themes.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="theme"></param>
		public void ThemedHighlightingAdd(string key, IHLTheme theme)
		{
			lock (lockObj)
			{
				themedHighlightings.Add(key, theme);
			}
		}

		/// <summary>
		/// Removes a highlighting theme from the current collection
		/// of highlighting themes.
		/// </summary>
		/// <param name="removekey"></param>
		public void ThemedHighlightingRemove(string removekey)
		{
			lock (lockObj)
			{
				themedHighlightings.Remove(removekey);
			}
		}

		/// <summary>
		/// Initializes the current default theme available at start-up of application
		/// (without registration of highlightings).
		/// </summary>
		/// <param name="themeNameKey"></param>
		protected void SetCurrentThemeInternal(string themeNameKey)
		{
			CurrentTheme = themedHighlightings[themeNameKey];
		}

		/// <summary>
		/// Helper method to find the correct namespace of an internal xshd resource
		/// based on the name of a (WPF) theme (eg. 'TrueBlue') and an internal
		/// constant (eg. 'HL.Resources')
		/// </summary>
		/// <param name="themeNameKey"></param>
		/// <returns></returns>
		protected virtual string GetPrefix(string themeNameKey)
		{
			lock (lockObj)
			{
                if (themedHighlightings.TryGetValue(themeNameKey, out var theme))
				{
					return theme.HLBasePrefix;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the highlighting theme definition by name, or null if there is none to be found.
		/// </summary>
		/// <param name="highlightingName"></param>
		SyntaxDefinition IHighlightingThemeDefinitionReferenceResolver.GetThemeDefinition(string highlightingName)
		{
			lock (lockObj)
			{
                return CurrentTheme?.GetThemeDefinition(highlightingName);
            }
		}

		/// <summary>
		/// Gets the highlighting theme definition by name of the theme (eg 'Dark2' or 'TrueBlue')
		/// and the highlighting, or null if there is none to be found.
		/// </summary>
		/// <param name="hlThemeName"></param>
		/// <param name="highlightingName"></param>
		SyntaxDefinition IHighlightingThemeDefinitionReferenceResolver.GetThemeDefinition(string hlThemeName,
																						  string highlightingName)
		{
			lock (lockObj)
			{
                this.themedHighlightings.TryGetValue(hlThemeName, out var highlighting);

				return highlighting?.GetThemeDefinition(hlThemeName);
            }
		}
	}
}
