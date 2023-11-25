using System;
using UnityEditor.Experimental.GraphView;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public enum RelayDirection
	{
		Left,
		Right
	}
	
	[NodeType(NodeType.Relay)]
	[PortOrientation(Orientation.Vertical)]
	public abstract class Relay : State
	{
		[Transition]
		public event Action Exit;
		
		public RelayDirection Direction { get; set; }
		
		public override void EnterState()
		{
			Exit?.Invoke();
		}

		public override void UpdateState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}