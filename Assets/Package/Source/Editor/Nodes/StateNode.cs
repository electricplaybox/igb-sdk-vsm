using UnityEngine;
using UnityEngine.UIElements;
using Vsm.Serialization;

namespace Vsm.Editor.Nodes
{
	public class StateNode : BaseNode
	{
		public StateNodeData Data;

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