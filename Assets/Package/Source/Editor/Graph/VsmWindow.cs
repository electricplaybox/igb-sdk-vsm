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
		private VsmGraphView _graphView;

		static VsmWindow()
		{
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
		}
		
		[MenuItem("Tools/Visual State Machine")]
		public static VsmWindow OpenWindow()
		{
			var window = GetWindow<VsmWindow>();
			window.titleContent = new GUIContent("Visual State Machine");
			window.Populate(graphData: null);
			
			return window;
		}
		
		private void Update()
		{
			HandleSaveKeyboardShortcut();

			if (_graphView == null) return;
			_graphView.Update();
		}
		
		private void CleanUpGraphView()
		{
			if(_graphView == null) return;
			
			rootVisualElement.Remove(_graphView);
			_graphView?.Dispose();
			_graphView = null;
		}

		private static void HandlePlayModeStateChanged(PlayModeStateChange playModeState)
		{
			var window = GetWindow<VsmWindow>();
			window.Draw();
		}
		
		public static void OpenWindowWithGraphData(VsmGraphData graphData)
		{
			var window = OpenWindow();
			window.Populate(graphData);
		}
		
		private void Populate(VsmGraphData graphData)
		{
			_graphData = graphData;
			Draw();
		}
		
		private void Draw()
		{
			CleanUpGraphView();
			
			_graphView = new VsmGraphView(_graphData);
			_graphView.StretchToParentSize();
			rootVisualElement.Add(_graphView);
		}

		private void HandleSaveKeyboardShortcut()
		{
			if (focusedWindow != this) return;
			if (Event.current == null) return;
			if (!Event.current.control) return;
			if (Event.current.type != EventType.KeyDown) return;
			if (Event.current.keyCode != KeyCode.S) return;

			_graphView.SaveData();
			Event.current.Use();
		}

		private void OnGUI()
		{
			var selectedGameObject = Selection.activeGameObject;
			if (selectedGameObject == null) return;

			var stateMachine = selectedGameObject.GetComponent<StateMachineController>();
			if (stateMachine == null) return;
			
			VsmGraphData newGraphData = null;

			if (Application.isPlaying)
			{
				newGraphData = stateMachine.LiveGraphData;
			}
			else
			{
				newGraphData = stateMachine.GraphData;
			}

			if (newGraphData == null) return;
			if (newGraphData == _graphData) return;

			_graphData = newGraphData;
			Draw();
		}
	}
}