using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine;

namespace Editor.VisualStateMachineEditor
{
	public class StateNodeView : Node
	{
		public StateNode Data;
		public StateMachine StateMachine;

		public void Update()
		{
			if (Data == null) return;
			
			DrawEntryPoint();
			DrawActiveNode();
		}
		
		private void DrawActiveNode()
		{
			if (!Application.isPlaying) return;
			
			if (Data.IsActive)
			{
				AddToClassList("active-node");
				this.Query<ProgressBar>("progress-bar").First().value = (Time.time % 1f) * 100f;
			}
			else
			{
				RemoveFromClassList("active-node");
			}
		}

		private void DrawEntryPoint()
		{
			if (Data.IsEntryNode)
			{
				AddToClassList("entry-node");
			}
			else
			{
				RemoveFromClassList("entry-node");
			}
		}
	}
}