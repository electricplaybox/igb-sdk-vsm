using System;
using Vsm.Attributes;
using Vsm.States;

namespace Samples.VSMExample.Source.States
{
	public class SomeOtherState : State
	{
		[Transition] public event Action Foo;
	}
}