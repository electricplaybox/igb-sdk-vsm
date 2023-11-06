using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.StateMachineEditor
{
	public class StateMachineFieldFactory
	{
		public static VisualElement CreateIntegerField(FieldInfo fieldInfo, object targetObject)
		{
			var intField = new IntegerField(fieldInfo.Name);
			intField.value = (int)fieldInfo.GetValue(targetObject);
			return intField;
		}
		
		public static VisualElement CreateTextField(FieldInfo fieldInfo, object targetObject)
		{
			var textField = new TextField(fieldInfo.Name);
			textField.value = (string)fieldInfo.GetValue(targetObject);
			return textField;
		}

		public static VisualElement CreateScriptableObjectField(FieldInfo fieldInfo, object targetObject)
		{
			var objectField = new ObjectField(fieldInfo.Name);
			objectField.objectType = typeof(ScriptableObject);
			objectField.value = (ScriptableObject)fieldInfo.GetValue(targetObject);
			return objectField;
		}

		public static VisualElement CreateListField(FieldInfo fieldInfo, object targetObject)
		{
			var listContainer = new VisualElement();
			listContainer.Add(new Label($"{fieldInfo.Name}:"));

			var list = fieldInfo.GetValue(targetObject) as System.Collections.IList;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					object item = list[i];

					// Create a label for each item
					var itemLabel = new Label($"Item {i + 1}: {item?.ToString() ?? "null"}");
					listContainer.Add(itemLabel);
				}
			}

			return listContainer;
		}
	}
}