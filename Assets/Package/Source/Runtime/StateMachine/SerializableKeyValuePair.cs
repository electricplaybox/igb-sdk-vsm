using System;

namespace Package.Source.Runtime.StateMachine
{
	[Serializable]
	public class SerializableKeyValuePair<TKey, TValue>
	{
		public TKey Key;
		public TValue Value;

		public SerializableKeyValuePair() { }

		public SerializableKeyValuePair(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}
	}
}