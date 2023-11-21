using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using StateMachine = VisualStateMachine.StateMachine;

namespace VisualStateMachine.Editor
{
	[InitializeOnLoad]
	public class StateMachineWindow : GraphViewEditorWindow
	{
		private StateMachine _stateMachine;
		private StateMachineGraphView _graphView;
		private StateMachineWindow _openWindow;

		private const string Title = "State Machine Editor";
		
		/**
		 * Static
		 */
		
		[MenuItem("Tools/State Machine Editor")]
		public static StateMachineWindow OpenWindow()
		{
			var window = GetWindow<StateMachineWindow>(false, Title, false);
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateMachineEditor"));
			window.Draw();
			
			return window;
		}
		
		public static StateMachineWindow OpenWindow(StateMachine stateMachine)
		{
			var window = OpenWindow();
			window.Draw(stateMachine);

			return window;
		}

		static StateMachineWindow()
		{
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
			EditorApplication.projectWindowItemOnGUI += HandleProjectWindowItemGUI;
		}
		
		private static void HandleProjectWindowItemGUI(string guid, Rect selectionRect)
		{
			if (Event.current.type != EventType.MouseDown || Event.current.clickCount != 2) return;
			if (!TryGetSelectedStateMachine(out StateMachine stateMachine)) return;
			
			OpenWindow(stateMachine);
			Event.current.Use();
		}

		private static void HandlePlayModeStateChanged(PlayModeStateChange obj)
		{
			//Do something
		}

		private static bool TryGetSelectedStateMachine(out StateMachine stateMachine)
		{
			stateMachine = null;
			
			var selected = Selection.activeObject;
			if (selected == null) return false;
			if (selected is not StateMachine) return false;
			
			stateMachine = selected as StateMachine;

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
	

		private static bool TryGetStateMachine(out StateMachine stateMachine)
		{
			stateMachine = null;
			
			if (TryGetSelectedStateMachine(out var selectedStateMachine))
			{
				stateMachine = selectedStateMachine;
				return stateMachine != null;
			}

			if (TryGetSelectedStateController(out var selectedStateController))
			{
				stateMachine = selectedStateController.GetStateMachine();
				return stateMachine != null;
			}

			return false;
		}
		
		
		
		
		
		
		
		/**
		 * Instance
		 */
		private void Draw()
		{
			if (_stateMachine != null)
			{
				Draw(_stateMachine);
				return;
			}

			if (_graphView == null)
			{
				_graphView = new StateMachineGraphView();
				rootVisualElement.Clear();
				rootVisualElement.Add(_graphView);
			}
		}
		
		private void Draw(StateMachine stateMachine)
		{
			_stateMachine = stateMachine;
			
			if (_graphView == null)
			{
				_graphView = new StateMachineGraphView(_stateMachine);
				rootVisualElement.Clear();
				rootVisualElement.Add(_graphView);
			}
			else
			{
				_graphView.Update(_stateMachine);
			}
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
			if (!TryGetStateMachine(out var stateMachine)) return;
			
			OpenWindow(stateMachine);
		}
		
		// private void OnFocus()
		// {
		// 	Debug.Log("OnFocus");
		// }
		//
		// private void OnDestroy()
		// {
		// 	Debug.Log("OnDestroy");
		// }

		// private void OnBecameInvisible()
		// {
		// 	Debug.Log("OnBecameInvisible");
		// }
		//
		// private void OnBecameVisible()
		// {
		// 	Debug.Log("OnBecameVisible");
		// }
		//
		// private void OnLostFocus()
		// {
		// 	Debug.Log("OnLostFocus");
		// }
		//
		// private void ModifierKeysChanged()
		// {
		// 	Debug.Log("ModifierKeysChanged");
		// }
	}
}