using System;
using System.Collections.Generic;

namespace Editor.Utils
{
	public class AssetUtils
	{
		public static List<Type> GetAllDerivedTypes<T>()
		{
			var derivedTypes = new List<Type>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
					if (type.IsSubclassOf(typeof(T)))
						derivedTypes.Add(type);
			}

			return derivedTypes;
		}
	}
}