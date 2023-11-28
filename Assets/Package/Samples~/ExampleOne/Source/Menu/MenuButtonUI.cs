using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ExampleOne.Menu
{
	public class MenuButtonUI : MonoBehaviour
	{
		public event Action<MenuButtonUI> OnClick;
		public string Id => _buttonId;
		
		[SerializeField] private Button _button;
		[SerializeField] private TMP_Text _label;
		[SerializeField] private string _labelText;
		[SerializeField] private string _buttonId;

		public void Awake()
		{
			Assert.IsNotNull(_button);
			Assert.IsNotNull(_label);
			
			_label.text = _labelText;
		}

		public void OnEnable()
		{
			_button.onClick.AddListener(HandleClick);
		}

		public void OnDisable()
		{
			_button.onClick.RemoveListener(HandleClick);
		}

		private void OnValidate()
		{
			if (_label == null) return;
			
			_label.text = _label.text = _labelText;;
			name = $"Button: {_label.text}";
		}

		private void Reset()
		{
			_button = GetComponentInChildren<Button>();
			_label = GetComponentInChildren<TMP_Text>();
		}

		private void HandleClick()
		{
			OnClick?.Invoke(this);
		}
	}
}