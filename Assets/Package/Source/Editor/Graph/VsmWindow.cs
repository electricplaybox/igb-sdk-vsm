using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Vsm.Serialization;

namespace Vsm.Editor.Graph
{
	public class VsmWindow : GraphViewEditorWindow
	{
		private VsmGraphData _graphData;
		private VsmGraphView _vsm;

		private void Update()
		{
			HandleSaveKeyboardShortcut();
		}

		private void OnDisable()
		{
			if (_vsm == null) return;

			_vsm.Dispose();
		}

		[MenuItem("Tools/Visual State Machine")]
		public static VsmWindow OpenWindow()
		{
			var window = GetWindow<VsmWindow>();
			window.titleContent = new GUIContent("Visual State Machine");

			return window;
		}

		public static void OpenWindowWithGraphData(VsmGraphData graphData)
		{
			var window = OpenWindow();
			window.Populate(graphData);
		}

		private void Populate(VsmGraphData graphData)
		{
			_graphData = graphData;

			if (_vsm != null)
			{
				rootVisualElement.Remove(_vsm);
				_vsm.Dispose();
			}

			_vsm = new VsmGraphView(_graphData);
			_vsm.StretchToParentSize();
			rootVisualElement.Add(_vsm);
		}

		private void HandleSaveKeyboardShortcut()
		{
			if (focusedWindow != this) return;
			if (Event.current == null) return;
			if (!Event.current.control) return;
			if (Event.current.type != EventType.KeyDown) return;
			if (Event.current.keyCode != KeyCode.S) return;

			_vsm.SaveData();
			Event.current.Use();
		}
	}
}