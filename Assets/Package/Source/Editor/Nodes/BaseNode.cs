using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Vsm.Editor.Nodes
{
	public class BaseNode : Node
	{
		public string Guid;
		public bool EntryPoint;

		public void SetAsEntryPoint(bool entryPoint)
		{
			EntryPoint = entryPoint;

			if (EntryPoint)
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