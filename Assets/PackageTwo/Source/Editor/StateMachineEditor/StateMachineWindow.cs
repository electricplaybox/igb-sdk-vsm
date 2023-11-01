using StateMachine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.StateMachineEditor
{
	[InitializeOnLoad]
	public class StateMachineWindow : GraphViewEditorWindow
	{
		private static StateMachineGraph _stateMachineGraph;
		
		[MenuItem("Tools/State Machine Editor")]
		public static StateMachineWindow OpenWindow()
		{
			var window = GetWindow<StateMachineWindow>();
			window.titleContent = new GUIContent("State Machine Editor");
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateMachineEditor"));
			window.Draw(_stateMachineGraph);
			
			return window;
		}

		public static StateMachineWindow OpenWindow(StateMachineGraph graph)
		{
			_stateMachineGraph = graph;
			return OpenWindow();
		}
		
		static StateMachineWindow()
		{
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
			EditorApplication.projectWindowItemOnGUI += HandleProjectWindowItemGUI;
		}

		private static void HandleProjectWindowItemGUI(string guid, Rect selectionRect)
		{
			if (Event.current.type != EventType.MouseDown || Event.current.clickCount != 2) return;
			
			var selectedObject = Selection.activeObject;
			if (selectedObject is not StateMachineGraph) return;
			
			_stateMachineGraph = selectedObject as StateMachineGraph;

			OpenWindow();
			Event.current.Use();
		}

		private static void HandlePlayModeStateChanged(PlayModeStateChange stateChange)
		{
			OpenWindow();
		}

		private void Draw(StateMachineGraph stateMachineGraph)
		{
			var graphView = new StateMachineGraphView(stateMachineGraph);
			rootVisualElement.Add(graphView);
		}
	}
}