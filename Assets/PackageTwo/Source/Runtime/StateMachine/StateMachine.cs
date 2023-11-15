using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
	[CreateAssetMenu(fileName = "StateMachine", menuName = "StateMachine/StateMachine")]
	public class StateMachine : ScriptableObject
	{
		public StateMachine Base { get; set; }
		
		[SerializeField] 
		private State _entryState;
		
		[SerializeField]
		private List<StateNode> _nodes = new();
		
		[NonSerialized]
		private Dictionary<string, StateNode> _nodeLookup = new();

		[NonSerialized]
		private StateNode _currentNode;

		private bool _currentNodeIsNull;

		public static StateMachine CreateInstance(StateMachine stateMachine)
		{
			var instance = Instantiate(stateMachine);
			instance.name = instance.name + instance.GetInstanceID();
			instance.Base = stateMachine;
			
			return instance;
		}
		
		public void Initialize(StateMachineController controller)
		{
			InitalizeNodes(controller);
			CreateNodeLookupTable();
			
			if(_nodeLookup.Count == 0) throw new Exception($"StateMachine {this.name} has 0 nodes.");
			
			_currentNode = _nodeLookup[_entryState.name];
			_currentNodeIsNull = _currentNode == null;
			
			SubscribeToNode(_currentNode);
			_currentNode.Enter();
		}

		private void InitalizeNodes(StateMachineController controller)
		{
			foreach (var node in _nodes)
			{
				node.Initialize(controller);
			}
		}

		private void CreateNodeLookupTable()
		{
			_nodeLookup.Clear();

			foreach (var node in _nodes)
			{
				_nodeLookup.Add(node.Id, node);
			}
		}

		public void Update()
		{
			if (_currentNodeIsNull) return;
			
			_currentNode.Update();
		}
		
		#region StateMachine Management
		
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

		private void OnDestroy()
		{
			if (_currentNode == null) return;
			
			Unsubscribe(_currentNode);
		}

		private void OnTransition(StateConnection connection)
		{
			var nextNode = _nodeLookup[connection.ToNodeId];
			
			Unsubscribe(_currentNode);
			_currentNode.Exit();
			
			_currentNode = nextNode;
			_currentNodeIsNull = _currentNode == null;
			
			SubscribeToNode(_currentNode);
			_currentNode.Enter();
		}
		
		#endregion

		#region Node Management
		
		public void AddNode(StateNode node)
		{
			_nodes.Add(node);
			
			#if UNITY_EDITOR
			{
				AssetDatabase.AddObjectToAsset(node.State, this);
				AssetDatabase.SaveAssets();
			}
			#endif
		}

		public void SetEntryNode(StateNode node)
		{
			if (!_nodes.Contains(node)) throw new Exception("Node is not part of this state machine");
			
			_entryState = node.State;
		}

		public void RemoveNode(StateNode node)
		{
			if (!_nodes.Contains(node)) return;

			#if UNITY_EDITOR
			{
				var isEntryNode = _entryState == node.State;
				
				AssetDatabase.RemoveObjectFromAsset(node.State);
				_nodes.Remove(node);

				if(isEntryNode) SelectNextEntryNode();
				
				AssetDatabase.SaveAssets();
			}
			#endif
			
			_nodes.Remove(node);
		}

		private void SelectNextEntryNode()
		{
			_entryState = _nodes.Count > 0 ? _nodes[0].State : null;
		}

		public void RemoveAllNodes()
		{
			#if UNITY_EDITOR
			{
				foreach (var node in _nodes)
				{
					AssetDatabase.RemoveObjectFromAsset(node.State);
				}
				
				_nodes.Clear();
				AssetDatabase.SaveAssets();
			}
			#endif
		}
		
		#endregion
	}
}