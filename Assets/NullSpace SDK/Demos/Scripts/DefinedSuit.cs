using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace NullSpace.SDK.Demos
{
	public class DefinedSuit : MonoBehaviour
	{
		[SerializeField]
		public SuitConfiguration MySuit;

		[System.Serializable]
		public class SuitConfiguration
		{
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

				ObjectsToDestroy = new List<Object>();
			}

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
						GameObject targetGO = AddChildObjects ? SuitHolders[i].transform.FindChild(SuitHolders[i].name + childAppendName).gameObject : SuitHolders[i];

						Collider col = null;
						//Check if it has one already
						SuitBodyCollider suit = targetGO.GetComponent<SuitBodyCollider>();
						if (suit == null)
						{
							//Add one if it doesn't
							//suit = targetGO.AddComponent<SuitBodyCollider>(); - Not undo-able

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
				GameObject go = suit.gameObject;

				Collider col = go.AddComponent<BoxCollider>();
				col.gameObject.layer = HapticsLayer;
				col.isTrigger = true;
				return col;
			}
			#endregion
		}
	}
}