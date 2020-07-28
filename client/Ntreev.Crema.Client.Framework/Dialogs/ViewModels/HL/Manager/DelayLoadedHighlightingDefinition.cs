using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Manager
{
    internal sealed class DelayLoadedHighlightingDefinition : IHighlightingDefinition
	{
        private readonly object lockObj = new object();
        private readonly string name;
        private Func<IHighlightingDefinition> lazyLoadingFunction;
        private IHighlightingDefinition definition;
        private Exception storedException;

		public DelayLoadedHighlightingDefinition(string name, Func<IHighlightingDefinition> lazyLoadingFunction)
		{
			this.name = name;
			this.lazyLoadingFunction = lazyLoadingFunction;
		}

		public string Name => name ?? GetDefinition().Name;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The exception will be rethrown")]
        private IHighlightingDefinition GetDefinition()
		{
			Func<IHighlightingDefinition> func;
			lock (lockObj)
			{
				if (this.definition != null)
					return this.definition;
				func = this.lazyLoadingFunction;
			}
			Exception exception = null;
			IHighlightingDefinition def = null;
			try
			{
				using (var busyLock = BusyManager.Enter(this))
				{
					if (!busyLock.Success)
						throw new InvalidOperationException("Tried to create delay-loaded highlighting definition recursively. Make sure the are no cyclic references between the highlighting definitions.");
					def = func();
				}
				if (def == null)
					throw new InvalidOperationException("Function for delay-loading highlighting definition returned null");
			}
			catch (Exception ex)
			{
				exception = ex;
			}
			lock (lockObj)
			{
				this.lazyLoadingFunction = null;
				if (this.definition == null && this.storedException == null)
				{
					this.definition = def;
					this.storedException = exception;
				}

				if (this.storedException != null)
					throw new HighlightingDefinitionInvalidException("Error delay-loading highlighting definition", this.storedException);
				return this.definition;
			}
		}

		public HighlightingRuleSet MainRuleSet => GetDefinition().MainRuleSet;

        public HighlightingRuleSet GetNamedRuleSet(string name)
		{
			return GetDefinition().GetNamedRuleSet(name);
		}

		public HighlightingColor GetNamedColor(string name)
		{
			return GetDefinition().GetNamedColor(name);
		}

		public IEnumerable<HighlightingColor> NamedHighlightingColors => GetDefinition().NamedHighlightingColors;

        public override string ToString()
		{
			return this.Name;
		}

		public IDictionary<string, string> Properties => GetDefinition().Properties;
    }
}
