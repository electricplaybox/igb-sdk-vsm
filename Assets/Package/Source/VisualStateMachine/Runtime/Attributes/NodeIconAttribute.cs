using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeIconAttribute : Attribute
	{
		public string Path { get; private set; }
		public float Opacity { get; private set; }
		public ResourceSource Source { get; private set; }

		public NodeIconAttribute(string path, float opacity = 1f, ResourceSource source = ResourceSource.Resources)
		{
			Source = source;
			Path = path;
			Opacity = opacity;
		}
	}
}