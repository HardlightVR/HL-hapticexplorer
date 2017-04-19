using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class NSEditorStyles
{
	public static bool VisualOverhaul = true;
	public static Texture2D ArrowIconRight;
	public static Texture2D ArrowIconDown;
	public static Texture2D toggleInactiveIcon;
	public static Texture2D toggleActiveIcon;
	public static Texture2D PlusIcon;
	public static Texture2D MinusIcon;

	public static void QuickSelectButton(bool disabled, GameObject target, GUIContent content, GUILayoutOption[] options)
	{
		if (disabled)
		{
			//Disabling
			EditorGUI.BeginDisabledGroup(disabled);
			content.text = "Unassigned";
		}
		else
		{
			content.text = target.name;

			//Max out the string size?
		}

		//Color this button differently if the AreaFlag doesn't match?
		if (GUILayout.Button(content, EditorStyles.toolbarButton, options))
		{
			//Go to that object.
			Selection.activeGameObject = target;
		}

		if (disabled)
		{
			//Re-enable
			EditorGUI.EndDisabledGroup();
		}
	}

	public static Texture2D toggleNormalBackground;
	public static Texture2D toggleActiveBackground;

	public static Color activeColor = new Color(.3f, .8f, .4f);
	public static Color inactiveColor = new Color(.7f, .7f, .7f);
	public static Color otherColor = new Color(.4f, .4f, .9f);

	#region Title Label
	public static void DrawTitle(GUIContent content)
	{

	}
	#endregion

	#region LabelField
	public static string TextField(string value, string text, GUILayoutOption[] options)
	{
		return EditorGUILayout.TextField(value, text, options);
	}
	public static string TextField(string value, string text)
	{
		return EditorGUILayout.TextField(value, text);
	}
	#endregion

	public static bool DrawFoldout(bool toggleDropDown, string displayText = "", string tooltip = "")
	{
		//if (VisualOverhaul != true)
		//{
		return EditorGUILayout.Foldout(toggleDropDown, displayText, GetButton());
		//}

		Rect buttonRect = EditorGUILayout.BeginVertical();
		Rect extraRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height + 8);
		if (GUI.Button(extraRect, new GUIContent("", tooltip), GetSmallFoldoutButton()))
		{
			toggleDropDown = (toggleDropDown ? false : true);
		}

		EditorGUILayout.BeginHorizontal();
		if (toggleDropDown)
		{
			GUILayout.Label(((Texture)ArrowIconDown), GetSmallLabelIcon());
		}
		else
		{
			GUILayout.Label(((Texture)ArrowIconRight), GetSmallLabelIcon());
		}
		GUILayout.Space(-14);
		GUILayout.Label(displayText, GetSmallLabel());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		if (toggleDropDown)
		{
			GUILayout.Space(10f);
		}
		return toggleDropDown;
	}

	#region GUILayoutToggle
	public static bool DrawGUILayoutToggle(bool disabled, bool toggleDropDown, string displayText = "", string tooltip = "")
	{
		return DrawGUILayoutToggle(disabled, toggleDropDown, new GUIContent(displayText, tooltip));
	}

	public static bool DrawGUILayoutToggle(bool disabled, bool toggleDropDown, GUIContent content)
	{
		using (new EditorGUI.DisabledGroupScope(disabled))
		{
			return GUILayout.Toggle(toggleDropDown, content, GetButton());
		}
	}

	public static bool DrawGUILayoutToggle(bool disabled, bool toggleDropDown, GUIContent content, GUILayoutOption[] options)
	{
		using (new EditorGUI.DisabledGroupScope(disabled))
		{
			return GUILayout.Toggle(toggleDropDown, content, GetButton(), options);
		}
	}

	public static bool DrawGUILayoutToggle(bool toggleDropDown, string displayText = "", string tooltip = "")
	{
		return DrawGUILayoutToggle(false, toggleDropDown, new GUIContent(displayText, tooltip));
	}
	#endregion

	#region Foldouts with Icons
	public static bool DrawTitleIconFoldout(bool toggleDropDown, string displayText = "", string tooltip = "")
	{
		if (VisualOverhaul != true)
		{
			return EditorGUILayout.Foldout(toggleDropDown, displayText);
		}

		Rect buttonRect = EditorGUILayout.BeginVertical();
		buttonRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height + 12);
		if (GUI.Button(buttonRect, new GUIContent("", tooltip), GetFoldoutButton()))
		{
			toggleDropDown = (toggleDropDown ? false : true);
		}
		GUILayout.Space(5f);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(-5f);
		if (toggleDropDown)
		{
			GUILayout.Label(((Texture)ArrowIconDown), GetLargeLabelIcon());
		}
		else
		{
			GUILayout.Label(((Texture)ArrowIconRight), GetLargeLabelIcon());
		}
		GUILayout.Label(displayText, GetLargeLabel());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		GUILayout.Space(6f);

		return toggleDropDown;
	}

	public static bool DrawIconFoldout(bool toggleDropDown, string displayText = "", string tooltip = "")
	{
		if (VisualOverhaul != true)
		{
			return EditorGUILayout.Foldout(toggleDropDown, displayText);
		}

		Rect buttonRect = EditorGUILayout.BeginVertical();
		Rect extraRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height + 8);
		if (GUI.Button(extraRect, new GUIContent("", tooltip), GetSmallFoldoutButton()))
		{
			toggleDropDown = (toggleDropDown ? false : true);
		}

		EditorGUILayout.BeginHorizontal();
		if (toggleDropDown)
		{
			GUILayout.Label(((Texture)ArrowIconDown), GetSmallLabelIcon());
		}
		else
		{
			GUILayout.Label(((Texture)ArrowIconRight), GetSmallLabelIcon());
		}
		GUILayout.Space(-14);
		GUILayout.Label(displayText, GetSmallLabel());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		if (toggleDropDown)
		{
			GUILayout.Space(10f);
		}
		return toggleDropDown;
	}
	#endregion

	public static void DrawBackgroundImage(Texture2D Logo, Material transparentBackground, EditorWindow current)
	{
		//Debug.Log((Logo == null) + "  " + (transparentBackground == null) + "  " + (current == null) + "\n");
		Rect rect = new Rect(50, current.position.height - 600, 600, 600);
		Rect rect2 = new Rect(current.position.width - 164, current.position.height - 164, 128, 128);
		GUIStyle style = new GUIStyle();
		float minWidth = 0;
		float maxWidth = 0;
		GUIContent gcontent = new GUIContent((Texture)Logo);
		style.CalcMinMaxWidth(gcontent, out minWidth, out maxWidth);
		//Debug.Log("Min: " + minWidth + "   Max: " + maxWidth);
		style.stretchWidth = false;
		style.stretchHeight = false;
		style.padding = new RectOffset(0, 0, 0, 0);
		style.alignment = TextAnchor.LowerCenter;
		//EditorGUI.DrawPreviewTexture(rect, (Texture)Logo, (Material)transparentBackground, ScaleMode.ScaleToFit);

		GUI.Label(rect2, (Texture)Logo, style);
		style.alignment = TextAnchor.UpperLeft;
	}

	public static GUILayoutOption[] NColumnsLayoutOptions(int columnCount = 2, float minWidthPerColumn = 35, float spacingAcrossEntireWidth = 10)
	{
		float width = EditorGUIUtility.currentViewWidth;
		GUILayoutOption[] evenSplit = { GUILayout.MaxWidth(width / columnCount - spacingAcrossEntireWidth), GUILayout.MinWidth(minWidthPerColumn) };
		return evenSplit;
	}
	public static GUILayoutOption[] NRowLayoutOptions(int rowCount = 2, float minHeightPerRow = 35, float spacingAcrossEntireHeight = 10)
	{
		float height = EditorGUIUtility.singleLineHeight;
		GUILayoutOption[] evenSplit = { GUILayout.MaxHeight(height / rowCount - spacingAcrossEntireHeight), GUILayout.MaxHeight(minHeightPerRow) };
		return evenSplit;
	}

	public static GUILayoutOption[] CombinineOptions(GUILayoutOption[] firstGroup, GUILayoutOption[] secondGroup)
	{
		return firstGroup.Concat(secondGroup).ToArray();
	}

	public static bool OperationButton(bool disabledWhenTrue, GUIContent content)
	{
		using (new EditorGUI.DisabledGroupScope(disabledWhenTrue))
		{
			return GUILayout.Button(content);
		}
	}

	#region Buttons
	public static bool DrawMinusButton(int size = -1)
	{
		if (VisualOverhaul != true)
		{
			return GUILayout.Button(MinusIcon, GUILayout.ExpandWidth(false));
		}
		if (size > 0)
		{
			GUIStyle tallerToolbar = new GUIStyle(EditorStyles.toolbarButton);
			tallerToolbar.fixedHeight = size;
			tallerToolbar.fixedWidth = size;
			return GUILayout.Button(MinusIcon, tallerToolbar, GUILayout.ExpandWidth(false));
		}
		return GUILayout.Button(MinusIcon, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
	}

	public static bool DrawArrowButton()
	{
		if (VisualOverhaul != true)
		{
			return GUILayout.Button(ArrowIconDown, GUILayout.ExpandWidth(false));
		}
		return GUILayout.Button(ArrowIconDown, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
	}

	public static bool DrawPlusButton()
	{
		if (VisualOverhaul != true)
		{
			return GUILayout.Button(PlusIcon, GUILayout.ExpandWidth(false));
		}
		return GUILayout.Button(PlusIcon, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
	}

	public static void DrawLabel(string labelText, float minWidth = 105)
	{
		GUILayout.Label(labelText, NSEditorStyles.GetSubTitleLabel(), GUILayout.ExpandWidth(false), GUILayout.MinWidth(105));
	}

	/// <summary>
	/// Label Wrapper - takes a Rect to offset padding of the label by. Left/X, Top/Y, Right/Width, Bottom/Height format for the Rect
	/// </summary>
	/// <param name="labelText">Label text.</param>
	/// <param name="offset">Offset.</param>
	/// <param name="minWidth">Minimum width.</param>
	public static void DrawLabel(string labelText, Rect offset, float minWidth = 105)
	{
		GUILayout.Label(labelText, NSEditorStyles.GetSubTitleLabel(offset), GUILayout.ExpandWidth(false), GUILayout.MinWidth(105));
	}

	public static bool DrawButton(Texture image, params GUILayoutOption[] options)
	{
		if (VisualOverhaul != true)
		{
			return GUILayout.Button(image, GUILayout.ExpandWidth(false)); ;
		}

		GUIStyle buttonStyle = EditorStyles.toolbarButton;
		return GUILayout.Button(image, buttonStyle, GUILayout.ExpandWidth(false));
	}

	public static bool DrawButton(string content, params GUILayoutOption[] options)
	{
		if (VisualOverhaul != true)
		{
			GUIStyle oldButtonStyle = EditorStyles.miniButton;
			return GUILayout.Button(content, oldButtonStyle, GUILayout.ExpandWidth(false)); ;
		}
		GUIStyle buttonStyle = EditorStyles.toolbarButton;
		return GUILayout.Button(content, buttonStyle, GUILayout.ExpandWidth(false));
	}

	public static bool DrawButton(GUIContent content, params GUILayoutOption[] options)
	{
		if (VisualOverhaul != true)
		{
			return GUILayout.Button(content, GUILayout.ExpandWidth(false)); ;
		}

		GUIStyle buttonStyle = EditorStyles.toolbarButton;
		return GUILayout.Button(content, buttonStyle, GUILayout.ExpandWidth(false));
	}
	#endregion

	#region Toggle
	public static bool DrawToggle(bool value, params GUILayoutOption[] options)
	{
		if (VisualOverhaul != true)
		{
			return GUILayout.Toggle(value, "", options);
		}
		Rect buttonRect = EditorGUILayout.BeginVertical();

		GUILayout.Space(6f);

		if (GUI.Button(buttonRect, new GUIContent("", ""), GetToggleLabelButton(value)))
		{
			value = (value ? false : true);
		}
		GUIStyle label = new GUIStyle(GetToggleLabel(value));
		if (value)
		{
			GUILayout.Label(((Texture)toggleActiveIcon), label, GUILayout.Width(16));
		}
		else
		{
			GUILayout.Label(((Texture)toggleInactiveIcon), label, GUILayout.Width(16));
		}
		EditorGUILayout.EndVertical();

		return value;
	}

	public static bool DrawToggle(bool value, string text, params GUILayoutOption[] options)
	{
		return DrawToggle(value, new GUIContent(text), options);
	}

	public static void DrawToggleIcon(bool value, GUIStyle label)
	{
		if (value)
		{
			GUILayout.Label(((Texture)toggleActiveIcon), label, GUILayout.Width(16));
		}
		else
		{
			GUILayout.Label(((Texture)toggleInactiveIcon), label, GUILayout.Width(16));
		}
		GUILayout.Space(-4);
	}

	public static bool DrawToggle(bool value, Texture image, params GUILayoutOption[] options)
	{
		if (VisualOverhaul != true)
		{
			return GUILayout.Toggle(value, image, options);
		}

		Rect buttonRect = EditorGUILayout.BeginVertical();

		if (GUI.Button(buttonRect, new GUIContent("", ""), GetToggleLabelButton(value)))
		{
			value = (value ? false : true);
		}
		GUIStyle label = new GUIStyle(GetToggleLabel(value));

		EditorGUILayout.BeginHorizontal();
		DrawToggleIcon(value, label);
		GUILayout.Label(image, label);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		return value;
	}

	public static bool DrawToggle(bool value, GUIContent content, params GUILayoutOption[] options)
	{
		if (VisualOverhaul != true)
		{
			return GUILayout.Toggle(value, content, GetToggleStyle(value), options);
		}

		Rect buttonRect = EditorGUILayout.BeginVertical();

		if (GUI.Button(buttonRect, new GUIContent("", ""), GetToggleLabelButton(value)))
		{
			value = (value ? false : true);
		}
		GUIStyle label = new GUIStyle(GetToggleLabel(value));

		EditorGUILayout.BeginHorizontal();
		DrawToggleIcon(value, label);
		GUILayout.Label(content, label);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		return value;
	}
	#endregion

	#region DrawPopup
	public static int DrawPopup(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(selectedIndex, displayedOptions, GetPopup(), options);
	}

	public static int DrawPopup(int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(selectedIndex, displayedOptions, style, options);
	}

	public static int DrawPopup(int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(selectedIndex, displayedOptions, GetPopup(), options);
	}

	public static int DrawPopup(int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(selectedIndex, displayedOptions, style, options);
	}

	public static int DrawPopup(string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, GetPopup(), options);
	}

	public static int DrawPopup(string label, int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, style, options);
	}

	public static int DrawPopup(GUIContent label, int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, GetPopup(), options);
	}

	public static int DrawPopup(GUIContent label, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, style, options);
	}
	#endregion

	#region EnumPopup
	public static System.Enum DrawEnumPopup(System.Enum selectedIndex, params GUILayoutOption[] options)
	{
		System.Enum thing = EditorGUILayout.EnumPopup(selectedIndex, GetEnumPopup(), options);
		return thing;
	}
	#endregion

	#region GUIStyles
	public static GUIStyle GetTitleLabel()
	{
		GUIStyle titleLabel = new GUIStyle(EditorStyles.largeLabel);
		titleLabel.fontSize = 18;
		return titleLabel;
	}

	public static GUIStyle GetToggleStyle(bool value)
	{
		GUIStyle toggleStyle = new GUIStyle(EditorStyles.toggle);

		if (VisualOverhaul != true)
		{
			return toggleStyle;
		}

		toggleStyle.normal.textColor = inactiveColor;
		toggleStyle.active.textColor = activeColor;
		toggleStyle.focused.textColor = activeColor;
		toggleStyle.focused.textColor = activeColor;

		return toggleStyle;
	}

	//A bit obsolete
	public static GUIStyle GetSubTitleLabel()
	{
		if (VisualOverhaul != true)
		{
			return new GUIStyle(EditorStyles.boldLabel);
		}

		GUIStyle subtitleLabel = new GUIStyle(EditorStyles.boldLabel);

		subtitleLabel.padding = new RectOffset(subtitleLabel.padding.left, subtitleLabel.padding.right, subtitleLabel.padding.top - 4, subtitleLabel.padding.bottom);
		return subtitleLabel;
	}

	public static GUIStyle GetSubTitleLabel(Rect offset)
	{
		if (VisualOverhaul != true)
		{
			return new GUIStyle(EditorStyles.boldLabel);
		}

		GUIStyle subtitleLabel = new GUIStyle(EditorStyles.boldLabel);
		subtitleLabel.padding = new RectOffset(subtitleLabel.padding.left + (int)offset.x, subtitleLabel.padding.right + (int)offset.width, subtitleLabel.padding.top + (int)offset.y, subtitleLabel.padding.bottom + (int)offset.height);
		//subtitleLabel = new GUIStyle(EditorStyles.boldLabel);
		return subtitleLabel;
	}

	public static GUIStyle GetLargeLabel()
	{
		GUIStyle largeLabel = new GUIStyle(EditorStyles.largeLabel);
		largeLabel.fontStyle = FontStyle.Bold;
		return largeLabel;
	}

	public static GUIStyle GetPopup()
	{
		if (VisualOverhaul != true)
		{
			return new GUIStyle(EditorStyles.popup);
		}

		GUIStyle popupStyle = new GUIStyle(EditorStyles.toolbarPopup);
		popupStyle.margin = new RectOffset(popupStyle.margin.left, popupStyle.margin.right, popupStyle.margin.top + 2, popupStyle.margin.bottom);
		return popupStyle;
	}

	public static GUIStyle GetEnumPopup()
	{
		if (VisualOverhaul != true)
		{
			return new GUIStyle(EditorStyles.popup);
		}

		GUIStyle enumPopupStyle = new GUIStyle(EditorStyles.toolbarPopup);
		enumPopupStyle.fixedHeight = 15;
		enumPopupStyle.margin = new RectOffset(enumPopupStyle.margin.left, enumPopupStyle.margin.right, enumPopupStyle.margin.top + 2, enumPopupStyle.margin.bottom);
		return enumPopupStyle;
	}

	public static GUIStyle GetSmallFoldoutButton()
	{
		if (VisualOverhaul != true)
		{
			return new GUIStyle(EditorStyles.foldout);
		}

		GUIStyle smallFoldoutButton = new GUIStyle(EditorStyles.toolbarButton);
		smallFoldoutButton.fixedHeight = 20f;
		return smallFoldoutButton;
	}

	public static GUIStyle GetFoldoutButton()
	{
		if (VisualOverhaul != true)
		{
			return new GUIStyle(EditorStyles.foldout);
		}

		GUIStyle foldoutButton = new GUIStyle(EditorStyles.toolbarButton);
		foldoutButton.fixedHeight = 30f;
		//foldoutButton.fixedWidth = 300;
		return foldoutButton;
	}

	public static GUIStyle GetButton()
	{
		if (VisualOverhaul != true)
		{
			return new GUIStyle(GUI.skin.button);
		}

		GUIStyle fullWidthButton = new GUIStyle(GUI.skin.button);
		//foldoutButton.fixedHeight = 30f;
		//foldoutButton.fixedWidth = 300;
		return fullWidthButton;
	}

	public static GUIStyle GetToggleLabelButton(bool toggled)
	{

		if (VisualOverhaul != true)
		{ }
		return EditorStyles.label;
		//}

		//If we wanted to have custom backgrounds or something for the toggle labels.
		/*
		GUIStyle toggleLabel = new GUIStyle(EditorStyles.label);

		toggleNormalBackground = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/UnityBuild/pale-border-icon.png", typeof(Texture2D));
		toggleActiveBackground = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/UnityBuild/minus-icon.png", typeof(Texture2D));
		toggleLabel.normal.background = toggleNormalBackground;
		toggleLabel.focused.background = new Texture2D(2,2);
		toggleLabel.active.background = toggleActiveBackground;

		return toggleLabel;*/
	}

	public static GUIStyle GetToggleLabel(bool toggled)
	{

		GUIStyle toggleLabel = new GUIStyle(EditorStyles.label);
		if (toggled)
		{
			//Active focused and hover don't appear to work at all. Probably needs to force repaints.
			toggleLabel.normal.textColor = activeColor;
			toggleLabel.active.textColor = otherColor;
			toggleLabel.focused.textColor = otherColor;
			toggleLabel.hover.textColor = otherColor;
		}
		else
		{
			toggleLabel.normal.textColor = inactiveColor;
			toggleLabel.active.textColor = otherColor;
			toggleLabel.focused.textColor = otherColor;
			toggleLabel.hover.textColor = otherColor;
		}
		return toggleLabel;
	}

	public static GUIStyle GetLargeLabelIcon()
	{
		GUIStyle largeLabelIcon = new GUIStyle(EditorStyles.largeLabel);
		largeLabelIcon.padding = new RectOffset(12, 0, 0, 0);
		largeLabelIcon.fontStyle = FontStyle.Bold;
		//if(EditorGUIUtility.isProSkin)
		//	largeLabelIcon.normal.textColor = LightGreen;
		return largeLabelIcon;
	}

	public static GUIStyle GetSmallLabelIcon()
	{
		GUIStyle largeLabelIcon = new GUIStyle(EditorStyles.largeLabel);
		largeLabelIcon.fixedWidth = 36;
		largeLabelIcon.padding = new RectOffset(6, 0, -2, 0);
		largeLabelIcon.fontStyle = FontStyle.Bold;
		if (EditorGUIUtility.isProSkin)
			largeLabelIcon.normal.textColor = Color.white;
		return largeLabelIcon;
	}

	public static GUIStyle GetSmallLabel()
	{
		GUIStyle label = new GUIStyle(EditorStyles.label);
		label.alignment = TextAnchor.LowerLeft;
		return label;
	}

	public static GUIStyle GetColumnStyle()
	{
		return new GUIStyle(EditorStyles.toolbarButton);
	}
	#endregion
}
