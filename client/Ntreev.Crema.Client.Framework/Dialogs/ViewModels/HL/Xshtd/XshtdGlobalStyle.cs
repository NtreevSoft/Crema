using System;
using System.Windows.Media;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Xshtd
{
    /// <summary>
	/// An element contained in a &lt;GlobalStyles&gt; element.
	/// </summary>
	[Serializable]
	public class XshtdGlobalStyle : XshtdElement
	{
		private readonly XshtdGlobalStyles styles;

		/// <summary>
		/// Creates a new XshtdSyntaxDefinition object.
		/// </summary>
		/// <param name="styles">Parent collection of styles in which this style occurs.</param>
		public XshtdGlobalStyle(XshtdGlobalStyles styles)
			: this()
		{
			this.styles = styles;
		}

		/// <summary>
		/// Hidden class constructor
		/// </summary>
		protected XshtdGlobalStyle()
		{
		}

		/// <summary>
		/// Gets/sets the style definition name
		/// </summary>
		public string TypeName { get; set; }

		/// <summary>
		/// Gets/sets the style definition name
		/// </summary>
		public Color? foreground { get; set; }

		/// <summary>
		/// Gets/sets the style definition name
		/// </summary>
		public Color? background { get; set; }

		/// <summary>
		/// Gets/sets the style definition name
		/// </summary>
		public Color? bordercolor { get; set; }

		/// <summary>
		/// Applies the visitor to this element.
		/// </summary>
		/// <param name="visitor"></param>
		/// <returns></returns>
		public override object AcceptVisitor(IXshtdVisitor visitor)
		{
			return visitor.VisitGlobalStyle(styles, this);
		}
	}
}
