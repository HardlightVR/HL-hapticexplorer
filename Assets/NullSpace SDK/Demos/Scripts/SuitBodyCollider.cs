/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://www.hardlightvr.com/wp-content/uploads/2017/01/NullSpace-SDK-License-Rev-3-Jan-2016-2.pdf
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using UnityEngine;

namespace NullSpace.SDK
{
	/// <summary>
	/// A simple class to link up a collider with a certain area on the suit
	/// </summary>
	public class SuitBodyCollider : MonoBehaviour
	{
		[Header("Note: Attach this to your player's body.")]
		[Header("You can select multiple areas for a single collider.")]
		//You can enable [EnumFlag] over [RegionFlag] if you're having problems with the more customized inspector.
		//Additionally, I provided EnumFlag for easy reference of a similar implementation
		//[EnumFlag]
		[RegionFlag]
		public AreaFlag regionID;
		public Collider myCollider;

		void Awake()
		{
			//In case it isn't assigned or created by the BodySetup tool.
			if (myCollider == null)
			{
				//Find our collider
				myCollider = gameObject.GetComponent<Collider>();
			}
		}

		void Start()
		{
			if (myCollider == null)
			{
				Debug.LogError("SuitBodyCollider does not have a collider set - Name [" + name + "] with regionID [" + regionID + "].\n");
			}
			else
			{
				if (!myCollider.isTrigger)
				{
					Debug.LogWarning("Haptic Collider " + regionID + " is not attached to a trigger volume.\n");
				}
			}
		}
	}
}