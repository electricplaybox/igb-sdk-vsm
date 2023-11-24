using System;
using System.Reflection;

namespace VisualStateMachine.Editor
{
	public static class AttributeUtils
	{
		public static T GetInheritedCustomAttribute<T>(Type type) where T : Attribute
		{
			T attribute = null;

			while (type != null && attribute == null)
			{
				attribute = type.GetCustomAttribute<T>();
				type = type.BaseType;
			}

			return attribute;
		}
	}
}