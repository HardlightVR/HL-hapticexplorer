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

			//This holds all the scene references to objects.
			public SuitDefinition MyDefinition;

			public bool CanChangeValues = true;
			public List<Object> ObjectsToDestroy;
			public Vector2 errorScrollPos;
			bool TopFoldout = true;
			bool ObjectFieldFoldout = true;
			List<HelpMessage> outputMessages;
			string childAppendName = "'s Suit Body Collider";

			private int ComponentsToDestroy;
			private int GameObjectsToDestroy;
			public EditorSuitConfig()
			{
				CheckSuitDefinition();

				outputMessages = new List<HelpMessage>();
				ObjectsToDestroy = new List<Object>();
			}

			public void OnGUI()
			{
				CheckSuitDefinition();

				//scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

				GUIStyle style = new GUIStyle(GUI.skin.button);
				GUILayoutOption[] options = new GUILayoutOption[0];
				string suitDisplayName = MyDefinition.SuitName.Length > 0 ? MyDefinition.SuitName : MyDefinition.SuitRoot == null ? "Unnamed Suit" : MyDefinition.SuitRoot.name;
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

			private void CheckSuitDefinition()
			{
				if (MyDefinition == null)
				{
					MyDefinition = ScriptableObject.CreateInstance<SuitDefinition>();
					MyDefinition.Init();
				}
			}

			#region Options
			void DrawOptions(GUILayoutOption[] options)
			{
				GUIContent content = new GUIContent();
				float width = EditorGUIUtility.currentViewWidth;
				GUILayoutOption[] innerOptions = { GUILayout.MaxWidth(width / 2 - 10), GUILayout.MinWidth(35) };

				//GUIStyle columnStyle = new GUIStyle(EditorStyles.toolbarButton);

				EditorGUILayout.BeginVertical("Box");
				#region Suit Auto-Configuration
				//EditorGUILayout.LabelField("Suit Options");
				SuitRootObjectField(options);

				content = new GUIContent("Autoconfigure based on Root Object", "Uses common names to try and establish the different suit body colliders for the suit.");
				bool Result = OperationButton(MyDefinition.SuitRoot == null, content);
				if (Result)
				{
					HardlightSuit definition = MyDefinition.SuitRoot.GetComponent<HardlightSuit>();

					if (definition != null)
					{
						PopulateFromSuitDefinition(definition);
					}
					else
					{
						//	//Check if they have anything assigned.
						//	//If they do, warn them this will overwrite it.
						AutoFindElementsFromRoot(MyDefinition.SuitRoot);
					}
				}
				#endregion

				//content = new GUIContent("Can Change Values", "Can't be adjusted if you have a current layout. Remove SuitBodyColliders to adjust config.");
				//CanChangeValues = CreateStyledToggle(false, CanChangeValues, content, innerOptions);

				EditorGUILayout.BeginHorizontal();
				content = new GUIContent("Add Child Objects", "Create child objects instead of directly adding to the targeted object.");
				MyDefinition.AddChildObjects = CreateStyledToggle(!CanChangeValues, MyDefinition.AddChildObjects, content, innerOptions);

				content = new GUIContent("Add New Colliders", "Adds SuitBodyCollider objects instead of using existing ones or letting you configure it manually.\nWill add the colliders to child objects if that is also selected.");
				MyDefinition.AddExclusiveTriggerCollider = CreateStyledToggle(!CanChangeValues, MyDefinition.AddExclusiveTriggerCollider, content, innerOptions);

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}

			void SuitNaming(GUILayoutOption[] options)
			{
				MyDefinition.SuitName = EditorGUILayout.TextField("Suit Name", MyDefinition.SuitName, options);
			}
			void SuitRootObjectField(GUILayoutOption[] options)
			{
				GUIContent content = new GUIContent("Suit Root (Optional)", "For modifying an existing configuration. In the future this will try to find the related objects based on common naming conventions.");

				if (MyDefinition.SuitRoot != null)
					Undo.RecordObject(MyDefinition.SuitRoot, "Suit Root Selected");
				MyDefinition.SuitRoot = EditorGUILayout.ObjectField(content, MyDefinition.SuitRoot, typeof(GameObject), true, options) as GameObject;

				//Disallow Prefabs
				if (MyDefinition.SuitRoot != null && PrefabUtility.GetPrefabType(MyDefinition.SuitRoot) == PrefabType.Prefab)
				{
					MyDefinition.SuitRoot = null;
				}
			}
			bool OperationButton(bool disabledWhenTrue, GUIContent content)
			{
				//GUIStyle style = new GUIStyle(GUI.skin.button);
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
					//if (this != null)
					//{
					//	Debug.Log("Record toggle\n");
					//	Undo.RecordObject(this, "Toggle Field");
					//}

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


				for (int i = 0; i < MyDefinition.DefaultOptions.Count; i++)
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
				if (MyDefinition.DefaultOptions != null && MyDefinition.DefaultOptions.Count > index)
				{
					AreaFlag o = MyDefinition.DefaultOptions[index];
					string label = o.ToString();

					//Remove the underscores?

					GUILayout.Label(label, options);
				}
			}

			void CreateSuitObjectField(int index, GUILayoutOption[] options)
			{
				//GUILayout.Label("Suit Object Field " + SuitHolders.Count);
				if (MyDefinition.SuitHolders != null && MyDefinition.SuitHolders.Count > index)
				{
					GameObject o = MyDefinition.SuitHolders[index];
					//GUIContent label = new GUIContent("Intended Parent");
					MyDefinition.SuitHolders[index] = EditorGUILayout.ObjectField(o, typeof(GameObject), true, options) as GameObject;

					//Disallow Prefabs
					if (MyDefinition.SuitHolders[index] != null && PrefabUtility.GetPrefabType(MyDefinition.SuitHolders[index]) == PrefabType.Prefab)
					{
						MyDefinition.SuitHolders[index] = null;
					}

					//If the value changes
					if (MyDefinition.SuitHolders[index] != null && o != MyDefinition.SuitHolders[index])
					{
						SuitBodyCollider suit = LookupSceneReference(index);
						AssignQuickButton(suit, index);
					}
				}
			}

			//When we change the field. Lookup that object and assign the quickbutton if it can
			SuitBodyCollider LookupSceneReference(int index)
			{
				GameObject targObj = MyDefinition.SuitHolders[index];
				if (MyDefinition.SuitHolders[index] != null)
				{
					string lookupName = MyDefinition.SuitHolders[index].name + childAppendName;
					Transform objToCheck = MyDefinition.AddChildObjects ? targObj.transform.FindChild(lookupName) : targObj.transform;

					if (objToCheck != null)
					{
						return objToCheck.gameObject.GetComponent<SuitBodyCollider>();
					}
				}
				return null;
			}
			bool AssignQuickButton(SuitBodyCollider suit, int index)
			{
				if (suit != null && MyDefinition.SceneReferences[index] == null)
				{
					MyDefinition.SceneReferences[index] = suit;
					return true;
				}
				return false;
			}

			void SuitQuickButton(int index, GUILayoutOption[] options)
			{
				if (MyDefinition.SceneReferences != null && MyDefinition.SceneReferences.Count > index)
				{
					bool invalidObj = MyDefinition.SceneReferences[index] == null;
					string buttonLabel = string.Empty;

					if (invalidObj)
					{
						//Disabling
						EditorGUI.BeginDisabledGroup(invalidObj);
						buttonLabel = "Unassigned";
					}
					else
					{
						buttonLabel = MyDefinition.SceneReferences[index].name;

						//Max out the string size?
					}

					//Color this button differently if the AreaFlag doesn't match?
					if (GUILayout.Button(buttonLabel, EditorStyles.toolbarButton, options))
					{
						//Go to that object.
						Selection.activeGameObject = MyDefinition.SceneReferences[index].gameObject;
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
				string tooltip = "This will create Suit components on " + (MyDefinition.AddChildObjects ? " children of the selected objects" : " the selected objects");
				GUIContent content = new GUIContent("Create SuitBodyColliders", tooltip);
				bool OperationForbidden = CountValidSuitHolders() < 1;
				bool Result = OperationButton(OperationForbidden, content);
				if (Result)
				{
					//If we should add child objects
					if (MyDefinition.AddChildObjects)
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

								//If they choose to delete their current suit
								//Do they also want to remove the root's definition
								//This will handle if it doesn't have a root.
								HandleHardlightRemovalChoice();
							}
						}
					}
				}
			}

			int CountValidSuitHolders()
			{
				int validCount = 0;
				if (MyDefinition.SuitHolders != null && MyDefinition.SuitHolders.Count == MyDefinition.DefaultOptions.Count)
				{
					for (int i = 0; i < MyDefinition.SuitHolders.Count; i++)
					{
						if (MyDefinition.SuitHolders[i] != null)
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
				if (MyDefinition.SceneReferences != null && MyDefinition.SceneReferences.Count == MyDefinition.DefaultOptions.Count)
				{
					for (int i = 0; i < MyDefinition.SceneReferences.Count; i++)
					{
						if (MyDefinition.SceneReferences[i] != null)
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

					for (int optionIndex = 0; optionIndex < MyDefinition.DefaultOptions.Count; optionIndex++)
					{
						for (int suitIndex = 0; suitIndex < suitObjects.Count; suitIndex++)
						{
							if (suitObjects[suitIndex].regionID.HasFlag(MyDefinition.DefaultOptions[optionIndex]))
							{
								//Set the object field
								if (MyDefinition.AddChildObjects && suitObjects[suitIndex].transform.parent != null)
								{
									MyDefinition.SuitHolders[optionIndex] = suitObjects[suitIndex].transform.parent.gameObject;
								}
								if (!MyDefinition.AddChildObjects)
								{
									MyDefinition.SuitHolders[optionIndex] = suitObjects[suitIndex].gameObject;
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

			public void PopulateFromSuitDefinition(HardlightSuit hardlightSuit)
			{
				if (hardlightSuit != null)
				{
					MyDefinition.SuitRoot = hardlightSuit.gameObject;
					MyDefinition.DefaultOptions = hardlightSuit.Definition.DefaultOptions.ToList();
					MyDefinition.SuitHolders = hardlightSuit.Definition.SuitHolders.ToList();
					MyDefinition.SceneReferences = hardlightSuit.Definition.SceneReferences.ToList();
					MyDefinition.HapticsLayer = hardlightSuit.Definition.HapticsLayer;
					//We can't change stuff, we imported it
					CanChangeValues = false;
					MyDefinition.AddChildObjects = hardlightSuit.Definition.AddChildObjects;
					MyDefinition.AddExclusiveTriggerCollider = hardlightSuit.Definition.AddExclusiveTriggerCollider;
				}
			}

			//This function is for AUTOMATICALLY configuring a body object based on common names
			//It is not done
			void ProcessSuitRoot()
			{

			}

			string ClearSuitBodyColliders()
			{
				Debug.LogError("This is not used\nYou probably shouldn't use this - it was a temporary call and was refactored into RemoveComponentsForSuit\n");
				string output = string.Empty;
				for (int i = 0; i < MyDefinition.SceneReferences.Count; i++)
				{
					if (MyDefinition.SceneReferences[i] != null)
					{
						output += "Removing Suit Body Collider from " + MyDefinition.SceneReferences[i].name + " which had RegionID: " + MyDefinition.DefaultOptions[i].ToString();
					}
				}

				//Yep, we're going through it twice that way we can have a good output info.
				for (int i = 0; i < MyDefinition.SceneReferences.Count; i++)
				{
					//If something is on multiple objects, it'll get cleaned up only once.
					if (MyDefinition.SceneReferences[i] != null)
					{
						DestroyImmediate(MyDefinition.SceneReferences[i]);
					}
				}

				output = "Remove SuitBodyCollider scripts - Operation Finished\n\n" + output + "\n";
				return output;
			}

			#region Adding
			string AddChildNodesForSuit()
			{
				string output = string.Empty;
				for (int i = 0; i < MyDefinition.SuitHolders.Count; i++)
				{
					if (MyDefinition.SuitHolders[i] != null)
					{
						GameObject go = new GameObject();
						Undo.RegisterCreatedObjectUndo(go, "Add Suit Child Node");

						//Undo.SetTransformParent(myGameObject.transform, newTransformParent);
						Undo.SetTransformParent(go.transform, MyDefinition.SuitHolders[i].transform, "Set New Suit Node as Child");

						//We don't use this anymore, instead we register the transform change with Undo
						//go.transform.SetParent(MyDefinition.SuitHolders[i].transform);

						go.name = MyDefinition.SuitHolders[i].name + childAppendName;
					}
				}
				return output;
			}

			string AddComponentForSuit()
			{
				string output = string.Empty;

				HardlightSuit hardlightSuit = null;
				if (MyDefinition.SuitRoot != null)
				{
					Undo.RecordObject(MyDefinition.SuitRoot, "Configuring Suit Definition");
					hardlightSuit = MyDefinition.SuitRoot.GetComponent<HardlightSuit>();
					if (hardlightSuit == null)
					{
						Debug.Log("Adding Defined Suit Component\n");

						hardlightSuit = Undo.AddComponent<HardlightSuit>(MyDefinition.SuitRoot);
						//SuitRoot.AddComponent<DefinedSuit>();
					}
				}

				if (hardlightSuit != null)
				{
					Debug.Log("Recording DefinedSuit\n");
					Undo.RecordObject(hardlightSuit, "Filling out Defined Suit fields");
					hardlightSuit.Definition.AddChildObjects = MyDefinition.AddChildObjects;
					hardlightSuit.Definition.AddExclusiveTriggerCollider = MyDefinition.AddExclusiveTriggerCollider;
					hardlightSuit.Definition.SuitHolders = MyDefinition.SuitHolders.ToList();
					hardlightSuit.Definition.DefaultOptions = MyDefinition.DefaultOptions.ToList();
					hardlightSuit.Definition.SuitRoot = MyDefinition.SuitRoot;
				}

				for (int i = 0; i < MyDefinition.SuitHolders.Count; i++)
				{
					if (MyDefinition.SuitHolders[i] != null)
					{
						Undo.RecordObject(MyDefinition.SuitHolders[i], "Add Suit Node to Marked Objects");

						output += "Processing " + MyDefinition.SuitHolders[i].name + "";
						GameObject targetGO = MyDefinition.AddChildObjects ? MyDefinition.SuitHolders[i].transform.FindChild(MyDefinition.SuitHolders[i].name + childAppendName).gameObject : MyDefinition.SuitHolders[i];

						Collider col = null;
						//Check if it has one already
						SuitBodyCollider suit = targetGO.GetComponent<SuitBodyCollider>();
						if (suit == null)
						{
							//Add one if it doesn't
							//suit = targetGO.AddComponent<SuitBodyCollider>(); - Not undo-able

							suit = Undo.AddComponent<SuitBodyCollider>(targetGO);
							Undo.RecordObject(suit.gameObject, "Modifying Suit Object");

							if (MyDefinition.AddExclusiveTriggerCollider)
							{
								col = AddColliderForSuit(suit);
							}

							if (hardlightSuit != null)
							{
								Undo.RecordObject(hardlightSuit, "Filling out Defined Suit fields");
								hardlightSuit.Definition.SceneReferences[i] = suit;
							}

							output += "\t  Adding Suit Body Collider to " + MyDefinition.SuitHolders[i].name + "";
						}

						output += "\t  Adding " + MyDefinition.DefaultOptions[i].ToString() + " " + MyDefinition.SuitHolders[i].name + "\n";

						//Add this region to it.
						suit.regionID = suit.regionID | MyDefinition.DefaultOptions[i];

						//Save the collider if we made one
						if (col != null)
						{ suit.myCollider = col; }

						MyDefinition.SceneReferences[i] = suit;

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
				GameObject go = suit.gameObject;

				Collider col = Undo.AddComponent<BoxCollider>(go);
				//Collider col = go.AddComponent<BoxCollider>();
				col.gameObject.layer = MyDefinition.HapticsLayer;
				col.isTrigger = true;
				return col;
			}
			#endregion

			#region Removal
			string DetectComponentsToRemove()
			{
				string output = string.Empty;

				//We want to clear out the list
				ObjectsToDestroy.Clear();

				ComponentsToDestroy = 0;
				GameObjectsToDestroy = 0;

				if (MyDefinition.SceneReferences != null)
				{
					for (int i = 0; i < MyDefinition.SceneReferences.Count; i++)
					{
						if (MyDefinition.SceneReferences[i] != null)
						{
							//If add child objects
							if (MyDefinition.AddChildObjects)
							{
								int componentCount = MyDefinition.SceneReferences[i].gameObject.GetComponents<Component>().Length;

								//Debug.Log("Component Count is equal to " + componentCount + "\n");

								if (componentCount < 4)
								{
									ComponentsToDestroy++;
									if (MyDefinition.AddExclusiveTriggerCollider)
										ComponentsToDestroy++;
									GameObjectsToDestroy++;
									//Mark for deletion - they'll have to confirm removal.
									ObjectsToDestroy.Add(MyDefinition.SceneReferences[i].gameObject);
									output += "Marking : " + MyDefinition.SceneReferences[i].gameObject + "'s SuitBodyCollider Component for removal - has " + componentCount + " components\n";
								}
							}
							//Attached to the parrent
							else
							{
								if (MyDefinition.SuitHolders[i] != null)
								{
									SuitBodyCollider suit = MyDefinition.SuitHolders[i].GetComponent<SuitBodyCollider>();

									if (suit != null)
									{
										ComponentsToDestroy++;
										ObjectsToDestroy.Add(suit);
										output += "Marking : " + MyDefinition.SuitHolders[i].name + "'s SuitBodyCollider Component for removal\n";
									}

									if (MyDefinition.AddExclusiveTriggerCollider && suit.myCollider != null)
									{
										ComponentsToDestroy++;
										ObjectsToDestroy.Add(suit.myCollider);
										output += "Marking : " + MyDefinition.SuitHolders[i] + "'s " + suit.myCollider.GetType().ToString() + " Component for removal\n";
									}
								}
							}
						}
					}
				}

				output += "Operation Finished - " + ObjectsToDestroy.Count + " objects marked to destory\n";

				return output;
			}
			void DeleteMarkedObjects()
			{
				for (int i = ObjectsToDestroy.Count - 1; i > -1; i--)
				{
					if (ObjectsToDestroy[i] != null)
					{
						Undo.DestroyObjectImmediate(ObjectsToDestroy[i]);
					}
				}
				ObjectsToDestroy.Clear();
			}

			void HandleHardlightRemovalChoice()
			{
				HardlightSuit suit = GetRootHardlightSuitComponent();
				if (suit)
				{
					bool userResult = EditorUtility.DisplayDialog("Remove HardlightSuit Component", "Delete Hardlight Suit Definition\n(Stores a SuitDefinition for fast reimplementation)", "Remove", "Cancel");
					if (userResult)
					{
						DeleteHardlightSuitComponent(suit);
					}
				}
			}
			HardlightSuit GetRootHardlightSuitComponent()
			{
				if (MyDefinition.SuitRoot != null)
				{
					HardlightSuit suit = MyDefinition.SuitRoot.GetComponent<HardlightSuit>();
					if (suit)
					{
						return suit;
					}
				}
				return null;
			}
			void DeleteHardlightSuitComponent(HardlightSuit suit)
			{
				Undo.DestroyObjectImmediate(suit);
			}
			#endregion
			#endregion
		}

		#region Init & Setup
		// Add menu item to show this demo.
		[MenuItem("Tools/Hardlight Configure Body")]
		public static void Init()
		{
			myWindow = GetWindow(typeof(HardlightSetupWindow)) as HardlightSetupWindow;
			myWindow.Setup();
		}

		public void Setup()
		{
			Debug.Log("Performing Setup\n");
			Suits = new List<EditorSuitConfig>();

			List<HardlightSuit> existingDefinitions = FindObjectsOfType<HardlightSuit>().ToList();

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
			//GUILayoutOption[] options = new GUILayoutOption[0];
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
			/*bool result = */NullSpaceEditorStyles.OperationButton(!allowExpandAll, content);

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
			//GUIStyle style = new GUIStyle(GUI.skin.button);
			using (new EditorGUI.DisabledGroupScope(disabledWhenTrue))
			{
				return GUILayout.Button(content);
			}
		}
	}


}

