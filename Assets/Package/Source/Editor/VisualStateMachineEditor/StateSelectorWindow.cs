using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.States;

namespace Editor.VisualStateMachineEditor
{
	public class StateSelectorWindow : EditorWindow
	{
		public Action<Type> OnTypeSelected;
		
		private TextField _searchField;
		private const string FlowIcon = "flow";

		public static void Open(Action<Type> onTypeSelected)
		{
			var window = GetWindow<StateSelectorWindow>("Select State Type");
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateSelectorWindow"));
			window.OnTypeSelected = onTypeSelected;
			window.SearchStates(string.Empty);
		}

		public void CreateGUI()
		{
			
			var root = rootVisualElement;

			var searchField = new ToolbarSearchField();
			searchField.AddToClassList("search-bar");
			searchField.RegisterValueChangedCallback(evt =>
			{
				SearchStates(evt.newValue);
			});
			root.Add(searchField);

			var results = new ScrollView();
			root.Add(results);
			results.Clear();
		}

		private void SearchStates(string searchQuery)
		{
			var results = rootVisualElement.Q<ScrollView>(); // Assuming there's only one ScrollView
			results.Clear();

			var derivedTypes = AssetUtils.GetAllDerivedTypes<State>();
			
			for (var i = 0; i < derivedTypes.Count; i++)
			{
				var isEven = i % 2 == 0;
				var stateType = derivedTypes[i];
				
				if (stateType.Name.Contains(searchQuery))
				{
					var button = new Button(() => SelectState(stateType)) { };
					if(isEven) button.AddToClassList("even");

					button.Insert(0, new Image()
					{
						image = Resources.Load<Texture2D>(FlowIcon),
						scaleMode = ScaleMode.ScaleToFit
					});

					button.Add(new Label()
					{
						text = stateType.Name
					});

					results.Add(button);
				}
			}
		}

		private void SelectState(Type stateType)
		{
			OnTypeSelected?.Invoke(stateType);
			Close();
		}
	}
}