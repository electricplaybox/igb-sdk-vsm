using System;
using System.Collections.Generic;
using ExampleOne.Menu;
using UnityEngine;
using VisualStateMachine.Attributes;

namespace ExampleOne.States
{
	public class HomeMenuState : MenuState
	{
		[Transition("Store")] 
		public event Action OnStore;

		[Transition("Learn More")] 
		public event Action OnLearnMore;

		[Transition("FAQs")] 
		public event Action OnFaqs;
		
		protected override Dictionary<string, Action> MapActions()
		{
			return new Dictionary<string, Action>
			{
				{ "Store", () => OnStore?.Invoke() },
				{ "Learn More", () => OnLearnMore?.Invoke() },
				{ "FAQs", () => OnFaqs?.Invoke() }
			};
		}
	}
}