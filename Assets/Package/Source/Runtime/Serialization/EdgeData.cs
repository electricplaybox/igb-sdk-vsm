using System;

namespace Vsm.Serialization
{
	[Serializable]
	public class EdgeData
	{
		public string OutputNode;
		public string OutputPort;
		public string InputNode;
		public string InputPort;
	}
}