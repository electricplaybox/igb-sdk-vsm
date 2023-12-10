using System;
using UnityEngine;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	
	[NodeType(NodeType.Jump)]
	[NodeWidth(100)]
	public abstract class JumpState : State
	{
		public JumpId JumpId;
		
	}
}