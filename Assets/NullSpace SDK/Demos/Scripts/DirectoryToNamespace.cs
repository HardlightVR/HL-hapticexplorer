using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public static class DirectoryToNamespace
{
	//I would like to apologize for the presence of this class
	//It was a short term implementation. Hopefully you never see it.
	//Add your directory to this class for support in the Haptic Library demo scene.

	//Limitation: Windows is wonky about capitalization. I recommend making all package namespaces all lower case with no special characters

	public static string GetNameSpace(string path)
	{
		string pattern = @"""package""\s?:\s?""([\w|.]+)""";

		string text = File.ReadAllText(path + "/config.json");
		Regex matcher = new Regex(pattern, RegexOptions.IgnoreCase);

		Match m = matcher.Match(text);

		return m.Groups[1].ToString() + ".";
	}
}
