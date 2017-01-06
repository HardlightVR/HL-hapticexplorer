//Credit to Martin Nerukar at Shark Bomb Studios for this code
//http://www.sharkbombs.com/2015/02/17/unity-editor-enum-flags-as-toggle-buttons/
//Used 10/31/2016

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Linq;
using NullSpace.SDK;
[CustomPropertyDrawer(typeof(RegionFlagAttribute))]
public class RegionFlagsAttributeDrawer : PropertyDrawer
{
	public static bool FoldoutOpened = true;
	public static bool ShowMirroredRegions = false;
	public static bool ShowMassRegions = true;
	public static bool ShowDebugButtons = false;
	//Provided by Nick Cellini
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		//Two extra entries for the dev buttons
		int extraEntries = 4;
		return FoldoutOpened ? (CountValidEntries(property) + extraEntries) * 22 : (22 * extraEntries) + 10;
	}

	//This OnGUI was modified by Nick Cellini to stack the buttons 
	//(modified from Martin Nerukar's base code).
	//Further modified by NullSpace VR's Jonathan Palmer
	public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
	{
		int buttonsIntValue = _property.intValue;
		int enumLength = _property.enumNames.Length;
		int buttonCount = CountValidEntries(_property) + 4;
		int bothCount = _property.enumNames.Where(x => x.Contains("Both")).Count();
		bool[] buttonPressed = new bool[enumLength];
		bool[] pressedLastFrame = new bool[enumLength];
		float buttonWidth = EditorGUIUtility.currentViewWidth - (EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth);
		float buttonHeight = _position.height / (buttonCount);

		Rect buttonPos = new Rect(
				_position.x,
				_position.y + (0 * 18),
				buttonWidth,
				20);

		GUIStyle gStyle = new GUIStyle(GUI.skin.button);
		FoldoutOpened = EditorGUI.Toggle(buttonPos, (FoldoutOpened ? "Hide Areas" : "Show Areas"), FoldoutOpened);

		int offset = 0;

		for (int i = 0; i < pressedLastFrame.Length; i++)
		{
			//Construct last frames state based on our previous value
			if (HasFlag(buttonsIntValue, _property.enumNames[i]))
			{
				pressedLastFrame[i] = true;
			}
		}

		#region Foldout
		if (FoldoutOpened)
		{
			#region Toggle Buttons
			buttonPos = new Rect(
						_position.x,
						_position.y + (1 * 18),
						buttonWidth,
						22);

			ShowMirroredRegions = EditorGUI.Toggle(buttonPos, (ShowMirroredRegions ? "Hide Both Regions" : "Show Both Regions"), ShowMirroredRegions);

			buttonPos = new Rect(
					_position.x,
					_position.y + (2 * 18),
					buttonWidth,
					22);
			ShowMassRegions = EditorGUI.Toggle(buttonPos, (ShowMassRegions ? "Hide Mass Regions" : "Show Mass Regions"), ShowMassRegions);
			buttonPos = new Rect(
					_position.x,
					_position.y + (3 * 18),
					buttonWidth,
					22);
			#endregion

			EditorGUI.BeginChangeCheck();
			#region Show Debug IntField
			if (ShowDebugButtons)
			{
				int newValue = EditorGUI.IntField(buttonPos, "Value", _property.intValue);
				if (newValue != _property.intValue)
				{
					_property.intValue = newValue;
					buttonsIntValue = newValue;
				}
			}
			#endregion

			#region Create Button Loop
			for (int i = 0; i < enumLength; i++)
			{
				if (ValidEntry(_property.enumNames[i]))
				{
					AreaFlag flag = (AreaFlag)System.Enum.Parse(typeof(AreaFlag), _property.enumNames[i], true);

					//We use the below one because we want the flag for other uses
					//if (HasFlag(_property, _property.enumNames[i]))
					if (HasFlag(buttonsIntValue, flag))
					{
						if (i == 0 && buttonsIntValue > 0)
						//if (i == 0 && _property.intValue > 0)
						{
							//Debug.Log("Comparing: " + _property.intValue + " to " + (int)flag + "\n");
							buttonPressed[i] = false;
							pressedLastFrame[i] = false;
						}
						else
						{
							buttonPressed[i] = pressedLastFrame[i];
						}
					}

					bool wasPressed = false;
					#region Debugging Buttons
					if (ShowDebugButtons)
					{
						buttonPos = new Rect(
							_position.x + EditorGUIUtility.labelWidth - 80,
							_position.y + ((4 + i - offset) * buttonHeight),
							80,
							buttonHeight - 0);

						wasPressed = buttonPressed[i];
						GUI.Toggle(buttonPos, pressedLastFrame[i], "" + (int)flag, "Button");
					}
					#endregion

					buttonPos = new Rect(
						_position.x + EditorGUIUtility.labelWidth,
						_position.y + ((4 + i - offset) * buttonHeight),
						buttonWidth,
						buttonHeight - 0);

					wasPressed = pressedLastFrame[i];
					buttonPressed[i] = GUI.Toggle(buttonPos, buttonPressed[i], CleanString(_property.enumNames[i]), "Button");
				}
				else
				{
					offset++;
				}
			}
			#endregion

			#region Loop through buttons
			for (int i = buttonPressed.Length - 1; i > 0; i--)
			{
				AreaFlag flag = (AreaFlag)System.Enum.Parse(typeof(AreaFlag), _property.enumNames[i], true);

				//Staying active
				if (buttonPressed[i] && pressedLastFrame[i])
				{
					buttonsIntValue = buttonsIntValue | (int)flag;
				}
				//Adding new entries
				else if (buttonPressed[i] && !pressedLastFrame[i])
				{
					buttonsIntValue = buttonsIntValue | (int)flag;
				}
				//Removing entries
				//We can't nicely remove the mass regions. This is deemed acceptable.
				//The problem is looping through forward always leaves the mass regions to last
				//Looping backward makes it so we can't turn off the child elements nicely.
				//To fix this, you'd want to sort the order you do them and that isn't worth the dev time currently.
				else if (pressedLastFrame[i] && !buttonPressed[i])
				{
					buttonsIntValue = buttonsIntValue & ~(int)flag;
				}
			}
			#endregion

			//None Case
			if (buttonPressed[0] && _property.intValue > 0)
			{
				buttonsIntValue = 0;
			}

			if (EditorGUI.EndChangeCheck())
			{
				_property.intValue = buttonsIntValue;
			}
		}
		#endregion
	}

	private int CountValidEntries(SerializedProperty _property)
	{
		var names = _property.enumNames;
		if (!ShowMassRegions)
		{
			names = names.Where(x => !x.Contains("All")).ToArray();
		}
		if (!ShowMirroredRegions)
		{
			names = names.Where(x => !x.Contains("Both")).ToArray();
		}
		//Debug.Log("Valid entries: " + names.Count() + "\n");
		return names.Count();
	}
	private bool ValidEntry(string entry)
	{
		bool valid = true;
		if (!ShowMassRegions)
		{
			if (entry.Contains("All"))
			{
				valid = false;
			}
		}
		if (!ShowMirroredRegions)
		{
			if (entry.Contains("Both"))
			{
				valid = false;
			}
		}
		return valid;
	}

	private AreaFlag ConvertFlag(string flagName)
	{
		return (AreaFlag)System.Enum.Parse(typeof(AreaFlag), flagName, true); ;
	}
	private bool HasFlag(int baseFlag, int flag)
	{
		if ((baseFlag & (flag)) == flag)
		{
			return true;
		}
		return false;
	}
	private bool HasFlag(int baseFlag, AreaFlag flag)
	{
		return HasFlag(baseFlag, (int)flag);
	}
	private bool HasFlag(int baseFlag, string flagName)
	{
		AreaFlag flag = ConvertFlag(flagName);

		return HasFlag(baseFlag, flag);
	}

	private string CleanString(string cleanedString)
	{
		//			Remove tiny faces from the enum, they have no place here.
		cleanedString = cleanedString.Replace('_', ' ');

		return cleanedString;
	}
}
#endif
