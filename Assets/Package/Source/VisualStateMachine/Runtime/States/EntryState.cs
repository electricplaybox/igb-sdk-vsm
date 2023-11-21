﻿using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{ 
	public class EntryState : State
	{
		[Transition]
		public event Action Exit;
		
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