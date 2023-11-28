using System;
using System.Collections.Generic;
using ExampleOne.Menu;
using TMPro;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace ExampleOne.States
{
	public abstract class MenuState : State
	{
		[SerializeField] 
		private MenuConfig _menuConfig;

		private Dictionary<string, Action> _menuActions;

		protected abstract Dictionary<string, Action> MapActions();

		public MenuState()
		{
			_menuActions = MapActions();
		}
		
		public override void EnterState()
		{
			_menuConfig.ShowMenu();
			_menuConfig.OnMenuSelected += HandleMenuSelected;
		}

		protected virtual void HandleMenuSelected(string buttonId)
		{
			if (!_menuActions.ContainsKey(buttonId)) return;
			_menuActions[buttonId]?.Invoke();
		}

		public override void UpdateState()
		{
			
		}

		public override void ExitState()
		{
			_menuConfig.HideMenu();
		}
	}
}