using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Vsm.Editor.Nodes
{
	public class BaseNode : Node
	{
		public string Guid;
		public bool EntyPoint { get; private set; }

		public void SetAsEntryPoint(bool entryPoint)
		{
			EntyPoint = entryPoint;

			if (EntyPoint)
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