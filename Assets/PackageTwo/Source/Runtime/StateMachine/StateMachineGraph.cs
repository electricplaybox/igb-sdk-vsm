using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace StateMachine
{
	[CreateAssetMenu(fileName = "StateMachineGraph", menuName = "StateMachine/StateMachineGraph")]
	public class StateMachineGraph : ScriptableObject
	{
		public List<SerializableKeyValuePair<string, StateNode>> Nodes = new();
		public string EntryNodeId;
		
		private Dictionary<string, StateNode> _nodeDictionary;
		private StateNode _entryNode;
		private StateNode _currentNode;

		public void Initialize()
		{
			CacheDictionaries();
			InitializeStateNodes();
			
			if (string.IsNullOrEmpty(EntryNodeId))
			{
				Debug.LogError("Entry node id is null or empty.", this);
				return;
			}
			
			_currentNode = _nodeDictionary[EntryNodeId];

			SubscribeToNode(_currentNode);
			_currentNode.Enter();
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
		
		private void CacheDictionaries()
		{
			_nodeDictionary = GetNodeDictionary();
		}

		private void InitializeStateNodes()
		{
			foreach (var kvp in _nodeDictionary)
			{
				kvp.Value.Initialize();
			}
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
