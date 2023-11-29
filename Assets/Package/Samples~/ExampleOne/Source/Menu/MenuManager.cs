using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExampleOne.Menu
{
	public class MenuManager : MonoBehaviour
	{
		[SerializeField] private List<MenuUI> _menus = new();

		private void Reset()
		{
			_menus = GetComponentsInChildren<MenuUI>().ToList();
		}
	}
}