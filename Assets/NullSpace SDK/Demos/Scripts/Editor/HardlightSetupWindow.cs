using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace NullSpace.SDK.Demos
{
	public class HardlightSetupWindow : EditorWindow
	{
		public static HardlightSetupWindow myWindow;

		public Rect windowRect = new Rect(100, 100, 200, 200);

		[SerializeField]
		public List<SuitConfiguration> Suits;

		[SerializeField]
		public class SuitConfiguration
		{
			class HelpMessage
			{
				public HelpMessage(string message, MessageType messageType)
				{
					this.message = message;
					this.messageType = messageType;
				}
				public string message;
				public MessageType messageType;
			}

			string SuitName = "Player Body";
			public GameObject SuitRoot;

			[SerializeField]
			public List<AreaFlag> DefaultOptions;

			//The Game Objects to fill the fields (which will get suit body references
			[SerializeField]
			public List<GameObject> SuitHolders;

			//the objects added. Will get a nice button list to quick get to each of them.
			[SerializeField]
			public List<SuitBodyCollider> SceneReferences;

			public Vector2 scrollPos;
			public Vector2 errorScrollPos;
			bool TopFoldout = true;
			bool ObjectFieldFoldout = true;
			List<HelpMessage> outputMessages;

			public int HapticsLayer = 31;
			public bool CanChangeValues = true;
			public bool AddChildObjects = true;
			public bool AddExclusiveTriggerCollider = true;

			public SuitConfiguration()
			{
				#region Setup Default Options
				if (DefaultOptions == null || DefaultOptions.Count == 0)
				{
					DefaultOptions = new List<AreaFlag>();

					DefaultOptions.Add(AreaFlag.Forearm_Left);
					DefaultOptions.Add(AreaFlag.Upper_Arm_Left);

					DefaultOptions.Add(AreaFlag.Shoulder_Left);
					DefaultOptions.Add(AreaFlag.Back_Left);
					DefaultOptions.Add(AreaFlag.Chest_Left);

					DefaultOptions.Add(AreaFlag.Upper_Ab_Left);
					DefaultOptions.Add(AreaFlag.Mid_Ab_Left);
					DefaultOptions.Add(AreaFlag.Lower_Ab_Left);

					DefaultOptions.Add(AreaFlag.Forearm_Right);
					DefaultOptions.Add(AreaFlag.Upper_Arm_Right);

					DefaultOptions.Add(AreaFlag.Shoulder_Right);
					DefaultOptions.Add(AreaFlag.Back_Right);
					DefaultOptions.Add(AreaFlag.Chest_Right);

					DefaultOptions.Add(AreaFlag.Upper_Ab_Right);
					DefaultOptions.Add(AreaFlag.Mid_Ab_Right);
					DefaultOptions.Add(AreaFlag.Lower_Ab_Right);
				}
				#endregion

				#region Game Object Parents
				if (SuitHolders == null || SuitHolders.Count == 0)
				{
					//Debug.Log("Resetting Suit Holders\n");
					SuitHolders = new List<GameObject>();

					for (int i = 0; i < DefaultOptions.Count; i++)
					{
						SuitHolders.Add(null);
					}
				}
				#endregion

				#region Suit Body Colliders
				if (SceneReferences == null || SceneReferences.Count == 0)
				{
					//Debug.Log("Resetting Filled Areas\n");
					SceneReferences = new List<SuitBodyCollider>();

					for (int i = 0; i < DefaultOptions.Count; i++)
					{
						SceneReferences.Add(null);
					}
				}
				#endregion

				outputMessages = new List<HelpMessage>();
			}

			public void OnGUI()
			{
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

				GUIStyle style = new GUIStyle(GUI.skin.button);
				GUILayoutOption[] options = new GUILayoutOption[0];
				string rootName = SuitRoot == null ? "No Root Defined" : SuitRoot.name;
				TopFoldout = GUILayout.Toggle(TopFoldout, "Show Suit: ", style);
				if (TopFoldout)
				{
					//NOTE TO SELF: Add tooltips
					//new GUIContent("Test Float", "Here is a tooltip")

					EditorGUILayout.BeginVertical("Box");
					SuitNaming(options);

					ObjectFieldFoldout = GUILayout.Toggle(ObjectFieldFoldout, "Show Suit Configuration", style);
					if (ObjectFieldFoldout)
					{
						DrawOptions(options);

						DrawAssignmentAndDisplay();
					}

					DrawProcessControls();
				}

				DrawOutputMessages();

				EditorGUILayout.EndScrollView();
			}

			#region Options
			void DrawOptions(GUILayoutOption[] options)
			{
				GUIContent content = new GUIContent();
				float width = EditorGUIUtility.currentViewWidth;
				GUILayoutOption[] innerOptions = { GUILayout.MaxWidth(width / 2 - 10), GUILayout.MinWidth(35) };

				GUIStyle columnStyle = new GUIStyle(EditorStyles.toolbarButton);

				EditorGUILayout.BeginVertical("Box");
				EditorGUILayout.LabelField("Suit Options");
				SuitRootObjectField(options);


				content = new GUIContent("Autoconfigure based on Root Object", "Uses common names to try and establish the different suit body colliders for the suit.");
				bool Result = OperationButton(SuitRoot == null, content);
				if (Result)
				{
					//Check if they have anything assigned.
					//If they do, warn them this will overwrite it.
				}

				EditorGUILayout.BeginHorizontal();

				content = new GUIContent("Add Child Objects", "Create child objects instead of directly adding to the targeted object.");
				AddChildObjects = ToggleButton(!CanChangeValues, AddChildObjects, content, innerOptions);

				content = new GUIContent("Add New Colliders", "Adds SuitBodyCollider objects instead of using existing ones or letting you configure it manually.\nWill add the colliders to child objects if that is also selected.");
				AddExclusiveTriggerCollider = ToggleButton(!CanChangeValues, AddExclusiveTriggerCollider, content, innerOptions);

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}

			void SuitNaming(GUILayoutOption[] options)
			{
				SuitName = EditorGUILayout.TextField("Suit Name", SuitName, options);
			}
			void SuitRootObjectField(GUILayoutOption[] options)
			{
				SuitRoot = EditorGUILayout.ObjectField(SuitRoot, typeof(GameObject), true, options) as GameObject;

				//Disallow Prefabs
				if (SuitRoot != null && PrefabUtility.GetPrefabType(SuitRoot) == PrefabType.Prefab)
				{
					SuitRoot = null;
				}
			}
			bool OperationButton(bool disabledWhenTrue, GUIContent content)
			{
				GUIStyle style = new GUIStyle(GUI.skin.button);
				using (new EditorGUI.DisabledGroupScope(disabledWhenTrue))
				{
					return GUILayout.Button(content);
				}
			}
			bool ToggleButton(bool disabled, bool toggleState, GUIContent content, GUILayoutOption[] options)
			{
				//GUIStyle style = GUI.skin.button;
				GUIStyle style = new GUIStyle(GUI.skin.button);
				//style.fixedWidth = EditorGUIUtility.currentViewWidth / 3;
				using (new EditorGUI.DisabledGroupScope(disabled))
				{
					return EditorGUILayout.Foldout(toggleState, content, style);
				}
			}
			void AddChildObjectsOption(GUILayoutOption[] options)
			{
				GUIStyle style = GUI.skin.button;
				using (new EditorGUI.DisabledGroupScope(CanChangeValues))
				{
					GUIContent content = new GUIContent("Add Child Objects", "Create child objects instead of directly adding to the targeted object.");
					AddChildObjects = EditorGUILayout.Foldout(AddChildObjects, content, style);
				}
			}
			void AddCollidersTriggerOption(GUILayoutOption[] options)
			{
				GUIStyle style = GUI.skin.button;
				using (new EditorGUI.DisabledGroupScope(CanChangeValues))
				{
					AddExclusiveTriggerCollider = EditorGUILayout.Foldout(AddExclusiveTriggerCollider, "Add Colliders if none exist", style);
				}
			}
			#endregion

			#region Core Columns
			void DrawAssignmentAndDisplay()
			{

				float width = EditorGUIUtility.currentViewWidth;
				GUILayoutOption[] innerOptions = { GUILayout.MaxWidth(width / 3 - 10), GUILayout.MinWidth(35) };

				GUIStyle columnStyle = new GUIStyle(EditorStyles.toolbarButton);
				//columnStyle.fixedHeight = 22;


				for (int i = 0; i < DefaultOptions.Count; i++)
				{
					//New Horizontal
					EditorGUILayout.BeginHorizontal("Box");

					EditorGUILayout.BeginHorizontal(columnStyle);
					//Create the default option display
					DisplayAreaFlag(i, innerOptions);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(columnStyle);
					//The object field for selecting the relevant object
					CreateSuitObjectField(i, innerOptions);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(columnStyle);
					//If we have a suitbodycollider for this index
					SuitQuickButton(i, innerOptions);
					EditorGUILayout.EndHorizontal();

					//End Horizontal
					EditorGUILayout.EndHorizontal();

					//GUILayout.Space(24);
				}

			}

			void DisplayAreaFlag(int index, GUILayoutOption[] options)
			{
				//GUILayout.Label("Area Flag Display");
				if (DefaultOptions != null && DefaultOptions.Count > index)
				{
					AreaFlag o = DefaultOptions[index];
					string label = o.ToString();

					//Remove the underscores?

					GUILayout.Label(label, options);
				}
			}

			void CreateSuitObjectField(int index, GUILayoutOption[] options)
			{
				//GUILayout.Label("Suit Object Field " + SuitHolders.Count);
				if (SuitHolders != null && SuitHolders.Count > index)
				{
					GameObject o = SuitHolders[index];
					//GUIContent label = new GUIContent("Intended Parent");
					SuitHolders[index] = EditorGUILayout.ObjectField(o, typeof(GameObject), true, options) as GameObject;

					//Disallow Prefabs
					if (SuitHolders[index] != null && PrefabUtility.GetPrefabType(SuitHolders[index]) == PrefabType.Prefab)
					{
						SuitHolders[index] = null;
					}
				}
			}

			void SuitQuickButton(int index, GUILayoutOption[] options)
			{
				//GUILayout.Label("Suit Quick Button");
				if (SceneReferences != null && SceneReferences.Count > index)
				{
					bool invalidObj = SceneReferences[index] == null;
					string buttonLabel = string.Empty;

					if (invalidObj)
					{
						//Disabling
						EditorGUI.BeginDisabledGroup(invalidObj);
						buttonLabel = "Unassigned";
					}
					else
					{
						buttonLabel = SceneReferences[index].name;

						//Max out the string size?
					}

					//Color this button differently if the AreaFlag doesn't match?
					if (GUILayout.Button(buttonLabel, EditorStyles.toolbarButton, options))
					{
						//Go to that object.
						Selection.activeGameObject = SceneReferences[index].gameObject;
					}

					if (invalidObj)
					{
						//Re-enable
						EditorGUI.EndDisabledGroup();
					}
				}
			}
			#endregion

			#region End Process Controls
			void DrawProcessControls()
			{
				GUILayout.BeginHorizontal();

				SuitSetupOperation();

				SuitRemovalOperation();

				GUILayout.EndHorizontal();

				EditorGUILayout.EndVertical();
			}
			string SuitSetupOperation()
			{
				string output = string.Empty;
				string tooltip = "This will create Suit components on " + (AddChildObjects ? " children of the selected objects" : " the selected objects");
				GUIContent content = new GUIContent("Create SuitBodyColliders", tooltip);
				bool OperationForbidden = CountValidSuitHolders() < 1;
				bool Result = OperationButton(OperationForbidden, content);
				if (Result)
				{
					//If we should add child objects
					if (AddChildObjects)
						output += AddChildNodesForSuit();
					//Then create the component (it will handle the Collider functionalities)
					output += AddComponentForSuit();
				}

				return output;
			}

			void SuitRemovalOperation()
			{
				bool disabled = true;
				if (CountSuitBodyColliders() > 0)
				{
					disabled = false;
				}

				using (new EditorGUI.DisabledGroupScope(disabled))
				{
					bool beginOperation = GUILayout.Button("Remove SuitBodyColliders");

					if (beginOperation)
					{
						ClearSuitBodyColliders();
					}
				}
			}

			int CountValidSuitHolders()
			{
				int validCount = 0;
				if (SuitHolders != null && SuitHolders.Count == DefaultOptions.Count)
				{
					for (int i = 0; i < SuitHolders.Count; i++)
					{
						if (SuitHolders[i] != null)
						{
							validCount++;
						}
					}
				}

				return validCount;
			}
			int CountSuitBodyColliders()
			{
				int validCount = 0;
				if (SceneReferences != null && SceneReferences.Count == DefaultOptions.Count)
				{
					for (int i = 0; i < SceneReferences.Count; i++)
					{
						if (SceneReferences[i] != null)
						{
							validCount++;
						}
					}
				}

				return validCount;
			}

			void DrawOutputMessages()
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Output Messages & Errors");

				bool disabled = true;
				if (outputMessages != null && outputMessages.Count > 0)
				{
					disabled = false;
				}
				using (new EditorGUI.DisabledGroupScope(disabled))
				{

					if (GUILayout.Button("Clear Messages", EditorStyles.toolbarButton))
					{
						outputMessages.Clear();
					}
				}

				GUILayout.EndHorizontal();

				if (outputMessages != null && outputMessages.Count > 0)
				{
					//GUIStyle messageBox = new GUIStyle("Box");
					//messageBox.fixedHeight = 200;

					int boxHeight = Mathf.Clamp(outputMessages.Count * 30, 140, 300);
					GUILayout.BeginVertical("Box", GUILayout.MinHeight(boxHeight));
					errorScrollPos = EditorGUILayout.BeginScrollView(errorScrollPos);
					for (int i = outputMessages.Count - 1; i >= 0; i--)
					{
						EditorGUILayout.HelpBox(outputMessages[i].message, outputMessages[i].messageType);
					}
					EditorGUILayout.EndScrollView();

					//float space = Mathf.Clamp(300 - outputMessages.Count * 30, 0, 300);
					//GUILayout.Space(space);
					EditorGUILayout.EndVertical();
				}
			}
			#endregion

			#region Processing Functions
			void ProcessSuitRoot()
			{

			}

			string ClearSuitBodyColliders()
			{
				string output = string.Empty;
				for (int i = 0; i < SceneReferences.Count; i++)
				{
					if (SceneReferences[i] != null)
					{
						output += "Removing Suit Body Collider from " + SceneReferences[i].name + " which had RegionID: " + DefaultOptions[i].ToString();
					}
				}

				//Yep, we're going through it twice that way we can have a good output info.
				for (int i = 0; i < SceneReferences.Count; i++)
				{
					//If something is on multiple objects, it'll get cleaned up only once.
					if (SceneReferences[i] != null)
					{
						DestroyImmediate(SceneReferences[i]);
					}
				}

				output = "Remove SuitBodyCollider scripts - Operation Finished\n\n" + output + "\n";
				return output;
			}

			string childAppendName = "'s Suit Body Collider";
			string AddChildNodesForSuit()
			{
				string output = string.Empty;
				for (int i = 0; i < SuitHolders.Count; i++)
				{
					if (SuitHolders[i] != null)
					{
						GameObject go = new GameObject();
						go.transform.SetParent(SuitHolders[i].transform);
						go.name = SuitHolders[i].name + childAppendName;
					}
				}
				return output;
			}

			string AddComponentForSuit()
			{
				string output = string.Empty;
				for (int i = 0; i < SuitHolders.Count; i++)
				{
					if (SuitHolders[i] != null)
					{
						output += "Processing " + SuitHolders[i].name + "";
						GameObject targetGO = AddChildObjects ? SuitHolders[i] : SuitHolders[i].transform.FindChild(SuitHolders[i].name + childAppendName).gameObject;

						Collider col = null;
						//Check if it has one already
						SuitBodyCollider suit = targetGO.GetComponent<SuitBodyCollider>();
						if (suit == null)
						{
							//Add one if it doesn't
							suit = targetGO.AddComponent<SuitBodyCollider>();

							if (AddExclusiveTriggerCollider)
							{
								col = AddColliderForSuit(suit);
							}

							output += "\t  Adding Suit Body Collider to " + SuitHolders[i].name + "";
						}

						output += "\t  Adding " + DefaultOptions[i].ToString() + " " + SuitHolders[i].name + "\n";

						//Add this region to it.
						suit.regionID = suit.regionID | DefaultOptions[i];

						//Save the collider if we made one
						if (col != null)
						{ suit.myCollider = col; }

						SceneReferences[i] = suit;


						//Don't let the user change anything until they've deleted these?
						//These functions aren't robust enough yet.
						CanChangeValues = false;
					}
				}

				output = "Creating SuitBodyCollider - Operation Finished\n\n" + output + "\n";
				return output;
			}

			Collider AddColliderForSuit(SuitBodyCollider suit)
			{
				GameObject go = AddChildObjects ? suit.gameObject.transform.FindChild(suit.gameObject.name + childAppendName).gameObject : suit.gameObject;

				Collider col = go.AddComponent<BoxCollider>();
				col.gameObject.layer = HapticsLayer;
				col.isTrigger = true;
				return col;
			}
			#endregion
		}

		#region Init & Setup
		// Add menu item to show this demo.
		[MenuItem("Tools/Hardlight Configure Body")]
		static void Init()
		{
			myWindow = EditorWindow.GetWindow(typeof(HardlightSetupWindow)) as HardlightSetupWindow;
			myWindow.Setup();
		}

		public void Setup()
		{
			Suits = new List<SuitConfiguration>();
			AddSuitConfiguration();
		}
		#endregion

		void CheckIfInvalidSetup()
		{
			if (Suits == null)
			{
				Setup();
			}
		}

		void AddSuitConfiguration()
		{
			Suits.Add(new SuitConfiguration());
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}

		void OnGUI()
		{
			//Make a button that auto looks things up in children of an object
			CheckIfInvalidSetup();

			for (int i = 0; i < Suits.Count; i++)
			{
				Suits[i].OnGUI();
			}

			bool clicked = GUILayout.Button("Add Suit Configuration");

			if (clicked)
			{
				AddSuitConfiguration();
			}

			//Label describing section
			//[TODO]
		}

		void ProcessRootObject()
		{

		}

		//Addcomponent suit

		void DoWindow(int unusedWindowID)
		{
			GUILayout.Button("Hi");
			GUI.DragWindow();
		}
	}
}