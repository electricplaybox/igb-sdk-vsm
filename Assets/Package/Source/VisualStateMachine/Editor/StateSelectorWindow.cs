using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;
using Object = System.Object;

namespace VisualStateMachine.Editor
{
	public class StateSelectorWindow : EditorWindow
	{
		public Action<Type> OnTypeSelected;
		
		private TextField _searchField;
		private StateMachine _stateMachine;

		private const string FlowIcon = "flow";
		private const string FolderIcon = "folder";

		public static void Open(StateMachine stateMachine, Action<Type> onTypeSelected)
		{
			var window = GetWindow<StateSelectorWindow>("Select State Type");
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateSelectorWindow"));
			window.OnTypeSelected = onTypeSelected;
			window.SearchStates(stateMachine, string.Empty);
		}
		
		private void OnLostFocus()
		{
			this.Close();
		}

		public void CreateGUI()
		{
			var root = rootVisualElement;

			var searchField = new ToolbarSearchField();
			searchField.AddToClassList("search-bar");
			searchField.RegisterValueChangedCallback(evt =>
			{
				SearchStates(_stateMachine, evt.newValue);
			});
			root.Add(searchField);

			var results = new ScrollView();
			root.Add(results);
			results.Clear();
		}

		private void SearchStates(StateMachine stateMachine, string searchQuery)
		{
			_stateMachine = stateMachine;
			var container = rootVisualElement.Q<ScrollView>();
			container.Clear();

			var groupedStates = GetGroupedStates();
			var nearestGroupToStateMachine = FindNearestNamespaceToStateMachine(stateMachine, groupedStates);
			
			for (var groupIndex = 0; groupIndex < groupedStates.Count; groupIndex++)
			{
				var group = groupedStates[groupIndex];
				
				var open = nearestGroupToStateMachine == groupIndex;
				var groupBody = MakeGroupFoldout(groupIndex, group[0].Namespace, open);
				container.Add(groupBody);

				for (var stateIndex = 0; stateIndex < group.Count; stateIndex++)
				{
					var stateType = group[stateIndex];
					if (!stateType.Name.Contains(searchQuery)) continue;
					
					var button = MakeStateButton(stateType, stateIndex);
					groupBody.Add(button);
				}
			}
		}

		private int FindNearestNamespaceToStateMachine(StateMachine stateMachine, IReadOnlyList<List<Type>> groupedStates)
		{
			if (stateMachine == null) return -1;
			
			var closestMatch = 0;
			var smallestDistance = int.MaxValue;
			var stateMachinePath = AssetDatabase.GetAssetPath(stateMachine);

			for (var index = 0; index < groupedStates.Count; index++)
			{
				var group = groupedStates[index];
				var groupLocation = GetScriptPath(group[0]);
				
				
				var distance = StringUtils.FindLevenshteinDistance(stateMachinePath, groupLocation);
				if (distance >= smallestDistance) continue;
				
				smallestDistance = distance;
				closestMatch = index;
			}

			return closestMatch;
		}
		
		private Button MakeStateButton(Type stateType, int index)
		{
			var isEven = index % 2 == 0;
			var button = new Button(() => SelectState(stateType)) { };
			button.name = "state-button";

			if (isEven) button.AddToClassList("even");

			button.Insert(0, new Image()
			{
				image = Resources.Load<Texture2D>(FlowIcon),
				scaleMode = ScaleMode.ScaleToFit
			});

			var stateNamespace = stateType.Namespace;
			var stateName = $"{stateNamespace}/{stateType.Name}";

			button.Add(new Label()
			{
				text = stateName
			});

			return button;
		}

		private void SelectState(Type stateType)
		{
			OnTypeSelected?.Invoke(stateType);
			Close();
		}

		private Foldout MakeGroupFoldout(int groupIndex, string groupName, bool foldedState = false)
		{
			var groupBody = new Foldout();
			groupBody.value = foldedState;
			groupBody.name = "group-body";
			if(groupIndex == 0) groupBody.AddToClassList("first-group");
			
			var checkMark = groupBody.Q("unity-checkmark");
			checkMark.parent.name = "group-header";
			checkMark.parent.Insert(0, MakeGroupIcon());
				
			var label = new Label(groupName);
			checkMark.parent.Insert(1, label);

			return groupBody;
		}

		private Image MakeGroupIcon()
		{
			return new Image()
			{
				image = Resources.Load<Texture2D>(FolderIcon),
				scaleMode = ScaleMode.ScaleToFit,
				name = "group-icon"
			};
		}

		private List<List<Type>> GetGroupedStates()
		{
			var derivedTypes = AssetUtils.GetAllDerivedTypes<State>();
			var filteredStates = derivedTypes.Where(type => !Attribute.IsDefined(type, typeof(HideNodeAttribute))).ToList();
			return filteredStates
				.GroupBy(state => state.Namespace)
				.Select(group => group.ToList())
				.ToList();
		}
		
		public static string GetScriptPath(Type type)
		{
			var monoScript = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance(type));
			var path = AssetDatabase.GetAssetPath(monoScript);
			
			return path;
		}
	}
}