using Package.Source.Runtime.StateMachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Package.Source.Editor.StateMachineEditor
{
	public class StateNodeView : Node
	{
		public StateNode Data;

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
			if (Data.EntryPoint)
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