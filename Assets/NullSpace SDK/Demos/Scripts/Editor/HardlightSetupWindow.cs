using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace NullSpace.SDK.Demos
{
	public class HardlightSetupWindow : EditorWindow
	{
		public static HardlightSetupWindow myWindow;

		public Rect windowRect = new Rect(100, 100, 200, 200);

		bool QuickButtonFoldout = true;
		Vector2 scrollPos;

		[SerializeField]
		public List<EditorSuitConfig> Suits;

		[SerializeField]
		public class EditorSuitConfig
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

			public List<Object> ObjectsToDestroy;

			//public Vector2 scrollPos;
			public Vector2 errorScrollPos;
			bool TopFoldout = true;
			bool ObjectFieldFoldout = true;
			List<HelpMessage> outputMessages;

			public int HapticsLayer = 31;
			public bool CanChangeValues = true;
			public bool AddChildObjects = true;
			public bool AddExclusiveTriggerCollider = true;

			private int ComponentsToDestroy;
			private int GameObjectsToDestroy;
			public EditorSuitConfig()
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
				ObjectsToDestroy = new List<Object>();
			}

			public void OnGUI()
			{
				//scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

				GUIStyle style = new GUIStyle(GUI.skin.button);
				GUILayoutOption[] options = new GUILayoutOption[0];
				string suitDisplayName = SuitName.Length > 0 ? SuitName : SuitRoot == null ? "Unnamed Suit" : SuitRoot.name;
				TopFoldout = GUILayout.Toggle(TopFoldout, "Show Suit: " + suitDisplayName, style);
				#region Top Foldout
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
						GUILayout.Space(12);

						DrawAssignmentAndDisplay();
					}

					DrawProcessControls();

					DrawOutputMessages();
				}
				#endregion


				//EditorGUILayout.EndScrollView();
			}

			#region Options
			void DrawOptions(GUILayoutOption[] options)
			{
				GUIContent content = new GUIContent();
				float width = EditorGUIUtility.currentViewWidth;
				GUILayoutOption[] innerOptions = { GUILayout.MaxWidth(width / 2 - 10), GUILayout.MinWidth(35) };

				GUIStyle columnStyle = new GUIStyle(EditorStyles.toolbarButton);

				EditorGUILayout.BeginVertical("Box");
				#region Suit Auto-Configuration
				//EditorGUILayout.LabelField("Suit Options");
				SuitRootObjectField(options);

				content = new GUIContent("Autoconfigure based on Root Object", "Uses common names to try and establish the different suit body colliders for the suit.");
				bool Result = OperationButton(SuitRoot == null, content);
				if (Result)
				{
					DefinedSuit definition = SuitRoot.GetComponent<DefinedSuit>();

					if (definition != null)
					{
						PopulateFromSuitDefinition(definition);
					}
					else
					{
						//	//Check if they have anything assigned.
						//	//If they do, warn them this will overwrite it.
						AutoFindElementsFromRoot(SuitRoot);
					}
				}
				#endregion

				//content = new GUIContent("Can Change Values", "Can't be adjusted if you have a current layout. Remove SuitBodyColliders to adjust config.");
				//CanChangeValues = CreateStyledToggle(false, CanChangeValues, content, innerOptions);

				EditorGUILayout.BeginHorizontal();
				content = new GUIContent("Add Child Objects", "Create child objects instead of directly adding to the targeted object.");
				AddChildObjects = CreateStyledToggle(!CanChangeValues, AddChildObjects, content, innerOptions);

				content = new GUIContent("Add New Colliders", "Adds SuitBodyCollider objects instead of using existing ones or letting you configure it manually.\nWill add the colliders to child objects if that is also selected.");
				AddExclusiveTriggerCollider = CreateStyledToggle(!CanChangeValues, AddExclusiveTriggerCollider, content, innerOptions);

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}

			void SuitNaming(GUILayoutOption[] options)
			{
				SuitName = EditorGUILayout.TextField("Suit Name", SuitName, options);
			}
			void SuitRootObjectField(GUILayoutOption[] options)
			{
				GUIContent content = new GUIContent("Suit Root (Optional)", "For modifying an existing configuration. In the future this will try to find the related objects based on common naming conventions.");

				SuitRoot = EditorGUILayout.ObjectField(content, SuitRoot, typeof(GameObject), true, options) as GameObject;

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
			bool CreateStyledToggle(bool disabled, bool toggleState, GUIContent content, GUILayoutOption[] options)
			{
				GUIStyle style = new GUIStyle(GUI.skin.button);
				//GUIStyle style = new GUIStyle(GUI.skin.button);
				//style.fixedWidth = EditorGUIUtility.currentViewWidth / 3;
				using (new EditorGUI.DisabledGroupScope(disabled))
				{
					bool result = GUILayout.Toggle(toggleState, content, style);
					return result;
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

					//If the value changes
					if (SuitHolders[index] != null && o != SuitHolders[index])
					{
						SuitBodyCollider suit = LookupSceneReference(index);
						AssignQuickButton(suit, index);
					}
				}
			}

			//When we change the field. Lookup that object and assign the quickbutton if it can
			SuitBodyCollider LookupSceneReference(int index)
			{
				SuitBodyCollider suit = null;
				GameObject targObj = SuitHolders[index];
				if (SuitHolders[index] != null)
				{
					string lookupName = SuitHolders[index].name + childAppendName;
					Transform objToCheck = AddChildObjects ? targObj.transform.FindChild(lookupName) : targObj.transform;

					if (objToCheck != null)
					{
						return objToCheck.gameObject.GetComponent<SuitBodyCollider>();
					}
				}
				return null;
			}
			bool AssignQuickButton(SuitBodyCollider suit, int index)
			{
				if (suit != null && SceneReferences[index] == null)
				{
					SceneReferences[index] = suit;
					return true;
				}
				return false;
			}

			void SuitQuickButton(int index, GUILayoutOption[] options)
			{
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
				string output = string.Empty;
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
						output += DetectComponentsToRemove();
						Debug.Log(output + "\n");
						if (ObjectsToDestroy.Count > 0)
						{
							string dialogText = GameObjectsToDestroy + " game objects marked to be destroyed\n" + ComponentsToDestroy + " components marked to be removed.";
							bool userResult = EditorUtility.DisplayDialog("Delete Component Objects", dialogText, "Remove", "Cancel");
							if (userResult)
							{
								DeleteMarkedObjects();
								CanChangeValues = true;
							}
							//ClearSuitBodyColliders();
						}
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
			void AutoFindElementsFromRoot(GameObject Root)
			{
				List<SuitBodyCollider> suitObjects = Root.GetComponentsInChildren<SuitBodyCollider>().ToList();

				if (suitObjects.Count > 0)
				{
					//for (int i = 0; i < suitObjects.Count; i++)
					//{
					//if (suitObjects[i].regionID.HasFlag(DefaultOptions[i]))
					//{
					//	AssignQuickButton(suitObjects[i], i);
					//}
					//}

					for (int optionIndex = 0; optionIndex < DefaultOptions.Count; optionIndex++)
					{
						for (int suitIndex = 0; suitIndex < suitObjects.Count; suitIndex++)
						{
							if (suitObjects[suitIndex].regionID.HasFlag(DefaultOptions[optionIndex]))
							{
								//Set the object field
								if (AddChildObjects && suitObjects[suitIndex].transform.parent != null)
								{
									SuitHolders[optionIndex] = suitObjects[suitIndex].transform.parent.gameObject;
								}
								if (!AddChildObjects)
								{
									SuitHolders[optionIndex] = suitObjects[suitIndex].gameObject;
								}

								//Assign the quick button.
								AssignQuickButton(suitObjects[suitIndex], optionIndex);
							}
						}

					}

					//Index out of bounds - gotta search another way
					//for (int i = 0; i < DefaultOptions.Count; i++)
					//{
					//	if (suitObjects[i].regionID.HasFlag(DefaultOptions[i]))
					//	{
					//		AssignQuickButton(suitObjects[i], i);
					//	}
					//}
				}
			}

			public void PopulateFromSuitDefinition(DefinedSuit definition)
			{
				if (definition != null)
				{
					DefaultOptions = definition.MySuit.DefaultOptions.ToList();
					SuitHolders = definition.MySuit.SuitHolders.ToList();
					SceneReferences = definition.MySuit.SceneReferences.ToList();
					HapticsLayer = definition.MySuit.HapticsLayer;
					//We can't change stuff, we imported it
					CanChangeValues = false;
					AddChildObjects = definition.MySuit.AddChildObjects;
					AddExclusiveTriggerCollider = definition.MySuit.AddExclusiveTriggerCollider;
				}
			}

			//This function is for AUTOMATICALLY 
			void ProcessSuitRoot()
			{

			}

			void DeleteMarkedObjects()
			{
				for (int i = ObjectsToDestroy.Count - 1; i > -1; i--)
				{
					if (ObjectsToDestroy[i] != null)
					{
						DestroyImmediate(ObjectsToDestroy[i]);
					}
				}
				ObjectsToDestroy.Clear();
			}

			string ClearSuitBodyColliders()
			{
				Debug.LogError("This is not used\nYou probably shouldn't use this - it was a temporary call and was refactored into RemoveComponentsForSuit\n");
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
						Undo.RecordObject(go, "Add Suit Child Node");

						go.transform.SetParent(SuitHolders[i].transform);
						go.name = SuitHolders[i].name + childAppendName;
					}
				}
				return output;
			}

			string AddComponentForSuit()
			{
				string output = string.Empty;

				DefinedSuit suitDef = null;
				if (SuitRoot != null)
				{
					suitDef = SuitRoot.GetComponent<DefinedSuit>();
					if (suitDef == null)
					{
						SuitRoot.AddComponent<DefinedSuit>();
					}
				}

				if (suitDef != null)
				{
					suitDef.MySuit.AddChildObjects = AddChildObjects;
					suitDef.MySuit.AddExclusiveTriggerCollider = AddExclusiveTriggerCollider;
					suitDef.MySuit.SuitHolders = SuitHolders.ToList();
					suitDef.MySuit.DefaultOptions = DefaultOptions.ToList();
					suitDef.MySuit.SuitRoot = SuitRoot;
				}

				for (int i = 0; i < SuitHolders.Count; i++)
				{
					if (SuitHolders[i] != null)
					{
						Undo.RecordObject(SuitHolders[i], "Add Suit Node to Marked Objects");

						output += "Processing " + SuitHolders[i].name + "";
						GameObject targetGO = AddChildObjects ? SuitHolders[i].transform.FindChild(SuitHolders[i].name + childAppendName).gameObject : SuitHolders[i];

						Collider col = null;
						//Check if it has one already
						SuitBodyCollider suit = targetGO.GetComponent<SuitBodyCollider>();
						if (suit == null)
						{
							//Add one if it doesn't
							//suit = targetGO.AddComponent<SuitBodyCollider>(); - Not undo-able

							suit = Undo.AddComponent<SuitBodyCollider>(targetGO);

							if (AddExclusiveTriggerCollider)
							{
								col = AddColliderForSuit(suit);
							}

							if (suitDef != null)
							{
								suitDef.MySuit.SceneReferences[i] = suit;
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

			string DetectComponentsToRemove()
			{
				string output = string.Empty;

				//We want to clear out the list
				ObjectsToDestroy.Clear();

				ComponentsToDestroy = 0;
				GameObjectsToDestroy = 0;

				if (SceneReferences != null)
				{
					for (int i = 0; i < SceneReferences.Count; i++)
					{
						if (SceneReferences[i] != null)
						{
							//If add child objects
							if (AddChildObjects)
							{
								int componentCount = SceneReferences[i].gameObject.GetComponents<Component>().Length;

								//Debug.Log("Component Count is equal to " + componentCount + "\n");

								if (componentCount < 4)
								{
									ComponentsToDestroy++;
									if (AddExclusiveTriggerCollider)
										ComponentsToDestroy++;
									GameObjectsToDestroy++;
									//Mark for deletion - they'll have to confirm removal.
									ObjectsToDestroy.Add(SceneReferences[i].gameObject);
									output += "Marking : " + SceneReferences[i].gameObject + "'s SuitBodyCollider Component for removal - has " + componentCount + " components\n";
								}
							}
							//Attached to the parrent
							else
							{
								if (SuitHolders[i] != null)
								{
									SuitBodyCollider suit = SuitHolders[i].GetComponent<SuitBodyCollider>();

									if (suit != null)
									{
										ComponentsToDestroy++;
										ObjectsToDestroy.Add(suit);
										output += "Marking : " + SuitHolders[i].name + "'s SuitBodyCollider Component for removal\n";
									}

									if (AddExclusiveTriggerCollider && suit.myCollider != null)
									{
										ComponentsToDestroy++;
										ObjectsToDestroy.Add(suit.myCollider);
										output += "Marking : " + SuitHolders[i] + "'s " + suit.myCollider.GetType().ToString() + " Component for removal\n";
									}
								}
							}
						}
					}
				}

				output += "Operation Finished - " + ObjectsToDestroy.Count + " objects marked to destory\n";

				return output;
			}

			Collider AddColliderForSuit(SuitBodyCollider suit)
			{
				GameObject go = suit.gameObject;

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
			myWindow = GetWindow(typeof(HardlightSetupWindow)) as HardlightSetupWindow;
			myWindow.Setup();
		}

		public void Setup()
		{
			Suits = new List<EditorSuitConfig>();

			List<DefinedSuit> existingDefinitions = FindObjectsOfType<DefinedSuit>().ToList();

			EditorSuitConfig suit = null;

			for (int i = 0; i < existingDefinitions.Count; i++)
			{
				suit = AddSuitConfiguration();
				suit.PopulateFromSuitDefinition(existingDefinitions[i]);
			}

			if (Suits.Count == 0)
			{
				suit = AddSuitConfiguration();
			}
		}
		#endregion

		void CheckIfInvalidSetup()
		{
			if (Suits == null)
			{
				Setup();
			}
		}

		EditorSuitConfig AddSuitConfiguration()
		{
			EditorSuitConfig suit = new EditorSuitConfig();
			Suits.Add(suit);
			return suit;
		}

		void DrawQuickButtonsForSuitBodyColliders()
		{
			GUIStyle style = new GUIStyle(GUI.skin.button);
			GUILayoutOption[] options = new GUILayoutOption[0];
			float width = EditorGUIUtility.currentViewWidth;
			GUILayoutOption[] innerOptions = { GUILayout.MaxWidth(width / 3), GUILayout.MinWidth(35) };
			GUIContent content = new GUIContent(string.Empty);
			//Toggle to show a list of all the suits
			List<SuitBodyCollider> suitObjects = new List<SuitBodyCollider>();

			suitObjects = FindObjectsOfType<SuitBodyCollider>().ToList();

			if (suitObjects.Count > 0)
			{
				if (QuickButtonFoldout)
				{
					EditorGUILayout.BeginVertical("box");
				}
				string showQuickButtonName = QuickButtonFoldout ? "Hide" : "Show";
				QuickButtonFoldout = GUILayout.Toggle(QuickButtonFoldout, showQuickButtonName + " Existing Suit Body Colliders", style);
				if (QuickButtonFoldout)
				{
					bool horizOpen = false;
					for (int i = 0; i < suitObjects.Count; i++)
					{
						if (i % 3 == 0)
						{
							if (horizOpen) EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
							horizOpen = true;
						}
						if (suitObjects[i] != null)
						{
							content = new GUIContent(suitObjects[i].name, "Quick Navigate to " + suitObjects[i].name);
							//Create a select button
							NullSpaceEditorStyles.QuickSelectButton(false, suitObjects[i].gameObject, content, innerOptions);
						}
					}
					if (horizOpen) EditorGUILayout.EndHorizontal();
				}
				if (QuickButtonFoldout)
				{
					EditorGUILayout.EndVertical();
				}
			}
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}

		void OnGUI()
		{
			//Make a button that auto looks things up in children of an object
			CheckIfInvalidSetup();

			//EditorGUILayout.InspectorTitlebar(true, this, true);
			//GUILayoutOption[] innerOptions = { GUILayout.MaxHeight(45), GUILayout.MinHeight(35) };
			GUIStyle title = new GUIStyle();
			title.fontSize = 18;
			GUILayout.Label(" Hardlight Quick Setup Tool", title);

			bool allowExpandAll = Suits != null && Suits.Count > 1;
			GUIContent content = new GUIContent("Collapsed All");
			bool result = NullSpaceEditorStyles.OperationButton(!allowExpandAll, content);

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			for (int i = 0; i < Suits.Count; i++)
			{
				Suits[i].OnGUI();
			}

			EditorGUILayout.EndScrollView();

			DrawQuickButtonsForSuitBodyColliders();


			bool clicked = GUILayout.Button("Add Suit Configuration");

			if (clicked)
			{
				AddSuitConfiguration();
			}

			//Label describing section
			//[TODO]

		}

		#region Functions for body root processing
		//The feature target here would be a couple click process
		//Step 1: Assign the root node
		//Step 2: Set to AutoConfigure based on root object.
		//		A function recursively searches the child objects.
		//		For each SuitHolder that is desired, it creates a list of Transforms sorted by confidence levels.
		//		The user can then customize and select options for which spots get the body colliders

		void ProcessRootObject()
		{

		}

		public class LookupObject
		{

		}
		//Record confidence? Highlight the poor matches
		//Green/good for the ones that we light
		//Look at UE4/Unity's Mecanim default rigs - this will be a common naming convention
		//Mixamo's rig setup
		//Note: Spines might be lettered or numbered or vague
		//List<Transform> RecursiveSearchForRelevantObjects(Transform root)
		//{

		//}
		#endregion

		void DoWindow(int unusedWindowID)
		{
			GUILayout.Button("Hi");
			GUI.DragWindow();
		}

		#region Editor Saving
		private void OnEnable()
		{
			QuickButtonFoldout = ImprovedEditorPrefs.GetBool("NS-QuickButton", QuickButtonFoldout);
			//Debug.Log("Looking for NS-QuickButton - " + EditorPrefs.HasKey("NS-QuickButton") + "   \n" + EditorPrefs.GetBool("NS-QuickButton"));
		}

		private void OnDisable()
		{
			EditorPrefs.SetBool("NS-QuickButton", QuickButtonFoldout);
		}
		#endregion

	}

	public static class NullSpaceEditorStyles
	{
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

		public static bool OperationButton(bool disabledWhenTrue, GUIContent content)
		{
			GUIStyle style = new GUIStyle(GUI.skin.button);
			using (new EditorGUI.DisabledGroupScope(disabledWhenTrue))
			{
				return GUILayout.Button(content);
			}
		}
	}


}

