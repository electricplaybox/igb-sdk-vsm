using System;
using UnityEngine;
using VisualStateMachine.Editor;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeIconAttribute : Attribute
	{
		public string Path { get; private set; }
		public ResourceSource Source { get; private set; }

		public NodeIconAttribute(string path, ResourceSource source = ResourceSource.Resources)
		{
			Source = source;
			Path = path;
		}
		
		public Texture2D FetchTexture()
		{
			return ImageService.FetchTexture(Path, Source);
		}
	}
}