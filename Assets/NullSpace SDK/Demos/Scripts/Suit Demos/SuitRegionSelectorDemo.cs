/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://www.hardlightvr.com/wp-content/uploads/2017/01/NullSpace-SDK-License-Rev-3-Jan-2016-2.pdf
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullSpace.SDK;

namespace NullSpace.SDK.Demos
{
	public class SuitRegionSelectorDemo : SuitDemo
	{
		private Color selectedColor = new Color(127 / 255f, 227 / 255f, 127 / 255f, 1f);
		private Color unselectedColor = new Color(227 / 255f, 227 / 255f, 227 / 255f, 1f);
		bool Adding = false;

		public override void Start()
		{
			suitObjects = new List<SuitBodyCollider>();
			suitObjects = FindObjectsOfType<SuitBodyCollider>().ToList();
			for (int i = 0; i < suitObjects.Count; i++)
			{
				MeshRenderer rend = suitObjects[i].GetComponent<MeshRenderer>();
				if (rend != null)
				{
					rend.material.color = selectedColor;
				}
			}
			base.Start();

		}

		//Turn on my needed things
		public override void ActivateDemo()
		{
			//Set the colors of the suit
			SelectAllSuitColliders();
		}

		//Turn off my needed things
		public override void DeactivateDemo()
		{
			//Default behavior of inactive selector demo is all the colliders selected
			SelectAllSuitColliders();

			//But defaulting them to normal colors.
			UncolorAllSuitColliders();
		}

		public override void OnSuitClicked(SuitBodyCollider clicked, RaycastHit hit)
		{
			Adding = true;

			if (suitObjects.Contains(clicked))
			{
				Adding = false;
				suitObjects.Remove(clicked);
				StartCoroutine(ChangeColorDelayed(
				hit.collider.gameObject,
				unselectedColor,
				0.0f));
			}
			else
			{
				hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = selectedColor;
				suitObjects.Add(clicked);
			}
		}

		public override void OnSuitClicking(SuitBodyCollider clicked, RaycastHit hit)
		{
			if (!Adding)
			{
				if (suitObjects.Contains(clicked))
				{
					suitObjects.Remove(clicked);
					StartCoroutine(ChangeColorDelayed(
					hit.collider.gameObject,
					unselectedColor,
					0.0f));
				}
			}
			else
			{
				if (!suitObjects.Contains(clicked))
				{
					hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = selectedColor;
					suitObjects.Add(clicked);
				}
			}
		}

		public override void OnSuitNoInput()
		{
			Adding = false;
		}

		public void SelectAllSuitColliders()
		{
			suitObjects.Clear();
			suitObjects = FindObjectsOfType<SuitBodyCollider>().ToList();
			for (int i = 0; i < suitObjects.Count; i++)
			{
				MeshRenderer rend = suitObjects[i].GetComponent<MeshRenderer>();
				if (rend != null)
				{
					suitObjects[i].GetComponent<MeshRenderer>().material.color = selectedColor;
				}
			}
		}

		public void DeselectAllSuitColliders()
		{
			UncolorAllSuitColliders();
			suitObjects.Clear();
		}

		public void UncolorAllSuitColliders()
		{
			for (int i = 0; i < suitObjects.Count; i++)
			{
				MeshRenderer rend = suitObjects[i].GetComponent<MeshRenderer>();
				if (rend != null)
				{
					suitObjects[i].GetComponent<MeshRenderer>().material.color = unselectedColor;
				}
			}
		}
	}
}