using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Testing.NodeAttributes
{
	[NodeWidth(500)]
	public class BigNode : State
	{
		public int Foo;
		
		public override void EnterState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}