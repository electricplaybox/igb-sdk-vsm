using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
	[CreateAssetMenu(fileName = "StateMachineGraph", menuName = "StateMachine/StateMachineGraph")]
	public class StateMachineGraph : ScriptableObject
	{
		public List<SerializableKeyValuePair<string, StateNode>> Nodes = new();
		public string EntryNodeId;
		
		private Dictionary<string, StateNode> _nodeDictionary = new();
		private StateNode _entryNode;
		private StateNode _currentNode;

		public void Initialize()
		{
			if (string.IsNullOrEmpty(EntryNodeId))
			{
				return;
			}

			if (_nodeDictionary.Count == 0) return;
			
			_currentNode = _nodeDictionary[EntryNodeId];
			_currentNode.Enter();
			
			SubscribeToNode(_currentNode);
		}
			
			
		public void Load()
		{
			LoadStates();
			CacheDictionaries();
		}
		
		public void Save()
		{
			SaveStates();
		}

		public void SaveStates()
		{
			var assetPath = AssetDatabase.GetAssetPath(this);
			var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
			
			//remove all states
			foreach (var subAsset in subAssets)
			{
				if (subAsset is ScriptableObject)
				{
					AssetDatabase.RemoveObjectFromAsset(subAsset);
				}
			}
			
			//Add them all back again
			foreach (var kvp in Nodes)
			{
				if (kvp.Value.State == null) continue;
				
				AssetDatabase.AddObjectToAsset(kvp.Value.State, this);
				EditorUtility.SetDirty(this);
			}
			
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(); 
		}

		public void LoadStates()
		{
			var path = AssetDatabase.GetAssetPath(this);
			var allSubAssets = AssetDatabase.LoadAllAssetsAtPath(path).ToList();
			
			foreach (var kvp in Nodes)
			{
				var state = allSubAssets.Where(asset => asset.name == kvp.Value.Id).FirstOrDefault() as State;
				if (state == null) continue;
				
				kvp.Value.Load(state);
			}
		}

		private void SubscribeToNode(StateNode node)
		{
			var connections = node.Connections;

			foreach (var connection in node.Connections)
			{
				connection.OnTransition += OnTransition;
			}
		}

		private void Unsubscribe(StateNode node)
		{
			var connections = node.Connections;

			foreach (var connection in node.Connections)
			{
				connection.OnTransition -= OnTransition;
			}
		}

		public void OnDestroy()
		{
			_currentNode.Exit();
		}

		public void Update()
		{
			_currentNode?.Update();
		}
		
		public void AddNode(StateNode node)
		{
			Nodes.Add(new SerializableKeyValuePair<string, StateNode>(node.Id, node));
		}
		
		public void RemoveNode(StateNode nodeData)
		{
			var kvp = Nodes.Where(kvp =>
			{
				return kvp.Value == nodeData;
			}).FirstOrDefault();

			if (kvp == null) return;
			Nodes.Remove(kvp);
		}
		
		private void CacheDictionaries()
		{
			_nodeDictionary = GetNodeDictionary();
		}

		private void OnTransition(StateConnection connection)
		{
			var nextNode = _nodeDictionary[connection.ToNodeId];
			
			Unsubscribe(_currentNode);
			_currentNode.Exit();
			
			_currentNode = nextNode;
			SubscribeToNode(_currentNode);
			_currentNode.Enter();
		}

		private Dictionary<string, StateNode> GetNodeDictionary()
		{
			return Nodes.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		public void Clear()
		{
			foreach (var node in Nodes) node.Value.Clear();
			Nodes.Clear();
		}
	}
}
