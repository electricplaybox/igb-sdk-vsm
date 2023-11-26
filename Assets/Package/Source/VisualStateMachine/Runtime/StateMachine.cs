using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VisualStateMachine.States;
using VisualStateMachine.Tools;

namespace VisualStateMachine
{
	[Serializable]
	public class GraphViewState
	{
		public Vector3 Position = Vector3.zero;
		public float Scale = 0f;
	}
	
	[CreateAssetMenu(fileName = "StateMachine", menuName = "StateMachine/StateMachine")]
	public class StateMachine : ScriptableObject
	{
		public GraphViewState GraphViewState => _graphViewState ??= new GraphViewState();
		
		public StateMachine Base { get; set; }
		public IReadOnlyCollection<StateNode> Nodes => _nodes;
		
		[SerializeField] 
		private string _entryStateId;

		[SerializeField] 
		private GraphViewState _graphViewState;
		
		[SerializeField]
		private List<StateNode> _nodes = new();
		
		[NonSerialized]
		private Dictionary<string, StateNode> _nodeLookup = new();

		[NonSerialized]
		private StateNode _currentNode;

		public static StateMachine CreateInstance(StateMachine stateMachine)
		{
			var instance = Instantiate(stateMachine);
			instance.name = instance.name + instance.GetInstanceID();
			instance.Base = stateMachine;
			
			return instance;
		}

		public void UpdateGraphViewState(Vector3 position, float scale)
		{
			_graphViewState ??= new GraphViewState();
			_graphViewState.Position = position;
			_graphViewState.Scale = scale;
		}
		
		public void Initialize(StateMachineController controller)
		{
			InitalizeNodes(controller);
			CreateNodeLookupTable();
			
			if(_nodeLookup.Count == 0) throw new Exception($"StateMachine {this.name} has 0 nodes.");
			
			_currentNode = _nodeLookup[_entryStateId];
			if (_currentNode == null) return;
			
			SubscribeToNode(_currentNode);
			_currentNode.Enter();
		}
		
		public void Update()
		{
			_currentNode?.Update();
		}
		
		public void AddEntryNode()
		{
			#if UNITY_EDITOR
			{
				if (!AssetDatabase.Contains(this)) return;
				if (_nodes.Count > 0) return;

				var stateNode = new StateNode(typeof(EntryState));
				stateNode.SetAsEntryNode(true);
			
				_entryStateId = stateNode.Id;
			
				AddNode(stateNode);
			}
			#endif
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
			if (_currentNode == null) return;
			
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
		
		public void RemoveNode(StateNode node)
		{
			if (!_nodes.Contains(node)) return;

			#if UNITY_EDITOR
			{
				var isEntryNode = _entryStateId == node.Id;

				AssetDatabase.RemoveObjectFromAsset(node.State);
				RemoveConnectionsToNode(node.Id);
				
				_nodes.Remove(node);
				Save();
				
				if(isEntryNode) SelectNextEntryNode();
			}
			#endif
		}

		public void RemoveConnectionsToNode(string nodeId)
		{
			foreach (var node in _nodes)
			{
				node.RemoveAll(connection => connection.ToNodeId == nodeId);
			}
		}

		public void RemoveConnection(string fromNodeId, string toNodeId)
		{
			var fromNode = _nodes.FirstOrDefault(node => node.Id == fromNodeId);
			if (fromNode == null)
			{
				Debug.LogError($"Attempting to remove connection between node {fromNodeId} to {toNodeId} failed as the fromNode is not present");
			}
			
			fromNode.RemoveAll(connection => connection.ToNodeId == toNodeId);
		}

		public void SetEntryNode(StateNode node)
		{
			if (!_nodes.Contains(node)) throw new Exception("Node is not part of this state machine");
			
			SetEntryNodeId(node.Id);
		}

		private void SetEntryNodeId(string entryNodeId)
		{
			_entryStateId = entryNodeId;

			foreach (var node in _nodes)
			{
				node.SetAsEntryNode(node.Id == entryNodeId);
			}
			
			Save();
		}

		public void Save()
		{
			#if UNITY_EDITOR
			{
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssetIfDirty(this);
				Debug.Log($"StateMachine.Save: {this.name}, {_nodes.Count}, {_nodes.Sum(node => node.Connections.Count)}");
			}
			#endif
		}
		
		private void SelectNextEntryNode()
		{
			_entryStateId = _nodes.Count > 0 ? _nodes[0].Id : null;
			Save();
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
				Save();
			}
			#endif
		}
		
		#endregion
		
	}
}