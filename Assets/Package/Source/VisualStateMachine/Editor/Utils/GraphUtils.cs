using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VisualStateMachine.Editor.Windows;

namespace VisualStateMachine.Editor.Utils
{
	public class GraphUtils
	{
		public static Vector3 ScreenPointToGraphPoint(Vector2 screenPoint, GraphView graphView)
		{
			return (Vector3)screenPoint - graphView.contentViewContainer.transform.position;
		}
		
		public static Rect GraphRect(GraphView graphView)
		{
			var style = graphView.resolvedStyle;
			
			var rect = new Rect();
			rect.width = float.IsNaN(style.width) ? StateMachineWindow.WindowWidth : style.width;
			rect.height = float.IsNaN(style.height) ? StateMachineWindow.WindowHeight : style.height;
			
			return rect;
		}
	}
}