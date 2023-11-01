using System.Linq;
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
		private static StateMachineWindowData _windowData;

		[MenuItem("Tools/State Machine Editor")]
		public static StateMachineWindow OpenWindow()
		{
			var windowData = GetWindowData();
			var graph = windowData.StateMachineGraph;
			Debug.Log($"OpenWindow: {graph.name}");
			
			var window = GetWindow<StateMachineWindow>();
			window.titleContent = new GUIContent("State Machine Editor");
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateMachineEditor"));
			window.Draw(graph);
			
			return window;
		}
		
		public static StateMachineWindow OpenWindow(StateMachineController controller)
		{
			GetWindowData().SetStateMachineController(controller);
			
			return OpenWindow();
		}
		
		public static StateMachineWindow OpenWindow(StateMachineGraph graph)
		{
			GetWindowData().SetStateMachineGraph(graph);
			
			return OpenWindow();
		}

		private static bool HasGraph(StateMachineGraph graph)
		{
			return GetWindowData().StateMachineGraph == graph;
		}
		
		private bool HasController(StateMachineController stateMachine)
		{
			return GetWindowData().StateMachineController == stateMachine;
		}

		private static StateMachineWindowData GetWindowData()
		{
			if (_windowData != null) return _windowData;
			
			var guids = AssetDatabase.FindAssets("t:StateMachineWindowData", new[] {"Assets", "Packages"});
			if (guids.Length > 0)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
				_windowData = AssetDatabase.LoadAssetAtPath<StateMachineWindowData>(assetPath);
			}

			return _windowData;
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
			
			GetWindowData().SetStateMachineGraph(selectedObject as StateMachineGraph);

			OpenWindow();
			Event.current.Use();
		}

		private static void HandlePlayModeStateChanged(PlayModeStateChange stateChange)
		{
			OpenWindow();
		}
		
		private void Draw(StateMachineGraph graph)
		{
			rootVisualElement.Clear();
			
			var graphView = new StateMachineGraphView(graph);
			rootVisualElement.Add(graphView);
		}
		
		private void OnGUI()
		{
			HandleSelectedGameObject();
			HandleSelectedObject();
		}

		private void HandleSelectedObject()
		{
			var selectedObject = Selection.activeObject;
			if (selectedObject == null) return;
			if (selectedObject is not StateMachineGraph) return;
			
			var graph = selectedObject as StateMachineGraph;
			if (HasGraph(graph)) return;
			
			OpenWindow(graph);
		}

		private void HandleSelectedGameObject()
		{
			var selectedObject = Selection.activeGameObject;
			if (selectedObject == null) return;

			var selectedGameObject = selectedObject as GameObject;
			var stateMachine = selectedGameObject.GetComponent<StateMachineController>();
			if (stateMachine == null) return;
			
			if (HasController(stateMachine)) return;
			
			OpenWindow(stateMachine);
		}
	}
}