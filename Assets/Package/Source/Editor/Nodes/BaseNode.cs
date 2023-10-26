using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Vsm.Editor.Nodes
{
	public class BaseNode : Node
	{
		private Color _entryColor = new(0f, 0.4f, 0f, 0.8f);
		public string GUID;
		public bool EntyPoint { get; private set; }

		public void SetAsEntryPoint(bool entryPoint)
		{
			EntyPoint = entryPoint;


			//var titleElement = this.Q("title");

			//titleElement.style.backgroundColor = new StyleColor(EntyPoint ? _entryColor : Color.clear);
			if (EntyPoint)
				AddToClassList("entry-node");
			else
				RemoveFromClassList("entry-node");
		}
	}
}