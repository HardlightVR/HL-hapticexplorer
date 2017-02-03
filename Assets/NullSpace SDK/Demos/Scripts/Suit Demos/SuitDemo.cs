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

		public List<GameObject> ActiveObjects;
		public List<GameObject> ActiveIfDisabledObjects;

		public void HandleRequiredObjects(bool Activating)
		{
			for (int i = 0; i < ActiveObjects.Count; i++)
			{
				if (ActiveObjects[i])
				{
					//Debug.Log("Setting Active : " + ActiveObjects[i].name + "\n to " + Activating);
					ActiveObjects[i].SetActive(Activating);
				}
			}
			for (int i = 0; i < ActiveIfDisabledObjects.Count; i++)
			{
				if (ActiveIfDisabledObjects[i])
				{
					//Debug.Log("Setting Anti-Active : " + ActiveIfDisabledObjects[i].name + "\n to " + Activating);
					ActiveIfDisabledObjects[i].SetActive(!Activating);
				}
			}
		}

		public virtual void Start()
		{
			DeactivateDemo();
			enabled = false;
		}
	}

	public class SuitClickDemo : SuitDemo
	{
		//Turn on my needed things
		public override void ActivateDemo()
		{
			HandleRequiredObjects(true);

			//I need nothing
		}

		//Turn off my needed things
		public override void DeactivateDemo()
		{
			HandleRequiredObjects(false);

			//I need nothing
		}

		public override void OnSuitClicked(SuitBodyCollider clicked, RaycastHit hit)
		{
			Debug.Log("Clicked on " + clicked.name + " with a regionID value of: " + (int)clicked.regionID + "\n");
		}
	}
}