using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeIcon : Attribute
	{
		public string Path { get; private set; }
		
		public NodeIcon(string path)
		{
			Path = path;
		}
	}
}