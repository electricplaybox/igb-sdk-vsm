using System;
using UnityEngine;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	
	[NodeType(NodeType.Jump)]
	[NodeWidth(100)]
	public class JumpFromState : State
	{
		public JumpId JumpId;
		
		public override void EnterState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}