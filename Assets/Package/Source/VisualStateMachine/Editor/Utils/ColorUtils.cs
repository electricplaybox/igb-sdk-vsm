using UnityEngine;

namespace VisualStateMachine.Editor.Utils
{
	public class ColorUtils
	{
		public static Color HexToColor(string hex)
		{
			
			if (hex.StartsWith("#")) hex = hex.Substring(1);
			if (hex.Length == 6) hex += "FF";
			
			var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			var a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

			return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
		}
	}
}