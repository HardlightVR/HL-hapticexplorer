using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NullSpace.SDK.Demos
{
	public class SuitDefinition : ScriptableObject
	{
		public string SuitName = "Player Body";
		public GameObject SuitRoot;

		[SerializeField]
		public List<AreaFlag> DefaultOptions;

		//The Game Objects to fill the fields (which will get suit body references
		[SerializeField]
		public List<GameObject> SuitHolders;

		//the objects added. Will get a nice button list to quick get to each of them.
		[SerializeField]
		public List<SuitBodyCollider> SceneReferences;

		public int HapticsLayer = 31;
		public bool AddChildObjects = true;
		public bool AddExclusiveTriggerCollider = true;

		public void Init()
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
		}
	}
}