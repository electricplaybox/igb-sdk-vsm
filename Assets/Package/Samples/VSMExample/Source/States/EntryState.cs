using System;
using Vsm.Attributes;
using Vsm.States;

namespace Samples.VSMExample.Source.States
{
	public class EntryState : State
	{
		[Transition] public event Action Exit;

		[Transition] public event Action Continue;
	}
}