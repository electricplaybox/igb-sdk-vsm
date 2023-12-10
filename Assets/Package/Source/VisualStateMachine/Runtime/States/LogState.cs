using System;
using UnityEngine;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Blue), NodeIcon(NodeIcon.VsmFlatDocWhite, opacity:0.3f)]
	public class LogState : State
	{
		[Transition]
		public event Action Exit;
		
		[SerializeField, Multiline(3)] 
		private string _message = "Hello World";
		
		public override void EnterState()
		{
			Debug.Log(_message);
			Exit?.Invoke();
		}

		public override void ExitState()
		{
			//Do nothing
		}
	}
}