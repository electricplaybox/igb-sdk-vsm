using UnityEditor;
using UnityEngine;
using Vsm.Serialization;

namespace Vsm.Editor.Graph
{
	[InitializeOnLoad]
	public class VsmGraphDataOpener
	{
		static VsmGraphDataOpener()
		{
			EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
		}

		private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
		{
			if (Event.current.type != EventType.MouseDown || Event.current.clickCount != 2) return;

			var selectedObject = Selection.activeObject;
			if (selectedObject is not VsmGraphData) return;

			VsmWindow.OpenWindowWithGraphData((VsmGraphData)selectedObject);
			Event.current.Use();
		}
	}
}