using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Manager
{
    /// <summary>
	/// HighlightingBrush implementation that finds a brush using a resource.
	/// </summary>
	[Serializable]
    internal sealed class SystemColorHighlightingBrush : HighlightingBrush, ISerializable
	{
		readonly PropertyInfo property;

		public SystemColorHighlightingBrush(PropertyInfo property)
		{
			Debug.Assert(property.ReflectedType == typeof(SystemColors));
			Debug.Assert(typeof(Brush).IsAssignableFrom(property.PropertyType));
			this.property = property;
		}

		public override Brush GetBrush(ITextRunConstructionContext context)
		{
			return (Brush)property.GetValue(null, null);
		}

		public override string ToString()
		{
			return property.Name;
		}

        private SystemColorHighlightingBrush(SerializationInfo info, StreamingContext context)
		{
			property = typeof(SystemColors).GetProperty(info.GetString("propertyName"));
			if (property == null)
				throw new ArgumentException("Error deserializing SystemColorHighlightingBrush");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("propertyName", property.Name);
		}

		public override bool Equals(object obj)
		{
            return obj is SystemColorHighlightingBrush other && object.Equals(this.property, other.property);
        }

		public override int GetHashCode()
		{
			return property.GetHashCode();
		}
	}

}
