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
	public class SuitImpulseDemo : SuitDemo
	{
		public enum ImpulseType { Emanating, Traversing, /*RepeatedImpulse*/ }
		public ImpulseType CurrentMode = ImpulseType.Emanating;

		#region Impulse Defining Attributes
		//Impulse information
		//I think the max is like 6-8?
		[Range(0, 10)]
		public int depth = 2;
		public float ImpulseDuration = 2.0f;
		public float Attenuation = 1.0f;
		public float EffectStrength = 1.0f;
		public float EffectDuration = .15f;

		[Range(0, 15)]
		public int CurrentEffect = 6;

		//Repeat count
		//Delay between repeats

		//Have a list of all base families?
		#region Basic Effects
		public string[] effectOptions =
		{
			"bump",
			"buzz",
			"click",
			"double_click",
			"fuzz",
			"hum",
			"long_double_sharp_tick",
			"pulse",
			"pulse_sharp",
			"sharp_click",
			"sharp_tick",
			"short_double_click",
			"short_double_sharp_tick",
			"transition_click",
			"transition_hum",
			"triple_click"
		};
		#endregion

		//Impulse Visual Color
		private Color selectedColor = new Color(227 / 255f, 127 / 255f, 127 / 255f, 1f);
		private Color unselectedColor = new Color(227 / 255f, 227 / 255f, 227 / 255f, 1f);
		private Color OriginColor = new Color(218 / 255f, 165 / 255f, 32 / 255f, 1f);
		#endregion

		#region Scene Refs
		//First Selected
		public SuitBodyCollider ImpulseOrigin;
		public SuitBodyCollider ImpulseDestination;

		//Buttons
		List<SuitBodyCollider> SuitNodes;

		#endregion

		void Start()
		{
			SuitNodes = FindObjectsOfType<SuitBodyCollider>().ToList();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				CurrentMode = CurrentMode == ImpulseType.Emanating ? ImpulseType.Traversing : ImpulseType.Emanating;
			}
		}

		//Turn on my needed things
		public override void ActivateDemo()
		{
			enabled = true;
			//Pick a base sequence
		}

		//Turn off my needed things
		public override void DeactivateDemo()
		{
			enabled = false;
		}

		public override void OnSuitClicked(SuitBodyCollider clicked, RaycastHit hit)
		{
			if (CurrentMode == ImpulseType.Emanating)
			{
				//Debug.Log((int)suit.regionID + "\n");
				ImpulseGenerator.Impulse imp = ImpulseGenerator.BeginEmanatingEffect(clicked.regionID, depth);
				if (imp != null)
				{
					//Select first
					ImpulseOrigin = clicked;

					ConfigureAndPlayImpulse(imp);
				}
			}
			else if (CurrentMode == ImpulseType.Traversing)
			{
				ClickedSuitInTraversalMode(clicked, hit);
			}
		}

		private void ClickedSuitInTraversalMode(SuitBodyCollider clicked, RaycastHit hit)
		{
			//None selected
			if (ImpulseOrigin == null)
			{
				//Select first
				ImpulseOrigin = clicked;
				ColorSuit(clicked, OriginColor);
				//SelectSuit(suit, hit, selectedColor);
			}
			//One selected
			else
			{
				if (ImpulseOrigin == clicked)
				{
					//Unselect First
					ColorSuit(clicked, unselectedColor);
					ImpulseOrigin = null;
					if (ImpulseDestination != null)
					{
						ColorSuit(ImpulseDestination, unselectedColor);
						ImpulseDestination = null;
					}
				}
				else
				{
					if (ImpulseDestination != null)
					{
						ColorSuit(ImpulseDestination, unselectedColor);
						ImpulseDestination = null;
					}

					ImpulseDestination = clicked;
					ColorSuit(clicked, OriginColor);

					//Play Impulse from the origin to here.
					//Debug.Log((int)TraversalOrigin.regionID + "\t " + (int)suit.regionID);
					ImpulseGenerator.Impulse imp = ImpulseGenerator.BeginTraversingImpulse(ImpulseOrigin.regionID, clicked.regionID);

					//Then play it
					ConfigureAndPlayImpulse(imp);

					//Don't save what we just clicked, so that way they can rapid click for multiple effects
				}
			}
		}

		private void ColorSuit(SuitBodyCollider suit, Color col)
		{
			//Light up the pad
			suit.GetComponent<MeshRenderer>().material.color = col;
		}

		private void ConfigureAndPlayImpulse(ImpulseGenerator.Impulse imp)
		{
			//Create an impulse here?
			//These are broken up by lines for readability
			//StartCoroutine(ColorPadForXDuration(ImpulseOrigin));
			if (CurrentMode == ImpulseType.Emanating)
			{
				StartCoroutine(ColorSuitForEmanation());
			}
			else
			{
				StartCoroutine(ColorSuitForTraversal());
			}
			imp.WithDuration(ImpulseDuration);
			imp.WithAttenuation(Attenuation);
			imp.WithEffect(effectOptions[CurrentEffect], EffectDuration, EffectStrength);
			imp.Play();

			//You can do something like:
			//i.WithAttenuation(Attenuation).WithDuration(ImpulseDuration).WithEffect(effectOptions[CurrentEffect], EffectDuration, EffectStrength).Play();
			//Chaining and Functional Programming!
		}

		SuitBodyCollider GetSuitForNode(GraphEngine.SuitNode target)
		{
			SuitBodyCollider suit = null;
			suit = SuitNodes.Where(x => x.regionID == target.Location).First();
			//Debug.Log("Asking for " + target.Location.ToString() + "  " + suit.regionID.ToString() + "\n");
			return suit;
		}

		IEnumerator ColorSuitForEmanation()
		{
			SuitBodyCollider start = ImpulseOrigin;
			List<List<GraphEngine.SuitNode>> nodes = ImpulseGenerator._grapher.BFS(start.regionID, depth);
			if (nodes != null && nodes.Count > 0)
			{
				//All the nodes hit in this 'round'
				for (int outter = 0; outter < nodes.Count; outter++)
				{
					if (nodes[outter] != null && nodes[outter].Count > 0)
					{
						//All the nodes hit in this 'round'
						for (int inner = 0; inner < nodes[outter].Count; inner++)
						{
							SuitBodyCollider next = GetSuitForNode(nodes[outter][inner]);
							if (next != null)
							{
								StartCoroutine(ColorPadForXDuration(next));
							}
						}
					}
					//Wait for next wave of impulse
					yield return new WaitForSeconds(ImpulseDuration / nodes.Count);
				}
			}
		}

		IEnumerator ColorSuitForTraversal()
		{
			SuitBodyCollider start = ImpulseOrigin;
			SuitBodyCollider destination = ImpulseDestination;
			List<GraphEngine.SuitNode> nodes = ImpulseGenerator._grapher.Dijkstras(start.regionID, destination.regionID);
			if (nodes != null && nodes.Count > 0)
			{
				//All the nodes hit in this 'round'
				for (int outter = 0; outter < nodes.Count; outter++)
				{
					if (nodes[outter] != null)
					{
						//All the nodes hit in this 'round'
						SuitBodyCollider next = GetSuitForNode(nodes[outter]);
						if (next != null)
						{
							StartCoroutine(ColorPadForXDuration(next));
						}
					}
					//Wait for next wave of impulse
					yield return new WaitForSeconds(ImpulseDuration / nodes.Count);
				}
			}
		}

		/// <summary>
		/// Colors a pad as playing for the Effect Duratin
		/// </summary>
		/// <param name="suit">The node to play</param>
		/// <returns></returns>
		IEnumerator ColorPadForXDuration(SuitBodyCollider suit)
		{
			SuitBodyCollider current = suit;
			ColorSuit(current, selectedColor);
			yield return new WaitForSeconds(EffectDuration);
			Color targetColor = (current == ImpulseOrigin || current == ImpulseDestination) ? OriginColor : unselectedColor;
			ColorSuit(current, targetColor);
		}
	}
}