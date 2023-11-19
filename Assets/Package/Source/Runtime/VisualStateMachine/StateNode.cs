using System;
using System.Collections.Generic;
using UnityEngine;

namespace VisualStateMachine
{
	[Serializable]
	public class StateNode
	{
		public bool IsEntryNode => _isEntryNode;
		public bool IsActive => _isActive;
		public Vector2 Position => _position;
		public State State => _state;
		public string Id => _id;
		public IReadOnlyList<StateConnection> Connections => _connections;

		[SerializeField] private State _state;
		[SerializeField] private string _id = Guid.NewGuid().ToString();
		[SerializeField] private Vector2 _position;
		[SerializeField] private bool _isEntryNode;
		[SerializeField] private List<StateConnection> _connections = new();

		[NonSerialized]
		private bool _isActive;
		
		public StateNode(Type stateType, StateMachine parent)
		{
			if (!stateType.IsSubclassOf(typeof(State)))
			{
				throw new ArgumentException("The provided type must be a subclass of State.");
			}
			
			_state = ScriptableObject.CreateInstance(stateType) as State;
			_state.name = _id;
		}
		
		public void SetAsEntryNode(bool isEntryNode)
		{
			_isEntryNode = isEntryNode;
		}

		public void SetPosition(Vector2 position)
		{
			_position = position;
		}
		
		public void Initialize(StateMachineController controller)
		{
			_state = _state.Clone(controller);
		}
		
		public void AddConnection(StateConnection stateConnection)
		{
			_connections.Add(stateConnection);
		}
		
		public void Enter()
		{
			ToggleConnectionSubscription(subscribe: true);
			_isActive = true;
			State.Enter();
		}

		public void Update()
		{
			State.Update();
		}

		public void Exit()
		{
			ToggleConnectionSubscription(subscribe: false);
			_isActive = false;
			State.Exit();
		}

		private void ToggleConnectionSubscription(bool subscribe)
		{
			foreach (var connection in _connections) 
			{
				ToggleEventSubscriptionByName(State, connection.FromPortName, connection, subscribe);
			}
		}
		
		private void ToggleEventSubscriptionByName(object targetObject, string eventName, StateConnection connection, bool subscribe)
		{
			var eventInfo = targetObject.GetType().GetEvent(eventName);
			if (eventInfo == null) return;
			
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, connection, nameof(connection.Transition));

			if (subscribe)
			{
				eventInfo.AddEventHandler(targetObject, handler);
			}
			else
			{
				eventInfo.RemoveEventHandler(targetObject, handler);
			}
		}
	}
}