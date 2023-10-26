using System;
using Vsm.Attributes;
using Vsm.States;

namespace Samples.VSMExample.Source.States
{
	public class MyThingyState : State
	{
		[Transition] public event Action GoSomeWhere;

		[Transition] public event Action DoSomething;
	}
}