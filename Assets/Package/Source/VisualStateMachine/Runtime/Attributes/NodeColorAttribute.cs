using System;

namespace VisualStateMachine.Attributes
{
	public static class NodeColor
	{
		public const string Red = "#990e23";
		public const string Orange = "#915710";
		public const string LimeGreen = "#6d9111";
		public const string Green = "#00ff00";
		public const string Teal = "#08704a";
		public const string LightBlue = "#086d70";
		public const string Blue = "#084870";
		public const string Purple = "#4a0e99";
		public const string Violet = "#740e99";
		public const string Pink = "#990e97";
	}
	
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeColorAttribute : Attribute
	{
		public string HexColor { get; private set; }
		
		public NodeColorAttribute(string hexColor)
		{
			HexColor = hexColor;
		}
	}
}