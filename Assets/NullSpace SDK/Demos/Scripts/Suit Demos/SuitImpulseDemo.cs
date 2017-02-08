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
	public class SuitImpulseDemo : SuitDemo
	{
		public enum ImpulseType { Emanating, Traversing, /*RepeatedImpulse*/ }
		/// <summary>
		/// Reusability focused. SuitImpulseDemo can come in multiple varieties (with a few unused variables depending on the mode.
		/// Emanation - Start at a point and affect in waves the neighbor pads.
		/// Traversing - Start at a point and move in stages to the destination through neighbor pads
		/// [Future] Ripple - Emanation but will 'bounce' off dead-ends onto already traveled pads.
		/// [Future] RepeatedImpulse - Any other ImpulseType but replays itself N times after X duration. Not an actual Impulsetype
		/// </summary>
		public ImpulseType CurrentMode = ImpulseType.Emanating;

		#region Impulse Defining Attributes
		//For some reason Unity back-recognizes Header tags. I put them in reverse order to fix that
		//Weird.
		[Header("These values are default. Slider & display values are set by hand")]
		//Impulse information
		/// <summary>
		/// Maximum possible depth is currently 7 (Forearm to Forearm)
		/// </summary>
		[SerializeField]
		private int depth = 2;

		/// <summary>
		/// How long each individual effect of the impulse plays
		/// </summary>
		[SerializeField]
		private float effectDuration = .15f;
		/// <summary>
		/// The total duration of the impulse. This calculates the offset from the previous to next.
		/// </summary>
		[SerializeField]
		private float impulseDuration = .75f;

		[Header("[Not Yet Supported]")]
		public float Attenuation = 1.0f;
		public float EffectStrength = 1.0f;

		/// <summary>
		/// Which base effect will be used by the impulse. 
		/// Cannot use file sequences, only raw effects or CodeSequences (constructed of raw effects)
		/// Options are (in order): bump, buzz, click, double_click, fuzz, hum, 
		/// long_double_sharp_tick, pulse, pulse_sharp, sharp_click, 
		/// sharp_tick, short_double_click, short_double_sharp_tick, 
		/// transition_click, transition_hum, triple_click
		/// </summary>
		[Range(0, 15)]
		private int currentEffect = 6;

		private int selectedCodeSequence = -1;

		#region Properties
		public float Depth
		{
			get
			{
				return depth;
			}
			set
			{
				depth = Mathf.RoundToInt(value);
			}
		}
		public float ImpulseDuration
		{
			get
			{
				return impulseDuration;
			}
			set
			{
				impulseDuration = value;
			}
		}
		public float EffectDuration
		{
			get
			{
				return effectDuration;
			}
			set
			{
				effectDuration = (int)value;
			}
		}
		/// <summary>
		/// Which base effect will be used by the impulse. 
		/// Converted to an Int. Only a float for UI-Slider tricks
		/// Cannot use file sequences, only raw effects or CodeSequences (constructed of raw effects)
		/// Options are (in order): bump, buzz, click, double_click, fuzz, hum, 
		/// long_double_sharp_tick, pulse, pulse_sharp, sharp_click, 
		/// sharp_tick, short_double_click, short_double_sharp_tick, 
		/// transition_click, transition_hum, triple_click
		/// </summary>
		public float CurrentEffect
		{
			get
			{
				return currentEffect;
			}
			set
			{
				currentEffect = Mathf.RoundToInt(value);
			}
		}
		public float SelectedCodeSequence
		{
			get
			{
				return selectedCodeSequence;
			}
			set
			{
				selectedCodeSequence = Mathf.RoundToInt(value);
			}
		}
		#endregion

		//Repeat count
		//Delay between repeats

		//Have a list of all base families?
		#region Basic Effects
		public static string[] effectOptions =
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

		//Impulse Visual Color - feel free to change these if you have preferences.
		//Region Selector Suit Demo uses a light green.
		public Color selectedColor = new Color(227 / 255f, 127 / 255f, 127 / 255f, 1f);
		public Color unselectedColor = new Color(227 / 255f, 227 / 255f, 227 / 255f, 1f);
		public Color OriginColor = new Color(218 / 255f, 165 / 255f, 32 / 255f, 1f);
		#endregion

		#region Scene Refs
		[Header("Public Objects for ease of understanding")]
		//First Selected - For showing where we clicked/last clicked.
		public SuitBodyCollider ImpulseOrigin;
		//Second Selected - for showing the start/end points
		public SuitBodyCollider ImpulseDestination;

		//Each available 'button'. Very useful for creating the color visualization.
		//Our lookup method could be better, but the scene isn't heavily populated and we don't need VR FPS
		List<SuitBodyCollider> SuitNodes;
		#endregion

		public override void Start()
		{
			base.Start();
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
			ColorSuit(ImpulseOrigin, OriginColor);

			HandleRequiredObjects(true);

			//Pick a base sequence
		}

		//Turn off my needed things
		public override void DeactivateDemo()
		{
			HandleRequiredObjects(false);
			ColorSuit(ImpulseOrigin, unselectedColor);
			ColorSuit(ImpulseDestination, unselectedColor);

		}

		public override void OnSuitClicked(SuitBodyCollider clicked, RaycastHit hit)
		{
			//Start with which mode this SuitDemo is in

			// Emanation - Start at a point and affect in waves the neighbor pads.
			if (CurrentMode == ImpulseType.Emanating)
			{
				//Debug.Log((int)suit.regionID + "\n");
				ImpulseGenerator.Impulse imp = ImpulseGenerator.BeginEmanatingEffect(clicked.regionID, (int)Depth);
				if (imp != null)
				{
					ColorSuit(ImpulseOrigin, unselectedColor);
					//Select first
					ImpulseOrigin = clicked;

					ConfigureAndPlayImpulse(imp);
				}
			}
			// Traversing - Start at a point and move in stages to the destination through neighbor pads
			else if (CurrentMode == ImpulseType.Traversing)
			{
				ClickedSuitInTraversalMode(clicked, hit);
			}
		}

		private void ClickedSuitInTraversalMode(SuitBodyCollider clicked, RaycastHit hit)
		{
			//None are currently selected
			if (ImpulseOrigin == null)
			{
				//Select first
				ImpulseOrigin = clicked;

				//Mark it as selected
				ColorSuit(clicked, OriginColor);
			}
			//First one is already selected
			else
			{
				//If we click back on the first node.
				if (ImpulseOrigin == clicked)
				{
					//Unselect First
					ColorSuit(clicked, unselectedColor);
					ImpulseOrigin = null;

					//If we had a destination
					if (ImpulseDestination != null)
					{
						//Clear it.
						ColorSuit(ImpulseDestination, unselectedColor);
						ImpulseDestination = null;
					}
				}
				else
				{
					//If we had a destination (from last play)
					if (ImpulseDestination != null)
					{
						//Clear it to avoid leaving unnecessary colored nodes
						ColorSuit(ImpulseDestination, unselectedColor);
						ImpulseDestination = null;
					}

					//Set our destination
					ImpulseDestination = clicked;
					ColorSuit(clicked, OriginColor);

					//Leftover log to see that we're playing from the start to end.
					//Debug.Log((int)TraversalOrigin.regionID + "\t " + (int)suit.regionID);

					//Play Impulse from the origin to our brand new destination.
					ImpulseGenerator.Impulse imp = ImpulseGenerator.BeginTraversingImpulse(ImpulseOrigin.regionID, clicked.regionID);

					//Then play it
					ConfigureAndPlayImpulse(imp);
				}
			}
		}

		/// <summary>
		/// Colors a particular suit visual to the labeled color.
		/// Performs a null check on suit first.
		/// </summary>
		/// <param name="suit"></param>
		/// <param name="col"></param>
		private void ColorSuit(SuitBodyCollider suit, Color col)
		{
			//This is just sanitization and to make the code more robust.
			if (suit != null)
			{
				//We could easily be more efficient than getting the MeshRenderer each time (like having SuitBodyCollider hold onto a ref to it's MeshRenderer)
				//However this isn't a VR application, so ease of programming/readability is the priority here.
				suit.GetComponent<MeshRenderer>().material.color = col;
			}
		}
		/// <summary>
		/// Takes an impulse and plays it with the ImpulseDemo's parameters.
		/// This takes an impulse so you don't need to instantiate one every time.
		/// If you're doing the same impulse regularly, it saves traversal/breadth first searching
		/// </summary>
		/// <param name="imp">A constructed impulse of the emanation or traversal</param>
		private void ConfigureAndPlayImpulse(ImpulseGenerator.Impulse imp)
		{
			if (CurrentMode == ImpulseType.Emanating)
			{
				StartCoroutine(ColorSuitForEmanation());
			}
			else
			{
				StartCoroutine(ColorSuitForTraversal());
			}

			//To support CodeSequence Samples
			if (SelectedCodeSequence < 0)
			{
				//These are broken up by lines for readability
				imp.WithDuration(ImpulseDuration);
				imp.WithAttenuation(Attenuation);
				imp.WithEffect(effectOptions[currentEffect], EffectDuration, EffectStrength);
				imp.Play();

				//You can do something like:
				//i.WithAttenuation(Attenuation).WithDuration(ImpulseDuration).WithEffect(effectOptions[CurrentEffect], EffectDuration, EffectStrength).Play();
				//Chaining and Functional Programming!
			}
			else
			{
				//These are broken up by lines for readability
				imp.WithDuration(ImpulseDuration);
				imp.WithAttenuation(Attenuation);
				imp.WithEffect(GetCodeSequence());
				imp.Play();
			}
		}

		#region Code Sequence Names
		//Note: This is a junky way to do Code Sequences but I wanted to include them as samples
		public static string[] SampleCodeSequence =
		{
			"ClickHum",
			"ThockClunk",
			"ClickStorm",
			"DoubleClickImpact",
			"Shimmer",
			"ClickHumDoubleClick",
			"PulseBumpPulse",
			"TripleClickFuzzFalloff",
			"RandomPulses",
			"ThreeRandomEffects",
			"VeryRandomEffect",
			//If I add more later
			"",
			"",
			"",
			"",
			" "
		};
		#endregion

		CodeSequence GetCodeSequence()
		{
			if (SelectedCodeSequence == 0)
			{
				return ImpulseCodeSequenceSamples.ClickHum();
			}
			else if (SelectedCodeSequence == 1)
			{
				return ImpulseCodeSequenceSamples.ThockClunk();
			}
			else if (SelectedCodeSequence == 2)
			{
				return ImpulseCodeSequenceSamples.ClickStorm();
			}
			else if (SelectedCodeSequence == 3)
			{
				return ImpulseCodeSequenceSamples.DoubleClickImpact();
			}
			else if (SelectedCodeSequence == 4)
			{
				return ImpulseCodeSequenceSamples.Shimmer();
			}
			else if (SelectedCodeSequence == 5)
			{
				return ImpulseCodeSequenceSamples.ClickHumDoubleClick();
			}
			else if (SelectedCodeSequence == 6)
			{
				return ImpulseCodeSequenceSamples.PulseBumpPulse();
			}
			else if (SelectedCodeSequence == 7)
			{
				return ImpulseCodeSequenceSamples.TripleClickFuzzFalloff();
			}
			else if (SelectedCodeSequence == 8)
			{
				return ImpulseCodeSequenceSamples.RandomPulses(Random.Range(0, 10000));
			}
			else if (SelectedCodeSequence == 9)
			{
				return ImpulseCodeSequenceSamples.ThreeRandomEffects(Random.Range(0, 10000));
			}
			else if (SelectedCodeSequence == 10)
			{
				return ImpulseCodeSequenceSamples.VeryRandomEffect(Random.Range(0, 10000));
			}
			else //if (SelectedCodeSequence == 10)
			{
				return ImpulseCodeSequenceSamples.VeryRandomEffect(Random.Range(0, 10000));
			}
		}

		SuitBodyCollider GetSuitForNode(GraphEngine.SuitNode target)
		{
			SuitBodyCollider suit = null;

			//Get the SuitBodyCollider node where the region IDs match. If multiple match, take the first
			suit = SuitNodes.Where(x => x.regionID == target.Location).First();
			//Yay functional programming

			//This is potentially problematic if you are using a suit model with MULTIPLE flags set for individual locations.
			//It would likely give some inaccurate visuals or have odd error cases.

			//Debug.Log("Asking for " + target.Location.ToString() + "  " + suit.regionID.ToString() + "\n");
			return suit;
		}

		IEnumerator ColorSuitForEmanation()
		{
			//Save the beginning in local scope in case it gets changed by additional input 
			SuitBodyCollider start = ImpulseOrigin;

			//List of Lists
			//Stage 1: The pad clicked
			//Stage 2: One adjacent pad
			//Stage 3: Four pads adjacent to the previous pad
			//Stage 4: A few pads adjacent to last stage
			List<List<GraphEngine.SuitNode>> nodes = ImpulseGenerator._grapher.BFS(start.regionID, (int)Depth);
			if (nodes != null && nodes.Count > 0)
			{
				//For each possible stage
				for (int outter = 0; outter < nodes.Count; outter++)
				{
					if (nodes[outter] != null && nodes[outter].Count > 0)
					{
						//For each node in this stage
						for (int inner = 0; inner < nodes[outter].Count; inner++)
						{
							SuitBodyCollider next = GetSuitForNode(nodes[outter][inner]);
							if (next != null)
							{
								//Color that pad for the duration of the Effect
								StartCoroutine(ColorPadForXDuration(next));
							}
						}
					}
					//Wait for next stage of the impulse
					yield return new WaitForSeconds(ImpulseDuration / nodes.Count);
				}
			}
		}

		IEnumerator ColorSuitForTraversal()
		{
			//Save the beginning and end in local scope in case they get changed by additional input (Which could cause some null refs/index out of bounds)
			SuitBodyCollider start = ImpulseOrigin;
			SuitBodyCollider destination = ImpulseDestination;
			List<GraphEngine.SuitNode> nodes = ImpulseGenerator._grapher.Dijkstras(start.regionID, destination.regionID);
			if (nodes != null && nodes.Count > 0)
			{
				//For each step in the traversal
				for (int outter = 0; outter < nodes.Count; outter++)
				{
					if (nodes[outter] != null)
					{
						//Get the corresponding Suit based on the GraphEngine's edge map.
						SuitBodyCollider next = GetSuitForNode(nodes[outter]);
						if (next != null)
						{
							//Color that pad for it's effect duration.
							StartCoroutine(ColorPadForXDuration(next));
						}
					}
					//Wait for next stage of the impulse
					yield return new WaitForSeconds(ImpulseDuration / nodes.Count);
				}
			}
		}

		/// <summary>
		/// Colors a pad as playing for the Effect Duratin
		/// </summary>
		/// <param name="suit">The node to color</param>
		/// <returns></returns>
		IEnumerator ColorPadForXDuration(SuitBodyCollider suit)
		{
			//This function simulates the color of the pad. 
			//It doesn't actually track the under-the-hood information of what is/isn't playing
			//That means if you call halt, it will still color despite no haptics.
			//Tools for that functionality are currently in the pipeline.

			//I don't think we need to save this local reference. Just in case.
			SuitBodyCollider current = suit;

			//You could do a fancy color lerp functionality here...
			ColorSuit(current, selectedColor);

			//I clamp this to a min of .1 for user visibility.
			yield return new WaitForSeconds(Mathf.Clamp(EffectDuration, .1f, 100));

			//Revert our color
			Color targetColor = (current == ImpulseOrigin || current == ImpulseDestination) ? OriginColor : unselectedColor;
			ColorSuit(current, targetColor);
		}
	}
}