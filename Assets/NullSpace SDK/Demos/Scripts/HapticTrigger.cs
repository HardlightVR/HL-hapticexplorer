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
		Sequence onTriggerEnterSequence;
		public string fileName = "ns.pulse";

		void Awake()
		{
		}

		void Start()
		{
			NSManager.Instance.DisableTracking();
			onTriggerEnterSequence = new Sequence(fileName);
		}

		public void SetSequence(string sequenceName)
		{
			try
			{
				Sequence newSeq = new Sequence(sequenceName);
				if (newSeq != null)
				{
					fileName = sequenceName;
					onTriggerEnterSequence = newSeq;
				}
			}
			catch (HapticsLoadingException hExcept)
			{
				Debug.LogError("[Haptic Trigger - Haptics Loading Exception]   Attempted to set invalid sequence " + sequenceName + "\n\tLoad failed and set was disallowed, defaulted to previous sequence " + fileName + "\n" + hExcept.Message);
			}
		}

		void OnTriggerEnter(Collider collider)
		{
			SuitBodyCollider hit = collider.GetComponent<SuitBodyCollider>();
			if (hit != null)
			{
				AreaFlag flag = hit.regionID;
				//Debug.Log(flag.ToString() + "  " + (int)flag + "\n");
				onTriggerEnterSequence.CreateHandle(collider.GetComponent<SuitBodyCollider>().regionID).Play();
			} }
	}
}