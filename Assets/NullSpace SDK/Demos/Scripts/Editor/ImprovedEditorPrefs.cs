using UnityEditor;
using UnityEngine;
using System.Collections;

public class ImprovedEditorPrefs : MonoBehaviour
{
	public static bool GetBool(string key, bool DefaultValue = false)
	{
		if (EditorPrefs.HasKey(key))
		{ return EditorPrefs.GetBool(key); }
		return DefaultValue;
	}

	public static void BetterDelete(string key)
	{
		if (EditorPrefs.HasKey(key))
		{ EditorPrefs.DeleteKey(key); }
	}
}
