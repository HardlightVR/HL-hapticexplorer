using UnityEngine;
using System.Collections;

namespace NullSpace.SDK.Demos
{
	public class ImpulseCodeSequenceSamples : MonoBehaviour
	{
		public static CodeSequence ClickHum()
		{
			CodeSequence seq = new CodeSequence();
			CodeEffect eff = new CodeEffect("click", 0.0f, 1.0f);
			seq.AddEffect(0, eff);
			eff = new CodeEffect("hum", .2f, .5f);
			seq.AddEffect(.15, eff);
			return seq;
		}

		public static CodeSequence ThockClunk()
		{
			CodeSequence seq = new CodeSequence();
			CodeEffect eff = new CodeEffect("sharp_click", 0.15f, 1.0f);
			seq.AddEffect(0, eff);
			eff = new CodeEffect("fuzz", .2f, 1.0f);
			seq.AddEffect(.15, eff);
			return seq;
		}

		public static CodeSequence ClickStorm()
		{
			CodeSequence seq = new CodeSequence();

			CodeEffect eff = new CodeEffect("double_click", .0f, 1.0f);
			seq.AddEffect(0, eff);

			eff = new CodeEffect("click", .0f, 1.0f);
			seq.AddEffect(0.1, eff);

			eff = new CodeEffect("long_double_sharp_tick", .0f, 1.0f);
			seq.AddEffect(0.2, eff);

			eff = new CodeEffect("sharp_click", .0f, 1.0f);
			seq.AddEffect(0.3, eff);

			eff = new CodeEffect("sharp_tick", .0f, 1.0f);
			seq.AddEffect(0.4, eff);

			eff = new CodeEffect("short_double_click", .0f, 1.0f);
			seq.AddEffect(0.5, eff);

			eff = new CodeEffect("short_double_sharp_tick", .0f, 1.0f);
			seq.AddEffect(0.6, eff);

			eff = new CodeEffect("triple_click", .0f, 1.0f);
			seq.AddEffect(0.7, eff);

			return seq;
		}

		public static CodeSequence DoubleClickImpact()
		{
			CodeSequence seq = new CodeSequence();
			CodeEffect eff = new CodeEffect("double_click", 0.00f, 1.0f);

			seq.AddEffect(0, eff);
			eff = new CodeEffect("buzz", .05f, 1.0f);
			seq.AddEffect(.05, eff);
			eff = new CodeEffect("buzz", .10f, .6f);
			seq.AddEffect(.10, eff);
			eff = new CodeEffect("buzz", .15f, .2f);
			seq.AddEffect(.2, eff);

			return seq;
		}

		public static CodeSequence Shimmer()
		{
			CodeSequence seq = new CodeSequence();
			CodeEffect eff = new CodeEffect("double_click", 0.00f, 1.0f);

			//This is from the NS.DoD.Shimmer.sequence reimplemented as CodeSequence. this is because we don't yet have CodeSequence+File Sequence cross use.
			//{ "time" : 0.0, "effect" : "transition_hum", "strength" : 0.1, "duration" : 0.05},
			//{ "time" : 0.05, "effect" : "hum", "strength" : 0.1, "duration" : 0.1},
			//{ "time" : 0.15, "effect" : "hum", "strength" : 0.5, "duration" : 0.1},
			//{ "time" : 0.25, "effect" : "hum", "strength" : 0.1, "duration" : 0.1}

			seq.AddEffect(0, eff);
			eff = new CodeEffect("transition_hum", 0.05f, 0.1f);
			seq.AddEffect(.05, eff);
			eff = new CodeEffect("hum", .1f, .1f);
			seq.AddEffect(.15, eff);
			eff = new CodeEffect("hum", .1f, .5f);
			seq.AddEffect(.25, eff);
			eff = new CodeEffect("hum", .1f, .1f);

			return seq;
		}

		public static CodeSequence ClickHumDoubleClick()
		{
			CodeSequence seq = new CodeSequence();

			CodeEffect eff = new CodeEffect("click", 0.0f, 1.0f);
			seq.AddEffect(0, eff);

			eff = new CodeEffect("transition_hum", .50f, 1.0f);
			seq.AddEffect(0.10, eff);

			eff = new CodeEffect("double_click", .0f, 1.0f);
			seq.AddEffect(0.6, eff);

			return seq;
		}

		public static CodeSequence PulseBumpPulse()
		{
			CodeSequence seq = new CodeSequence();

			CodeEffect eff = new CodeEffect("pulse", 0.40f, 0.7f);
			seq.AddEffect(0.0, eff);

			eff = new CodeEffect("bump", .0f, 1.0f);
			seq.AddEffect(0.40, eff);

			eff = new CodeEffect("pulse", 0.0f, 0.2f);
			seq.AddEffect(0.55, eff);

			return seq;
		}

		public static CodeSequence TripleClickFuzzFalloff()
		{
			CodeSequence seq = new CodeSequence();

			CodeEffect eff = new CodeEffect("triple_click", 0.20f, 0.7f);
			seq.AddEffect(0.0, eff);

			eff = new CodeEffect("fuzz", .20f, 1.0f);
			seq.AddEffect(0.2, eff);

			eff = new CodeEffect("fuzz", .20f, 0.5f);
			seq.AddEffect(0.4, eff);

			return seq;
		}

		/// <summary>
		/// Creating a randomized code sequence is totally doable.
		/// This is a less than ideal approach (because static method)
		/// In your code you shouldn't use a static method like this (Do as I say, not as I do)
		/// </summary>
		/// <param name="randSeed">Hand in a random seed (or better yet, don't use random in static functions</param>
		/// <returns>A CodeSequence reference for use in Impulses</returns>
		public static CodeSequence RandomPulses(int randSeed)
		{
			//Debug.Log(randSeed + "\n");
			System.Random rand = new System.Random(randSeed);

			CodeSequence seq = new CodeSequence();

			float dur = ((float)rand.Next(0, 15)) / 10;
			float delay = ((float)rand.Next(0, 10)) / 20;
			CodeEffect eff = new CodeEffect("pulse", dur, ((float)rand.Next(0, 10)) / 10);
			seq.AddEffect(0.0, eff);
			float offset = dur;

			dur = ((float)rand.Next(0, 15)) / 20;
			delay = ((float)rand.Next(0, 8)) / 20;
			//Debug.Log(dur + "\n");
			eff = new CodeEffect("pulse", dur, ((float)rand.Next(0, 10)) / 10);
			seq.AddEffect(offset + delay, eff);
			offset = dur;

			dur = ((float)rand.Next(0, 15)) / 20;
			delay = ((float)rand.Next(0, 8)) / 20;
			//Debug.Log(dur + "\n");
			eff = new CodeEffect("pulse", dur, ((float)rand.Next(0, 10)) / 10);
			seq.AddEffect(offset + delay, eff);

			return seq;
		}


		/// <summary>
		/// Creating a randomized code sequence is totally doable.
		/// This is a less than ideal approach (because static method)
		/// In your code you shouldn't use a static method like this (Do as I say, not as I do)
		/// This one is about picking three effects at random (with random strength levels as well)
		/// </summary>
		/// <param name="randSeed">Hand in a random seed (or better yet, don't use random in static functions</param>
		/// <returns>A CodeSequence reference for use in Impulses</returns>
		public static CodeSequence ThreeRandomEffects(int randSeed)
		{
			//Debug.Log(randSeed + "\n");
			System.Random rand = new System.Random(randSeed);

			CodeSequence seq = new CodeSequence();

			int effIndex = rand.Next(0, SuitImpulseDemo.effectOptions.Length);

			CodeEffect eff = new CodeEffect(SuitImpulseDemo.effectOptions[effIndex], 0.0f, ((float)rand.Next(2, 10)) / 10);
			seq.AddEffect(0.0, eff);

			effIndex = rand.Next(0, SuitImpulseDemo.effectOptions.Length);
			eff = new CodeEffect(SuitImpulseDemo.effectOptions[effIndex], 0.0f, ((float)rand.Next(2, 10)) / 10);
			seq.AddEffect(.20f, eff);

			effIndex = rand.Next(0, SuitImpulseDemo.effectOptions.Length);
			eff = new CodeEffect(SuitImpulseDemo.effectOptions[effIndex], 0.0f, ((float)rand.Next(2, 10)) / 10);
			seq.AddEffect(.4f, eff);

			return seq;
		}


		/// <summary>
		/// A VERY random effect. More just for showing haptic varion
		/// </summary>
		/// <param name="randSeed"></param>
		/// <returns></returns>
		public static CodeSequence VeryRandomEffect(int randSeed)
		{
			//Debug.Log(randSeed + "\n");
			System.Random rand = new System.Random(randSeed);

			CodeSequence seq = new CodeSequence();

			int effIndex = rand.Next(0, SuitImpulseDemo.effectOptions.Length);

			float dur = ((float)rand.Next(0, 6)) / 3;
			float delay = 0;
			float offset = 0;
			CodeEffect eff = null;

			int HowManyEffects = rand.Next(2, 11);
			//Debug.Log("How many effects: " + HowManyEffects + "\n");
			for (int i = 0; i < HowManyEffects; i++)
			{
				effIndex = rand.Next(0, SuitImpulseDemo.effectOptions.Length);
				dur = ((float)rand.Next(0, 6)) / 3;
				delay = ((float)rand.Next(0, 8)) / 20;
				eff = new CodeEffect(SuitImpulseDemo.effectOptions[effIndex], dur, ((float)rand.Next(0, 10)) / 10);
				seq.AddEffect(offset + delay, eff);
				offset = dur;
			}

			return seq;
		}
	}
}