using UnityEditor;
using UnityEngine;
using Vsm.Editor.Graph;
using Vsm.Serialization;

namespace Vsm.Editor
{
	[CustomEditor(typeof(StateMachineController))]
	public class StateMachineControllerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			var stateMachineController = target as StateMachineController;
			var graphData = stateMachineController.GraphData;
			var isButtonEnabled = graphData != null;
			
			EditorGUILayout.BeginHorizontal();
			{
				stateMachineController.GraphData = (VsmGraphData) EditorGUILayout.ObjectField("Graph Data:", 
					stateMachineController.GraphData, 
					typeof(VsmGraphData),
					false);
			
				GUI.enabled = isButtonEnabled;
				if (GUILayout.Button("Edit", GUILayout.Width(50)))
				{
					VsmWindow.OpenWindow();
				}
				GUI.enabled = true;
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}