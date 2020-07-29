using System;
using System.Xml;
using System.Xml.Schema;
using ICSharpCode.AvalonEdit.Highlighting;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Xshtd;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Xshtd.interfaces;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Manager
{
    /// <summary>
	/// Static class with helper methods to load XSHTD highlighting files.
	/// </summary>
    internal static class HighlightingThemeLoader
	{
		/// <summary>
		/// Lodas a syntax definition from the xml reader.
		/// </summary>
		public static XhstdThemeDefinition LoadXshd(XmlReader reader)
		{
			return LoadXshd(reader, false);
		}

		internal static XhstdThemeDefinition LoadXshd(XmlReader reader, bool skipValidation)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");
			try
			{
				reader.MoveToContent();
				if (reader.NamespaceURI == XshtdLoader.Namespace)
				{
					return XshtdLoader.LoadDefinition(reader, skipValidation);
				}

				throw new ArgumentOutOfRangeException(reader.NamespaceURI);
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
			else
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
			schemaSet.ValidationEventHandler += delegate (object sender, ValidationEventArgs args)
			{
				throw new HighlightingDefinitionInvalidException(args.Message);
			};
			return schemaSet;
		}

		/// <summary>
		/// Creates a highlighting definition from the XSHD file.
		/// </summary>
		public static IHighlightingThemeDefinition Load(XhstdThemeDefinition syntaxDefinition,
														IHighlightingThemeDefinitionReferenceResolver resolver)
		{
			if (syntaxDefinition == null)
				throw new ArgumentNullException("syntaxDefinition");

			return new XmlHighlightingThemeDefinition(syntaxDefinition, resolver);
		}

		/// <summary>
		/// Creates a highlighting definition from the XSHD file.
		/// </summary>
		public static IHighlightingThemeDefinition Load(XmlReader reader,
														IHighlightingThemeDefinitionReferenceResolver resolver)
		{
			return Load(LoadXshd(reader), resolver);
		}
	}
}
