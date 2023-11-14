using StateMachine;
using UnityEditor;
using UnityEngine;

namespace Editor.StateMachineEditor
{
	[CustomEditor(typeof(StateMachineController))]
	public class StateMachineControllerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			var stateMachineController = target as StateMachineController;
			if(target == null) return;
			
			var graphData = stateMachineController.GraphData;
			var isButtonEnabled = graphData != null;
			
			EditorGUILayout.BeginHorizontal();
			{
				stateMachineController.GraphData = (StateMachineGraph) EditorGUILayout.ObjectField("State Machine Graph", 
					stateMachineController.GraphData, 
					typeof(StateMachineGraph),
					false);
			
				GUI.enabled = isButtonEnabled;
				if (GUILayout.Button("Edit", GUILayout.Width(50)))
				{
					StateMachineWindow.OpenWindow(stateMachineController.GraphData);
				}
				GUI.enabled = true;
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}