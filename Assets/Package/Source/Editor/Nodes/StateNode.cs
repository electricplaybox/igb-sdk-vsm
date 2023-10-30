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
			if (Data.IsActive)
			{
				AddToClassList("active-node");
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