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
		public Dictionary<SuitBodyCollider, float> PlayingLocations;
		public List<float> playingDurations;

		/// <summary>
		/// Note: the reason we care about duration is this version of the haptics API doesn't let us see what is currently playing/not playing.
		/// Therefore we can't visuall represent what is actually happening on the suit.
		/// </summary>
		private float duration = .25f;
		private float minDuration = .05f;

		public float Duration
		{
			get
			{
				return duration;
			}

			set
			{
				duration = value;
			}
		}

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

				playingDurations.Add(0);
			}
			base.Start();
		}

		public void Update()
		{
			for (int i = 0; i < playingDurations.Count; i++)
			{
				playingDurations[i] = Mathf.Clamp(playingDurations[i] - Time.deltaTime, 0, 1000);
				if (playingDurations[i] <= 0)
				{
					//This handles decoloring locations that have finished their duration (and can be reset)
					ColorSuitCollider(suitObjects[i], unselectedColor);
				}
			}
		}

		//Turn on my needed things
		public override void ActivateDemo()
		{
		}

		//Turn off my needed things
		public override void DeactivateDemo()
		{
			//But defaulting them to normal colors.
			UncolorAllSuitColliders();
		}

		public override void OnSuitClicked(SuitBodyCollider clicked, RaycastHit hit)
		{
			PlayDrawnHaptic(clicked, hit);
		}

		public override void OnSuitClicking(SuitBodyCollider clicked, RaycastHit hit)
		{
			PlayDrawnHaptic(clicked, hit);
		}

		public override void OnSuitNoInput()
		{
		}

		public void PlayDrawnHaptic(SuitBodyCollider clicked, RaycastHit hit)
		{
			//This could be done more efficiently. It is kept simple to make the code more readible.
			int index = suitObjects.IndexOf(clicked);

			//If the current duration is over.
			if (playingDurations[index] <= 0)
			{
				//We enforce a minimum duration mostly to ensure there is a good visual.
				playingDurations[index] = Mathf.Clamp(Duration, minDuration, float.MaxValue);

				//Color the suit (drawn haptic expiration handles decoloring.
				ColorSuitCollider(clicked, selectedColor);

				//Find where we drew on
				AreaFlag flag = clicked.regionID;

				//Play the last played sequence there.
				LibraryManager.Inst.LastSequence.CreateHandle(flag).Play();

				//When the effects expire, the decoloring is handled there.
				//StartCoroutine(ChangeColorDelayed(
				//	hit.collider.gameObject,
				//	unselectedColor,
				//	Duration));
			}
			else
			{
				//Don't do anything
			}
		}

		//public void SelectAllSuitColliders()
		//{
		//	suitObjects.Clear();
		//	suitObjects = FindObjectsOfType<SuitBodyCollider>().ToList();
		//	for (int i = 0; i < suitObjects.Count; i++)
		//	{
		//		ColorSuitCollider(suitObjects[i], selectedColor);
		//	}
		//}

		//public void DeselectAllSuitColliders()
		//{
		//	UncolorAllSuitColliders();
		//}

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