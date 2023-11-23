using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeIconAttribute : Attribute
	{
		public string Path { get; private set; }
		
		public NodeIconAttribute(string path)
		{
			Path = path;
		}
	}
}