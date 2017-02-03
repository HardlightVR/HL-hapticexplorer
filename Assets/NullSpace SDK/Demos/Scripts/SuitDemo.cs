/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://www.hardlightvr.com/wp-content/uploads/2017/01/NullSpace-SDK-License-Rev-3-Jan-2016-2.pdf
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NullSpace.SDK.Demos
{
	abstract public class SuitDemo : MonoBehaviour
	{
		//Turn on my needed things
		abstract public void ActivateDemo();

		//Turn off my needed things
		abstract public void DeactivateDemo();

		abstract public void OnSuitClicked(SuitBodyCollider suit, RaycastHit hit);

		public List<GameObject> RequiredObjects;
	}

	public class SuitClickDemo : SuitDemo
	{
		//Turn on my needed things
		public override void ActivateDemo()
		{
			enabled = true;
			//I need nothing
		}

		//Turn off my needed things
		public override void DeactivateDemo()
		{
			enabled = false;
			//I give up nothing
		}

		public override void OnSuitClicked(SuitBodyCollider clicked, RaycastHit hit)
		{
			Debug.Log("Clicked on " + clicked.name + " with a regionID value of: " + (int)clicked.regionID + "\n");
		}
	}
}