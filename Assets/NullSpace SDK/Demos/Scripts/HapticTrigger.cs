/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://nullspacevr.com/?wpdmpro=nullspace-developer-agreement
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/


using UnityEngine;
using NullSpace.SDK;


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
		AreaFlag flag = collider.GetComponent<SuitBodyCollider>().regionID;
		//Debug.Log(flag.ToString() + "  " + (int)flag + "\n");
		onTriggerEnterSequence.CreateHandle(collider.GetComponent<SuitBodyCollider>().regionID).Play();
	}
}
