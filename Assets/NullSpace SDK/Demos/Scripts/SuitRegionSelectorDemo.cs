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
		public List<SuitBodyCollider> selected;

		void Start()
		{
			selected = new List<SuitBodyCollider>();
			selected = FindObjectsOfType<SuitBodyCollider>().ToList();
			for (int i = 0; i < selected.Count; i++)
			{
				selected[i].GetComponent<MeshRenderer>().material.color = selectedColor;
			}
		}

		//Turn on my needed things
		public override void ActivateDemo()
		{
			enabled = true;

			//Set the colors of the suit
			SelectAllSuitColliders();
		}

		//Turn off my needed things
		public override void DeactivateDemo()
		{
			enabled = false;

			//Revert the colors of the suit
			DeselectAllSuitColliders();
		}

		public IEnumerator ChangeColorDelayed(GameObject g, Color c, float timeout)
		{
			yield return new WaitForSeconds(timeout);
			g.GetComponent<MeshRenderer>().material.color = c;
		}

		public override void OnSuitClicked(SuitBodyCollider clicked, RaycastHit hit)
		{
			if (selected.Contains(clicked))
			{
				selected.Remove(clicked);
				StartCoroutine(ChangeColorDelayed(
				hit.collider.gameObject,
				unselectedColor,
				0.0f));
			}
			else
			{
				hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = selectedColor;
				selected.Add(clicked);
			}
		}

		public void SelectAllSuitColliders()
		{
			selected.Clear();
			selected = FindObjectsOfType<SuitBodyCollider>().ToList();
			for (int i = 0; i < selected.Count; i++)
			{
				selected[i].GetComponent<MeshRenderer>().material.color = selectedColor;
			}
		}

		public void DeselectAllSuitColliders()
		{
			for (int i = 0; i < selected.Count; i++)
			{
				selected[i].GetComponent<MeshRenderer>().material.color = unselectedColor;
			}
			selected.Clear();
		}
	}
}