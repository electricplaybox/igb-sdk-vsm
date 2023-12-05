using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Attributes;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.States;

namespace VisualStateMachine.Editor.Windows
{
	public class StateSelectorWindow : EditorWindow
	{
		public Action<Type> OnTypeSelected;
		
		private TextField _searchField;
		private StateMachine _stateMachine;

		public static void Open(StateMachine stateMachine, Action<Type> onTypeSelected)
		{
			var window = GetWindow<StateSelectorWindow>("Select State Type");
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateSelectorWindow"));
			window.OnTypeSelected = onTypeSelected;
			window.SearchStates(stateMachine, string.Empty);
		}
		
		public static void Open(StateMachine stateMachine, Vector2 position, Action<Type> onTypeSelected)
		{
			var window = GetWindow<StateSelectorWindow>("Select State Type");
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateSelectorWindow"));
			window.OnTypeSelected = onTypeSelected;
			window.SearchStates(stateMachine, string.Empty);
			window.position = new Rect(position, window.position.size);
		}
		
		private void OnLostFocus()
		{
			this.Close();
		}

		public void CreateGUI()
		{
			var root = rootVisualElement;

			var searchField = new ToolbarSearchField();
			searchField.name = "search-field";
			searchField.AddToClassList("search-bar");
			searchField.RegisterValueChangedCallback(evt =>
			{
				SearchStates(_stateMachine, evt.newValue);
			});
			searchField.RegisterCallback<GeometryChangedEvent>(evt =>
			{
				searchField.Q<TextField>().Q("unity-text-input").Focus();
			});
			root.Add(searchField);

			var results = new ScrollView();
			root.Add(results);
			results.Clear();
		}
		
		private void SearchStates(StateMachine stateMachine, string searchQuery)
		{
			var emptySearchQuery = string.IsNullOrEmpty(searchQuery);
			
			_stateMachine = stateMachine;
			var container = rootVisualElement.Q<ScrollView>();
			container.Clear();

			var groupedStates = GetGroupedStates();
			var nearestGroupToStateMachine = FindNearestNamespaceToStateMachine(stateMachine, groupedStates);
			
			for (var groupIndex = 0; groupIndex < groupedStates.Count; groupIndex++)
			{
				var group = groupedStates[groupIndex];

				var firstGroup = groupIndex == 0;
				var open = firstGroup || nearestGroupToStateMachine == groupIndex || !emptySearchQuery;

				var buttonIcon = firstGroup ? NodeIcon.VsmBlue : NodeIcon.VsmGreen;
				var folderName = firstGroup ? "VSM States" : group[0].Namespace;
				var folderIcon = firstGroup ? NodeIcon.FolderBlue : NodeIcon.FolderGreen;
				
				Foldout groupBody = null;
				for (var stateIndex = 0; stateIndex < group.Count; stateIndex++)
				{
					var stateType = group[stateIndex];
					if (!stateType.Name.Contains(searchQuery)) continue;

					//Prevent empty groups from being created
					if (groupBody == null)
					{
						groupBody = MakeGroupFoldout(groupIndex, folderName, folderIcon, open);
						container.Add(groupBody);
					}

					var button = MakeStateButton(stateType, stateIndex, buttonIcon);
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
		
		private Button MakeStateButton(Type stateType, int index, string icon)
		{
			var isEven = index % 2 == 0;
			var button = new Button(() => SelectState(stateType)) { };
			button.name = "state-button";

			if (isEven) button.AddToClassList("even");

			button.Insert(0, new Image()
			{
				image = Resources.Load<Texture2D>(icon),
				scaleMode = ScaleMode.ScaleToFit
			});

			var stateNamespace = stateType.Namespace;
			var stateName = stateType.Name;

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

		private Foldout MakeGroupFoldout(int groupIndex, string groupName, string icon, bool foldedState = false)
		{
			var groupBody = new Foldout();
			groupBody.value = foldedState;
			groupBody.name = "group-body";
			if(groupIndex == 0) groupBody.AddToClassList("first-group");
			
			var checkMark = groupBody.Q("unity-checkmark");
			checkMark.parent.name = "group-header";
			checkMark.parent.Insert(0, MakeGroupIcon(icon));
				
			var label = new Label(groupName);
			checkMark.parent.Insert(1, label);

			return groupBody;
		}

		private Image MakeGroupIcon(string icon)
		{
			return new Image()
			{
				image = Resources.Load<Texture2D>(icon),
				scaleMode = ScaleMode.ScaleToFit,
				name = "group-icon"
			};
		}

		private List<List<Type>> GetGroupedStates()
		{
			var derivedTypes = AssetUtils.GetAllDerivedTypes<State>();
			var filterOutAbstractStates = derivedTypes.Where(type => !type.IsAbstract).ToList();
			var filterOutHiddenStates = filterOutAbstractStates.Where(type => !Attribute.IsDefined(type, typeof(HideNodeAttribute))).ToList();
			var groupedStates = filterOutHiddenStates
				.GroupBy(state => state.Namespace)
				.Select(group => group.ToList())
				.ToList();

			MoveListWithNamespaceToFront(groupedStates, "VisualStateMachine.States");
			
			return groupedStates;
		}
		
		public static void MoveListWithNamespaceToFront(List<List<Type>> lists, string namespaceName)
		{
			var matchingList = lists.FirstOrDefault(list => list.Any() && list[0].Namespace == namespaceName);

			if (matchingList != null)
			{
				lists.Remove(matchingList);
				lists.Insert(0, matchingList);
			}
		}
		
		public static string GetScriptPath(Type type)
		{
			var monoScript = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance(type));
			var path = AssetDatabase.GetAssetPath(monoScript);
			
			return path;
		}
	}
}