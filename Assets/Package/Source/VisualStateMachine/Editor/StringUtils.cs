using System.Globalization;
using System.Text.RegularExpressions;

namespace VisualStateMachine.Editor
{
	public class StringUtils
	{
		public static string PascalCaseToTitleCase(string pascalCaseString)
		{
			var withSpaces = Regex.Replace(pascalCaseString, "(\\B[A-Z])", " $1");
			var textInfo = CultureInfo.CurrentCulture.TextInfo;
			
			return textInfo.ToTitleCase(withSpaces);
		}
	}
}