using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.Highlighting;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Xshtd.interfaces;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Interfaces
{
    /// <summary>
	/// Defines a Highlighting Manager that associates syntax highlighting definitions with file extentions
	/// (*.cs -> 'C#') with consideration of the current WPF App theme
	/// 
	/// Extension  App Theme   SyntaxHighlighter
	/// (*.cs  +   'Dark')  -> 'C#' (with color definitions for 'Dark')
	/// </summary>
	public interface IThemedHighlightingManager : IHighlightingDefinitionReferenceResolver,
												  IHighlightingThemeDefinitionReferenceResolver
	{
		#region properties
		/// <summary>
		/// Gets the current highlighting theme (eg 'Light' or 'Dark') that should be used as
		/// a base for the syntax highlighting in AvalonEdit.
		/// </summary>
		IHLTheme CurrentTheme { get; }
		#endregion properties

		#region methods
		/// <summary>
		/// Gets a copy of all highlightings.
		/// </summary>
		ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions { get; }

        IDictionary<string, IHLTheme> Themes { get; }

		/// <summary>
		/// Gets a highlighting definition by extension.
		/// Returns null if the definition is not found.
		/// </summary>
		IHighlightingDefinition GetDefinitionByExtension(string extension);

		/// <summary>
		/// Registers a highlighting definition for the <see cref="CurrentTheme"/>.
		/// </summary>
		/// <param name="name">The name to register the definition with.</param>
		/// <param name="extensions">The file extensions to register the definition for.</param>
		/// <param name="highlighting">The highlighting definition.</param>
		void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting);

		/// <summary>
		/// Registers a highlighting definition.
		/// </summary>
		/// <param name="name">The name to register the definition with.</param>
		/// <param name="extensions">The file extensions to register the definition for.</param>
		/// <param name="lazyLoadedHighlighting">A function that loads the highlighting definition.</param>
		void RegisterHighlighting(string name, string[] extensions, Func<IHighlightingDefinition> lazyLoadedHighlighting);

		/// <summary>
		/// Resets the highlighting theme based on the name of the WPF App Theme
		/// (eg: WPF APP Theme 'Dark' -> Resolve highlighting 'C#' to 'Dark'->'C#')
		/// 
		/// Throws an <see cref="IndexOutOfRangeException"/> if the WPF APP theme is not known.
		/// </summary>
		/// <param name="name"></param>
		void SetCurrentTheme(string name);
		#endregion methods
	}
}
