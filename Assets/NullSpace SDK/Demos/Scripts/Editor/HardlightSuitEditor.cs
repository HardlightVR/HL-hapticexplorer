using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NullSpace.SDK.Demos
{
	[CustomEditor(typeof(HardlightSuit))]
	public class HardlightSuitEditor : Editor
	{
		bool ShowQuickButtons = true;
		public override void OnInspectorGUI()
		{
			HardlightSuit suit = (HardlightSuit)target;

			if (GUILayout.Button("Open Hardlight Setup Tool"))
			{
				HardlightSetupWindow.Init();
			}
			GUILayout.Space(10);

			//EditorGUILayout.LabelField("Quick Access Buttons", EditorStyles.boldLabel);

			ShowQuickButtons = EditorGUILayout.Foldout(ShowQuickButtons, "Quick Access Buttons");
			if (ShowQuickButtons)
			{
				int drawnCount = 0;
				for (int i = 0; i < suit.Definition.SceneReferences.Count; i++)
				{
					if (suit.Definition.SceneReferences[i] != null)
					{
						drawnCount++;
						if (GUILayout.Button(suit.Definition.DefaultOptions[i].ToString()))
						{
							Selection.activeGameObject = suit.Definition.SceneReferences[i].gameObject;
						}
					}
				}
				if (drawnCount < 1)
				{
					EditorGUILayout.LabelField("Configure the SuitBodyColliders in the Hardlight Quick Setup Tool");

				}

			}

			Editor DefinitionEditor = Editor.CreateEditor(suit.Definition);
			DefinitionEditor.OnInspectorGUI();
		}
	} 
}