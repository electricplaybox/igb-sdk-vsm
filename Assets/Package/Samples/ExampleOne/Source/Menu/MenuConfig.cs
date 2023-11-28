using System;
using UnityEngine;

namespace ExampleOne.Menu
{
	[CreateAssetMenu(fileName = "MenuConfig", menuName = "Samples/ExampleOne/MenuConfig")]
	public class MenuConfig : ScriptableInterface<MenuUI>
	{ 
		public event Action<string> OnMenuSelected;
		
		public void ShowMenu() => Instance?.Show();
		public void HideMenu() => Instance?.Hide();

		public void SelectMenu(string buttonId) => OnMenuSelected?.Invoke(buttonId);
	}
}