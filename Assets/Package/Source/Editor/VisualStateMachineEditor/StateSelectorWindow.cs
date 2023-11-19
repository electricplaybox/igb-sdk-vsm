using UnityEditor;
using UnityEngine;
using System;
using VisualStateMachine;

namespace Package.Source.Editor.VisualStateMachineEditor
{
	public class StateSelectorWindow : EditorWindow
	{
		private string _searchQuery = "";
		private Vector2 _scrollPosition;

		public Action<Type> OnTypeSelected;

		public static void Open(Action<Type> onTypeSelected)
		{
			var window = GetWindow<StateSelectorWindow>("Select State Type");
			window.OnTypeSelected = onTypeSelected;
		}

		void OnGUI()
		{
			RenderSearchField();
			RenderSearchResults();
		}

		private void RenderSearchField()
		{
			GUILayout.Label("Select State Type", EditorStyles.boldLabel);
			_searchQuery = EditorGUILayout.TextField("Search", _searchQuery);

			if (GUILayout.Button("Search"))
			{
				// Refreshes the list based on the search query.
			}
		}

		private void RenderSearchResults()
		{
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
			try
			{
				var guids = AssetDatabase.FindAssets("t:MonoScript");
				foreach (var guid in guids)
				{
					TryRenderAssetButton(guid);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error rendering search results: {ex.Message}");
			}
			GUILayout.EndScrollView();
		}

		private void TryRenderAssetButton(string guid)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

			if (asset != null && asset.GetClass() != null)
			{
				if (typeof(State).IsAssignableFrom(asset.GetClass()) && asset.GetClass().Name.Contains(_searchQuery))
				{
					if (GUILayout.Button(asset.GetClass().Name))
					{
						OnTypeSelected?.Invoke(asset.GetClass());
						Close();
					}
				}
			}
		}
	}

}