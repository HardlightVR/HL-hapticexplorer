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

namespace NullSpace.SDK.Demos
{
	public class SuitDrawHapticsDemos : SuitDemo
	{
		private Color selectedColor = new Color(127 / 255f, 227 / 255f, 227 / 255f, 1f);
		private Color unselectedColor = new Color(227 / 255f, 227 / 255f, 227 / 255f, 1f);
		public List<SuitBodyCollider> selected;
		public Dictionary<SuitBodyCollider, float> RecentPlay;
		bool Clicking = false;
		bool Adding = false;

		public override void Start()
		{
			selected = new List<SuitBodyCollider>();
			selected = FindObjectsOfType<SuitBodyCollider>().ToList();
			for (int i = 0; i < selected.Count; i++)
			{
				MeshRenderer rend = selected[i].GetComponent<MeshRenderer>();
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
			HandleRequiredObjects(true);
			//Set the colors of the suit
			SelectAllSuitColliders();
		}

		//Turn off my needed things
		public override void DeactivateDemo()
		{
			HandleRequiredObjects(false);
			//Default behavior of inactive selector demo is all the colliders selected
			SelectAllSuitColliders();

			//But defaulting them to normal colors.
			UncolorAllSuitColliders();
		}

		public IEnumerator ChangeColorDelayed(GameObject g, Color c, float timeout)
		{
			yield return new WaitForSeconds(timeout);
			g.GetComponent<MeshRenderer>().material.color = c;
		}

		public override void OnSuitClicked(SuitBodyCollider clicked, RaycastHit hit)
		{
			Clicking = true;

			if (selected.Contains(clicked))
			{
				Adding = false;
				selected.Remove(clicked);
				StartCoroutine(ChangeColorDelayed(
				hit.collider.gameObject,
				unselectedColor,
				0.0f));
			}
			else
			{
				Adding = true;
				hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = selectedColor;
				selected.Add(clicked);
			}
		}

		public override void OnSuitClicking(SuitBodyCollider clicked, RaycastHit hit)
		{
			if (!Adding)
			{
				if (selected.Contains(clicked))
				{
					selected.Remove(clicked);
					StartCoroutine(ChangeColorDelayed(
					hit.collider.gameObject,
					unselectedColor,
					0.0f));
				}
			}
			else
			{
				if (!selected.Contains(clicked))
				{
					hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = selectedColor;
					selected.Add(clicked);
				}
			}
		}

		public override void OnSuitNoInput()
		{
			Clicking = false;
		}

		public void SelectAllSuitColliders()
		{
			selected.Clear();
			selected = FindObjectsOfType<SuitBodyCollider>().ToList();
			for (int i = 0; i < selected.Count; i++)
			{
				MeshRenderer rend = selected[i].GetComponent<MeshRenderer>();
				if (rend != null)
				{
					selected[i].GetComponent<MeshRenderer>().material.color = selectedColor;
				}
			}
		}

		public void DeselectAllSuitColliders()
		{
			UncolorAllSuitColliders();
			selected.Clear();
		}

		public void UncolorAllSuitColliders()
		{
			for (int i = 0; i < selected.Count; i++)
			{
				MeshRenderer rend = selected[i].GetComponent<MeshRenderer>();
				if (rend != null)
				{
					selected[i].GetComponent<MeshRenderer>().material.color = unselectedColor;
				}
			}
		}
	}
}