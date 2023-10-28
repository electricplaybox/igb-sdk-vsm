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
		private StateMachineController _stateMachineController;

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
			
			if (playModeState != PlayModeStateChange.EnteredPlayMode)
			{
				window.RedrawLiveData();
			}
			else if (playModeState != PlayModeStateChange.ExitingPlayMode)
			{
				window.RedrawGraphData();
			}
		}

		public void RedrawLiveData()
		{
			if(_stateMachineController == null) return;
			
			_graphData = _stateMachineController.LiveGraphData;
			Draw();
		}
		
		public void RedrawGraphData()
		{
			_graphData = _stateMachineController.GraphData;
			Draw();
		}
		
		public static void OpenWindowWithGraphData(VsmGraphData graphData)
		{
			var window = OpenWindow();
			window.Populate(graphData);
		}
		
		public static void OpenWindowWithController(StateMachineController stateMachineController)
		{
			var window = OpenWindow();
			window.Populate(stateMachineController);
		}

		private void Populate(VsmGraphData graphData)
		{
			_stateMachineController = null;
			_graphData = graphData;
			
			Draw();
		}

		private void Populate(StateMachineController stateMachineController)
		{
			_stateMachineController = stateMachineController;
			_graphData = _stateMachineController.LiveGraphData;
			
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
	}
}