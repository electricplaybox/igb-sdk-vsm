using System;

namespace VisualStateMachine.Attributes
{
	public enum NodeType
	{
		None,
		Relay,
		Jump
	}
	
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeTypeAttribute : Attribute
	{
		public NodeType NodeType { get; private set; }
		
		public NodeTypeAttribute(NodeType nodeType)
		{
			NodeType = nodeType;
		}
	}
}