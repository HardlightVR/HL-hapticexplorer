using UnityEngine;
using System.Collections;

namespace NullSpace.SDK.Demos
{
	public class LightningExample : MonoBehaviour
	{
		//Should we start a shock.
		public bool ShouldShock = true;

		//Set to False to disrupt the current shock
		public bool CurrentlyShocking = true;

		//The Shock Impulse we want to make. We save this to only pay the more expensive cost
		ImpulseGenerator.Impulse shockImpulse;

		private void Start()
		{
			string whichEffect = "pulse";           //What's more electrical than pulses.
			float totalImpulseDuration = .35f;      //How long does the shock take to traverse to the heart.
			float effectDuration = 0.0f;            //0.0 defaults to the natural duration of the pulse effect.
			float effectStrength = 1;               //How strong is the pulse effect

			//Create the Impulse object (which can be told to play, which instantiates what it 'is')
			shockImpulse = ImpulseGenerator.BeginTraversingImpulse(AreaFlag.Forearm_Right, AreaFlag.Chest_Left);

			// This sets the duration to be .25 seconds
			shockImpulse.WithDuration(totalImpulseDuration);

			//This defines the base effect (which needs an effect name, a strength and a duration)
			shockImpulse.WithEffect(whichEffect, effectDuration, effectStrength);
		}

		void Update()
		{
			//IF we should shock and we aren't currently
			if (ShouldShock && !CurrentlyShocking)
			{
				//Start the coroutine!
				StartCoroutine(ShockPlayer());
				ShouldShock = false;
			}
		}

		//
		IEnumerator ShockPlayer()
		{
			//This serves as the 'Is Coroutine Running' variable
			CurrentlyShocking = true;

			//This is a construct for our repetition of the traversal effect.
			float delay = .4f;                      

			//Prevent the loop/effect from running forever.
			int breakout = 0;

			//A handle for canceling the shock when the condition is no longer true.
			HapticHandle shockHandle = null;

			//This is the loop that will re-start the impulse
			//Remember that CurrentShocking is a bool from outside this function. Meaning it can be turned off and then we leave the loop.
			while (CurrentlyShocking && breakout < 25)
			{
				//Finally, don't forget to play the effect.
				shockHandle = shockImpulse.Play();

				//Wait before the next zap
				yield return new WaitForSeconds(delay);

				//Max of X Zaps.
				breakout++;
			}

			//If we stop shocking the player, we want to clean up the last effect played. This means when the player stops touching the outlet, they stop getting shocked.
			shockHandle.Reset();

			//Mark that we aren't shocking the player.
			CurrentlyShocking = false;
		}
	}
}