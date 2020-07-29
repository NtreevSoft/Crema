using System;
using System.Collections.Generic;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels.HL.Manager
{
    /// <summary>
	/// This class is used to prevent stack overflows by representing a 'busy' flag
	/// that prevents reentrance when another call is running.
	/// However, using a simple 'bool busy' is not thread-safe, so we use a
	/// thread-static BusyManager.
	/// </summary>
	internal static class BusyManager
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Should always be used with 'var'")]
		public struct BusyLock : IDisposable
		{
			public static readonly BusyLock Failed = new BusyLock(null);

			readonly List<object> objectList;

			internal BusyLock(List<object> objectList)
			{
				this.objectList = objectList;
			}

			public bool Success => objectList != null;

            public void Dispose()
            {
                objectList?.RemoveAt(objectList.Count - 1);
            }
		}

		[ThreadStatic]
        private static List<object> activeObjects;

		public static BusyLock Enter(object obj)
		{
			var objects = BusyManager.activeObjects;
			if (objects == null)
				objects = BusyManager.activeObjects = new List<object>();
			for (int i = 0; i < objects.Count; i++)
			{
				if (objects[i] == obj)
					return BusyLock.Failed;
			}
			objects.Add(obj);
			return new BusyLock(objects);
		}
	}
}
