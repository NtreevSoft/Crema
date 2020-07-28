using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.HighlightingTheme;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Xshtd.interfaces
{
    /// <summary>
	/// Defines a resolver interface that can find highlighting theme definitions
	/// based on a highlighting name (searches within the current highlighting theme)
	/// or based on a highlighting name and name of highlighting theme that should
	/// contain the highlighting definition.
	/// </summary>
	public interface IHighlightingThemeDefinitionReferenceResolver
	{
		/// <summary>
		/// Gets a highlighting definition within the current highlighting theme
		/// by name, or null.
		/// </summary>
		/// <param name="highlightingName"></param>
		/// <returns></returns>
		SyntaxDefinition GetThemeDefinition(string highlightingName);

		/// <summary>
		/// Gets a highlighting theme definition by name from a given highlighting
		/// theme obtained via <paramref name="hlThemeName"/> or null.
		/// </summary>
		/// <param name="hlThemeName"></param>
		/// <param name="highlightingName"></param>
		SyntaxDefinition GetThemeDefinition(string hlThemeName,
											string highlightingName);
	}
}
