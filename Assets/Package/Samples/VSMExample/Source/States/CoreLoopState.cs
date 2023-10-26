using System;
using Vsm.Attributes;
using Vsm.States;

namespace Samples.VSMExample.Source.States
{
	public class CoreLoopState : State
	{
		[Transition] public event Action Pass;

		[Transition] public event Action Fail;
	}
}