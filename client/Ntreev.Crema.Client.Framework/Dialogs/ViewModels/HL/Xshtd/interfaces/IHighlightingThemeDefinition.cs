using System.Collections.Generic;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.HighlightingTheme;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Xshtd.interfaces
{
    /// <summary>
	/// A highlighting definition.
	/// </summary>
	public interface IHighlightingThemeDefinition
	{
		/// <summary>
		/// Gets the name of the highlighting theme definition.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a named highlighting color.
		/// </summary>
		/// <returns>The highlighting color, or null if it is not found.</returns>
		////HighlightingColor GetNamedColor(string name);
		SyntaxDefinition GetNamedSyntaxDefinition(string name);

		/// <summary>
		/// Gets all global stayles in the collection of global styles.
		/// </summary>
		IEnumerable<GlobalStyle> GlobalStyles { get; }
	}
}
