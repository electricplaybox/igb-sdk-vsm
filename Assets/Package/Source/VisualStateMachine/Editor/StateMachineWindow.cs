using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Tools;

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
			DevLog.Log("StateMachineWindow OpenWindow");
			var window = GetWindow<StateMachineWindow>(false, Title, false);
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateMachineEditor"));
			window.Draw();
			
			return window;
		}
		
		public static StateMachineWindow OpenWindow(StateMachine stateMachine)
		{
			DevLog.Log($"StateMachineWindow OpenWindow(stateMachine:{stateMachine})");
			var window = OpenWindow();
			window.Draw(stateMachine);

			return window;
		}

		static StateMachineWindow()
		{
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
			EditorApplication.projectWindowItemOnGUI += HandleProjectWindowItemGUI;
		}

		public override void DiscardChanges()
		{
			DevLog.Log("DISCARD CHANGES");
			base.DiscardChanges();
		}

		public override void SaveChanges()
		{
			DevLog.Log("SAVE CHANGES");
			base.SaveChanges();
		}

		public void OnBecameInvisible()
		{
			DevLog.Log("WINDOW BECAME INVISIBLE");
		}

		public void OnBecameVisible()
		{
			DevLog.Log("WINDOW BECAME VISIBLE");
			OpenWindow();
		}

		private void OnSelectionChange()
		{
			DevLog.Log("ON SELECTION CHANGE");
			OpenWindow();
		}

		private void OnTabDetached()
		{
			DevLog.Log("ON TAB DETACHED");
		}

		protected override void OnBackingScaleFactorChanged()
		{
			base.OnBackingScaleFactorChanged();
			DevLog.Log("ON BACKING SCALE FACTOR CHANGED");
		}

		private void OnAddedAsTab()
		{
			DevLog.Log("ON ADDED AS A TAB");
		}

		private void OnDidOpenScene()
		{
			DevLog.Log("ON DID OPEN SCENE");
		}

		private void OnMainWindowMove()
		{
			DevLog.Log("ON MAIN WINDOW MOVE");
		}

		private static void HandleProjectWindowItemGUI(string guid, Rect selectionRect)
		{
			if (Event.current.type != EventType.MouseDown || Event.current.clickCount != 2) return;
			if (!TryGetSelectedStateMachine(out StateMachine stateMachine)) return;
			
			DevLog.Log("HandleProjectWindowItemGUI");
			OpenWindow(stateMachine);
			Event.current.Use();
		}

		private static void HandlePlayModeStateChanged(PlayModeStateChange mode)
		{
			//Do something
		}

		private static bool TryGetSelectedStateMachine(out StateMachine stateMachine)
		{
			stateMachine = null;

			var selected = Selection.activeObject;
			if (selected == null) return false;
			if (selected is not StateMachine selectedStateMachine) return false;

			stateMachine = selectedStateMachine;
			
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
			DevLog.Log("StateMachineWindow Draw .");
			
			if (_stateMachine != null)
			{
				DevLog.Log("StateMachineWindow Draw 1a");
				Draw(_stateMachine);
				return;
			}

			if (_graphView == null)
			{
				DevLog.Log("StateMachineWindow Draw 1b");
				_graphView = new StateMachineGraphView();
				rootVisualElement.Clear();
				rootVisualElement.Add(_graphView);
			}
		}
		
		private void Draw(StateMachine stateMachine)
		{
			// DevLog.Log($"StateMachineWindow Draw(stateMachine) 1a - {stateMachine.Nodes.Sum(node => node.Connections.Count)}");
			_stateMachine = stateMachine;
			
			if (_graphView == null)
			{
				DevLog.Log($"StateMachineWindow Draw(stateMachine) 1b");
				_graphView = new StateMachineGraphView(_stateMachine);
				rootVisualElement.Clear();
				rootVisualElement.Add(_graphView);
			}
			else
			{
				// DevLog.Log($"StateMachineWindow Draw(stateMachine) 1c");
				_graphView.Update(_stateMachine);
			}
		}
		
		private void OnEnable()
		{
			EditorApplication.update += HandleEditorUpdate;
		}

		private void OnDisable()
		{
			EditorApplication.update -= HandleEditorUpdate;
		}
		
		private void HandleEditorUpdate()
		{
			if (!TryGetStateMachine(out var stateMachine)) return;
			
			if (_stateMachine == null && stateMachine == null)
			{
				_graphView?.ClearGraph();
			}
				
			Draw(stateMachine);
		}
	}
}