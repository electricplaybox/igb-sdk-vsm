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
		[MenuItem("Tools/State Machine Editor")]
		public static StateMachineWindow OpenWindow()
		{
			var window = GetWindow<StateMachineWindow>();
			window.titleContent = new GUIContent("State Machine Editor");
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateMachineEditor"));
			
			return window;
		}
		
		public static StateMachineWindow OpenWindow(StateMachine.StateMachine stateMachine)
		{
			// Debug.Log($"OpenWindow: {stateMachine.name}");
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
			if (!TryGetSelectedStateMachine(out StateMachine.StateMachine stateMachine)) return;
			
			OpenWindow(stateMachine);
			Event.current.Use();
		}

		private static void HandlePlayModeStateChanged(PlayModeStateChange obj)
		{
			//Do something
		}

		private static bool TryGetSelectedStateMachine(out StateMachine.StateMachine stateMachine)
		{
			stateMachine = null;
			
			var selected = Selection.activeObject;
			if (selected == null) return false;
			if (selected is not StateMachine.StateMachine) return false;
			
			stateMachine = selected as StateMachine.StateMachine;

			return true;
		}

		private static bool TryGetSelectedStateController(out StateMachineController stateMachineController)
		{
			stateMachineController = null;
			
			var selectedObject = Selection.activeGameObject;
			if (selectedObject == null) return false;
			
			stateMachineController = selectedObject.GetComponent<StateMachineController>();
			return stateMachineController != null;
		}
	

		private static bool TryGetStateMachine(out StateMachine.StateMachine stateMachine)
		{
			stateMachine = null;
			
			if (TryGetSelectedStateMachine(out var selectedStateMachine))
			{
				stateMachine = selectedStateMachine;
				return true;
			}

			if (TryGetSelectedStateController(out var selectedStateController))
			{
				stateMachine = selectedStateController.GetStateMachine();
				return true;
			}

			return false;
		}
		
		private void OnEnable()
		{
			EditorApplication.update += HandleEditorUpdate;
			// someState = EditorPrefs.GetInt("MyEditorWindowState", defaultValue: 0);
		}

		private void OnDisable()
		{
			EditorApplication.update -= HandleEditorUpdate;
			// EditorPrefs.SetInt("MyEditorWindowState", someState);
		}
		
		private void HandleEditorUpdate()
		{
			if (TryGetStateMachine(out StateMachine.StateMachine stateMachine))
			{
				OpenWindow(stateMachine);
			}
		}
		
		private void OnFocus()
		{
			Debug.Log("OnFocus");
		}

		private void OnDestroy()
		{
			Debug.Log("OnDestroy");
		}

		private void OnBecameInvisible()
		{
			Debug.Log("OnBecameInvisible");
		}

		private void OnBecameVisible()
		{
			Debug.Log("OnBecameVisible");
		}
		
		private void OnLostFocus()
		{
			Debug.Log("OnLostFocus");
		}

		private void ModifierKeysChanged()
		{
			Debug.Log("ModifierKeysChanged");
		}
	}
}