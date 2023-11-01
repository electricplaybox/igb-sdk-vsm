using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StateMachine
{
	[CreateAssetMenu(fileName = "StateMachineGraph", menuName = "StateMachine/StateMachineGraph")]
	public class StateMachineGraph : ScriptableObject
	{
		public List<SerializableKeyValuePair<string, StateNode>> Nodes = new();
		public List<SerializableKeyValuePair<string, StatePort>> Ports = new();
		public List<StateConnection> Connections = new();
		public string EntryNodeId;
		
		private Dictionary<string, StateNode> _nodeDictionary;
		private Dictionary<string, StatePort> _portDictionary;
		private StateNode _entryNode;
		private StateNode _currentNode;

		public void Initialize()
		{
			CacheDictionaries();
			InitializeStateNodes();
			BindPorts();

			if (string.IsNullOrEmpty(EntryNodeId))
			{
				Debug.LogError("Entry node id is null or empty.", this);
				return;
			}
			
			_currentNode = _nodeDictionary[EntryNodeId];
			_currentNode.Enter();
		}

		public void OnDestroy()
		{
			UnBindPorts();
		}

		public void Update()
		{
			_currentNode?.Update();
		}
	
		private void CacheDictionaries()
		{
			_nodeDictionary = GetNodeDictionary();
			_portDictionary = GetPortDictionary();
		}

		private void InitializeStateNodes()
		{
			foreach (var kvp in _nodeDictionary)
			{
				kvp.Value.Initialize();
			}
		}

		private void BindPorts()
		{
			foreach (var connection in Connections)
			{
				connection.OnTransition += OnTransition;
			}
		}
		
		private void UnBindPorts()
		{
			foreach (var connection in Connections) 
			{
				connection.OnTransition -= OnTransition;
			}
		}

		private void OnTransition(StateConnection connection)
		{
			var entryPort = _portDictionary[connection.ToPortId];
			var nextNode = _nodeDictionary[entryPort.NodeId];
			
			_currentNode.Exit();
			_currentNode = nextNode;
			_currentNode.Enter();
		}

		private Dictionary<string, StateNode> GetNodeDictionary()
		{
			return Nodes.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		private Dictionary<string, StatePort> GetPortDictionary()
		{
			return Ports.ToDictionary(pair => pair.Key, pair => pair.Value);
		}
	}
}
