/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://www.hardlightvr.com/wp-content/uploads/2017/01/NullSpace-SDK-License-Rev-3-Jan-2016-2.pdf
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using UnityEngine;

namespace NullSpace.SDK.Demos
{
	/// <summary>
	/// Scene-specific script to trigger haptic effects
	/// </summary>
	public class HapticTrigger : MonoBehaviour
	{
		public string fileName;

		public void SetSequence(string sequenceName)
		{
			fileName = sequenceName;
		}

		void OnTriggerEnter(Collider collider)
		{
			SuitBodyCollider hit = collider.GetComponent<SuitBodyCollider>();
			if (hit != null)
			{
				LibraryManager.Inst.LastSequence.CreateHandle(hit.regionID).Play();
			}
		}
	}
}