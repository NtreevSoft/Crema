using System;
using System.Xml;
using System.Xml.Schema;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.HighlightingTheme;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Manager
{
    /// <summary>
	/// Static class with helper methods to load XSHD highlighting files.
	/// </summary>
	public static class HighlightingLoader
	{
		/// <summary>
		/// Lodas a syntax definition from the xml reader.
		/// </summary>
		public static XshdSyntaxDefinition LoadXshd(XmlReader reader)
		{
			return LoadXshd(reader, false);
		}

		internal static XshdSyntaxDefinition LoadXshd(XmlReader reader, bool skipValidation)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");
			try
			{
				reader.MoveToContent();
				return V2Loader.LoadDefinition(reader, skipValidation);
			}
			catch (XmlSchemaException ex)
			{
				throw WrapException(ex, ex.LineNumber, ex.LinePosition);
			}
			catch (XmlException ex)
			{
				throw WrapException(ex, ex.LineNumber, ex.LinePosition);
			}
		}

        private static Exception WrapException(Exception ex, int lineNumber, int linePosition)
		{
			return new HighlightingDefinitionInvalidException(FormatExceptionMessage(ex.Message, lineNumber, linePosition), ex);
		}

		internal static string FormatExceptionMessage(string message, int lineNumber, int linePosition)
        {
            if (lineNumber <= 0)
				return message;
            
            return "Error at position (line " + lineNumber + ", column " + linePosition + "):\n" + message;
        }

		internal static XmlReader GetValidatingReader(XmlReader input, bool ignoreWhitespace, XmlSchemaSet schemaSet)
		{
			var settings = new XmlReaderSettings();
			settings.CloseInput = true;
			settings.IgnoreComments = true;
			settings.IgnoreWhitespace = ignoreWhitespace;
			if (schemaSet != null)
			{
				settings.Schemas = schemaSet;
				settings.ValidationType = ValidationType.Schema;
			}
			return XmlReader.Create(input, settings);
		}

		internal static XmlSchemaSet LoadSchemaSet(XmlReader schemaInput)
		{
			var schemaSet = new XmlSchemaSet();
			schemaSet.Add(null, schemaInput);
			schemaSet.ValidationEventHandler += (sender, args) => throw new HighlightingDefinitionInvalidException(args.Message);
			return schemaSet;
		}

		/// <summary>
		/// Creates a highlighting definition from the XSHD file.
		/// </summary>
		public static IHighlightingDefinition Load(XshdSyntaxDefinition syntaxDefinition,
												   IHighlightingDefinitionReferenceResolver resolver)
		{
			if (syntaxDefinition == null)
				throw new ArgumentNullException("syntaxDefinition");

			return new XmlHighlightingDefinition(syntaxDefinition, resolver);
		}

		/// <summary>
		/// Loads a highlighting definition base on a:
		/// </summary>
		/// <param name="themedHighlights">
		/// Themed Highlighting Definition
		/// (This contains the color definition for a highlighting in this theme)
		/// </param>
		/// <param name="syntaxDefinition">
		/// A Highlighting definition
		/// (This contains the pattern matching and color definitions where the later
		///  is usually overwritten be a highlighting theme)
		/// </param>
		/// <param name="resolver">An object that can resolve a highlighting definition by its name.</param>
		/// <returns></returns>
		public static IHighlightingDefinition Load(SyntaxDefinition themedHighlights,
												   XshdSyntaxDefinition syntaxDefinition,
												   IHighlightingDefinitionReferenceResolver resolver
												   )
		{
			if (syntaxDefinition == null)
				throw new ArgumentNullException("syntaxDefinition");

			return new XmlHighlightingDefinition(themedHighlights, syntaxDefinition, resolver);
		}

		/// <summary>
		/// Creates a highlighting definition from the XSHD file that is already initialled
		/// in the <see cref="XmlReader"/> instance of the <paramref name="reader"/> parameter.
		/// </summary>
		public static IHighlightingDefinition Load(XmlReader reader,
												   IHighlightingDefinitionReferenceResolver resolver)
		{
			return Load(LoadXshd(reader), resolver);
		}
	}
}
