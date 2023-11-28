using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace ExampleOne.Menu
{
	public class MenuUI : MonoBehaviour
	{
		public MenuConfig MenuConfig => _menuConfig;
		
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private MenuConfig _menuConfig;
		[SerializeField] private MenuButtonUI[] _menuButtons;
		[SerializeField] private CanvasGroup _canvasGroup;

		private void Awake()
		{
			Assert.IsNotNull(_canvasGroup);
			Assert.IsNotNull(_menuConfig);

			_titleLabel.text = _menuConfig.name;
			
			Hide();
		}

		public void OnEnable()
		{
			_menuConfig.Register(this);

			foreach (var button in _menuButtons)
			{
				button.OnClick += OnMenuButtonClicked;
			}
		}

		private void OnMenuButtonClicked(MenuButtonUI menuButtonUI)
		{
			_menuConfig.SelectMenu(menuButtonUI.Id);
		}

		public void OnDisable()
		{
			_menuConfig.UnRegister(this);
		}

		public void Show()
		{
			ToggleVisibility(true);
		}

		public void Hide()
		{
			ToggleVisibility(false);
		}

		private void ToggleVisibility(bool visibility)
		{
			_canvasGroup.alpha = visibility ? 1 : 0;
			_canvasGroup.interactable = visibility;
			_canvasGroup.blocksRaycasts = visibility;
		}

		private void Reset()
		{
			_menuButtons = GetComponentsInChildren<MenuButtonUI>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_titleLabel = GetComponentInChildren<TMP_Text>();
		}
	}
}