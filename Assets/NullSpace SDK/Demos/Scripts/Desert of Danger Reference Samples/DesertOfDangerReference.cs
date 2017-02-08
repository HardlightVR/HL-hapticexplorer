using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace NullSpace.SDK.Demos
{
	/// <summary>
	/// Note: These do not all necessarily work.
	/// Please only use this for reference to reimplement them.
	/// </summary>
	public class DesertOfDangerReference : MonoBehaviour
	{
		/////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////
		//  ██████╗ ███████╗███████╗███████╗██████╗ ████████╗     ██████╗ ███████╗
		//  ██╔══██╗██╔════╝██╔════╝██╔════╝██╔══██╗╚══██╔══╝    ██╔═══██╗██╔════╝
		//  ██║  ██║█████╗  ███████╗█████╗  ██████╔╝   ██║       ██║   ██║█████╗  
		//  ██║  ██║██╔══╝  ╚════██║██╔══╝  ██╔══██╗   ██║       ██║   ██║██╔══╝  
		//  ██████╔╝███████╗███████║███████╗██║  ██║   ██║       ╚██████╔╝██║     
		//  ╚═════╝ ╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝   ╚═╝        ╚═════╝ ╚═╝     
		//                                                                        
		//          ██████╗  █████╗ ███╗   ██╗ ██████╗ ███████╗██████╗            
		//          ██╔══██╗██╔══██╗████╗  ██║██╔════╝ ██╔════╝██╔══██╗           
		//          ██║  ██║███████║██╔██╗ ██║██║  ███╗█████╗  ██████╔╝           
		//          ██║  ██║██╔══██║██║╚██╗██║██║   ██║██╔══╝  ██╔══██╗           
		//          ██████╔╝██║  ██║██║ ╚████║╚██████╔╝███████╗██║  ██║           
		//          ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝   
		//        
		//								REFERENCE
		//
		//			This is a reference. This means that not all of these examples work.
		//				It was rushed out the door for a partner to help them reach their deadline. 
		//				My apologies if things aren't as clear as examples should be.
		//
		//
		//			I will repeat: Some of these examples DO NOT WORK. 
		//									Working examples will be clearly labeled in the <summary>. 
		//
		//							I probably should've put that in the giant text.
		/////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////

		#region Reference Data Classes

		/// <summary>
		/// This was the early-wave Desert of Danger implementation to track the player body and to simulate hits.
		/// This class lets you define the players body and query for distances between pads.
		/// Will not work unless you give an object a PlayerTorso, assign refs by hand and then give HapticLocations.
		/// </summary>
		[System.Serializable]
		public class PlayerTorso
		{
			//A poor list to track where all the objects area.
			//Desert of Danger used this an they need to be manually assigned.
			//The coming SuitConfigTool will make this much easier.
			//In Desert of Danger certain body parts were grouped (Torso and Shoulder)
			#region Body Objects
			//Right
			public GameObject RightUpArm;
			public GameObject RightBack;
			public GameObject RightTorsoShoulder;
			public GameObject RightChest;
			public GameObject RightUpAb;
			public GameObject RightMidAb;
			public GameObject RightLowAb;

			//Left
			public GameObject LeftUpArm;
			public GameObject LeftBack;
			public GameObject LeftTorsoShoulder;
			public GameObject LeftChest;
			public GameObject LeftUpAb;
			public GameObject LeftMidAb;
			public GameObject LeftLowAb;
			#endregion

			//A list for randomized selection
			public List<GameObject> regions;

			//Add the objects to the list (for random selection)
			public void Setup()
			{
				regions = new List<GameObject>();

				regions.Add(LeftTorsoShoulder);
				regions.Add(RightTorsoShoulder);
				regions.Add(LeftUpArm);
				regions.Add(RightUpArm);
				regions.Add(LeftBack);
				regions.Add(RightBack);
				regions.Add(LeftChest);
				regions.Add(RightChest);
				regions.Add(LeftUpAb);
				regions.Add(RightUpAb);
				regions.Add(LeftMidAb);
				regions.Add(RightMidAb);
				regions.Add(LeftLowAb);
				regions.Add(RightLowAb);
			}

			/// <summary>
			/// For finding the closest tracked GameObject (which ideally has a HapticLocation on it) to a point.
			/// </summary>
			/// <param name="point">The point in space to check</param>
			/// <param name="impactSize">The size of the check area. It's possible we want to find nothing (if a previous check wasn't absolute)</param>
			/// <returns>The found object, or null if nothing was within the impactSize</returns>
			public GameObject TorsoHit(Vector3 point, float impactSize = 1)
			{
				GameObject closest = RightChest;
				float closestDist = 1000;

				//Look through all the objects. Check which is closest.
				//Simple distance search
				for (int i = 0; i < regions.Count; i++)
				{
					float newDist = Vector3.Distance(point, regions[i].transform.position);

					if (newDist < closestDist)
					{
						closest = regions[i];
						closestDist = newDist;
					}
				}

				return closest;
			}

			public GameObject GetRandomBodyPosition()
			{
				return regions[Random.Range(0, regions.Count)];
			}

			public Vector3 GetRandomLocation()
			{
				return GetRandomBodyPosition().transform.position;
			}
		}

		/// <summary>
		/// A simple script (that you'll have to remove from this reference class)
		/// Put it on a game object to represent where that part of the player is in your game.
		/// It can be as simple as nodes, or objects with SuitBodyCollider (which serves to replace the HapticLocation)
		/// Coming down the dev tool pipeline is the HapticSuitCollider configuration tool - for quickly setting up a player character.
		/// This is included for the Shot and Explosion examples.
		/// </summary>
		public class HapticLocation : MonoBehaviour
		{
			//RegionFlag is a special type of attribute which gives better inspector assignment behavior to a HapticLocation. 
			//For more info look at Scripts/RegionFlawDrawer.cs
			[RegionFlag]
			public AreaFlag MyLocation;
		}

		/// <summary>
		/// This is a class used in Desert of Danger for this sample class.
		/// DOES NOT WORK.
		/// You'll see lines commented out followed by a similar line. The first line was the original line
		/// The second line is the converted line for your use or adaptation.
		/// It will need a bit of further customizing, but I wanted it to compile.
		/// </summary>
		public class ExplosionInfo
		{
			//An ExplosionInfo object is the result of an explosion and it's affect on another object. It can be saved for things like Coroutines as well as passed along to describe the event that took place to other parts of the code.
			//It is also used for creating Impulse Emanation effects that are based on the distance from the explosion.

			//public Explosion source;
			//public Explodable affected;

			//Distance between explosion center and the current target affected
			public float dist;

			//So that way you can avoid negative numbers or force a minimum explosion strength when inside the area.
			//A nuclear bomb would be like new Vector2(500, 1000000); whereas a water balloon would be new Vector2(.25f, 3).
			public Vector2 ClampBlastDamage = new Vector2(0, 1000);
			public Vector3 explosionCenter;

			//If you want to have a multiplier on knockback (one of the ways Desert of Danger had multiple different strengths of explosives)
			public Vector3 forceMultipliers;

			//public ExplosionInfo(/*Explosion src, Explodable afflicted, */Vector3 explosionOrigination, Vector3 explosionMultiplier

			public ExplosionInfo(Vector3 explosionOrigination, Vector3 targetPosition, Vector3 explosionMultiplier)
			{
				//source = src;
				//affected = afflicted;
				explosionCenter = explosionOrigination;
				//dist = Vector3.Distance(explosionCenter, affected.transform.position);
				dist = Vector3.Distance(explosionCenter, targetPosition);
				forceMultipliers = explosionMultiplier;
			}

			//public float BlastStrength()
			public float BlastStrength(float blastRadius, float MinTargetRadius)
			{
				//Say the Total distance is 13. If the target is 4 units away, we want them to take 9 damage before clamping
				//return Mathf.Clamp(source.BlastRadius - (dist - affected.MinSphereRadius / 2), ClampBlastDamage.x, ClampBlastDamage.y);
				return Mathf.Clamp(blastRadius - (dist - MinTargetRadius), ClampBlastDamage.x, ClampBlastDamage.y);
			}

			//For getting information out of ExplosionInfo when it happens.
			//You can reimplement if you need Debug.Log
			//public override string ToString()
			//{
			//	string output = "Explosion [" + source.name + "] applied to [" + affected.name + "]\t" + Mathf.Round(dist * 100) / 100 + " units apart\nCenter: " + explosionCenter + "\t\tBase Force: " + forceMultipliers + "\n";
			//	return output + base.ToString();
			//}
		}

		#endregion

		/// <summary>
		/// This is the Impulse that was used for the Desert of Danger recoil effect.
		/// It is imperfect in the implementation because it doesn't allow for flexibility to pick the effect.
		/// It could also take a more flexible Duration component but the core here is to give you what we used.
		/// Use ImpulseGenerator.CreateImpulse() function instead of modifying this.
		/// This sample does work.
		/// </summary>
		/// <param name="StartLocation">Pick the location to begin. DO NOT PROVIDE MULTIPLE AREAS.</param>
		/// <param name="EndLocation">Pick the destination to reach. DO NOT PROVIDE MULTIPLE AREAS.</param>
		/// <returns>Don't forget to call .Play() on the returned Impulse to create an instance of the haptic effect it defines.</returns>
		public static ImpulseGenerator.Impulse DesertOfDangerRecoil(AreaFlag StartLocation = AreaFlag.Forearm_Left, AreaFlag EndLocation = AreaFlag.Upper_Arm_Left)
		{
			//A simple code sequence
			CodeSequence seq = new CodeSequence();

			//The elements we will add
			CodeEffect eff = new CodeEffect("buzz", 0.00f, 1.0f);
			CodeEffect eff2 = new CodeEffect("buzz", 0.15f, 0.5f);

			//The time stamps of the different effects.
			seq.AddEffect(0, eff);
			seq.AddEffect(.1, eff2);

			//In Desert of Danger, we used a duration of .1 seconds. This means the recoil effect took .1 seconds to hit ALL pads it aimed to. If you hand in different pads, it'll likely want a longer duration.
			//Since we only used the forearm and the upper arm, .1s is more than sufficient.
			//We could've used a file for this, but this was right as we were conceptualizing and beginning the usage of the ImpulseGenerator.
			return ImpulseGenerator.BeginTraversingImpulse(StartLocation, EndLocation)
					.WithDuration(.10f)
					.WithEffect(seq);
		}

		/// <summary>
		/// Shoots the player body somewhere with a simple buzz emanation impulse.
		/// </summary>
		/// <param name="ShotWhere">Self explanator. DO NOT PROVIDE MULTIPLE AREAS.</param>
		/// <returns>Don't forget to call .Play() on the returned Impulse to create an instance of the haptic effect it defines.</returns>
		public static ImpulseGenerator.Impulse DesertOfDangerShot(AreaFlag ShotWhere = AreaFlag.Chest_Right)
		{
			CodeSequence seq = new CodeSequence();

			//This will be slightly different then the default "buzz" effect Impulse. If you ask for an effect with a duration of 0, it'll play the natural duration.
			//Natural durations range from .05s (click, buzz, etc) to .25s (the multiple click families)
			//So by providing a duration of .1, this will be slightly different than the line:
			//		CodeEffect eff = new CodeEffect("buzz", 0.00f, 1.0f);

			CodeEffect eff = new CodeEffect("buzz", 0.10f, 1.0f);
			
			seq.AddEffect(0, eff);

			//The Desert of Danger demo set the entire impulse duration to .25s, 
			//this means that it emanated out from where the playerwas hit.
			return ImpulseGenerator.BeginEmanatingEffect(ShotWhere)
				.WithEffect(seq)
				.WithDuration(.25f);
		}
		
		/// <summary>
		/// Desert of Danger - when the player is hit.
		/// This is not the best way to implement the following. We're still learning the best way to make reusable haptics.
		/// </summary>
		/// <param name="pos">This is used in tandem with the FindNearest() function that finds the AreaFlag that represents the closest point to the 'HitImpulse'</param>
		/// <param name="effect">This takes a different effect which can vary from projectile to projectile.
		/// You could also make a version that takes a CodeSequence for projectiles with varying characteristics</param>
		public void HitImpulse(Vector3 pos, string effect = "buzz")
		{
			//We have a function called FindNearestPos, which finds the closest AreaFlag on the player's body
			AreaFlag loc = AreaFlag.None;//FindNearest(pos);

			//Then if we found something
			if (loc != AreaFlag.None)
			{
				//Then we begin an emanating effect on the player from that point.
				//We want the ENTIRE emanation to last .25 seconds, so we pass in that parameter.
				//Finally, we play the emanation.

				ImpulseGenerator.BeginEmanatingEffect(loc).WithEffect(effect, .1f).WithDuration(.25f).Play();

				//We let the HapticHandle Play returns go out of scope because the effect is short lived and we don't care about stopping it early.
				//If you had something like Daxter (the Weasel character from Jak and Daxter) climbing around on the player's body, you'd want to hold onto both the Impulse and the HapticHandle, that way you can restart the impulse (or change parameters) as well as being able to stop the haptic impulse when Daxter stops moving.

				//You could modify this function to return a HapticHandle or the created Impulse.
			}
			else
			{
				//We shouldn't ever be getting 'hit' without being able to find a valid nearby position.
				//An error is thrown inside of the FindNearest function (also provided)
				Debug.LogWarning("Invalid Hit at " + pos + "\n");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public AreaFlag FindNearest(GameObject PlayerBody, Vector3 source)
		{
			Debug.LogError("This function does not ACTUALLY work in your game. It is provided sample code to give you the gist of what you should do.\nA better sample is coming soon - this was shipped early to help a particular partner hit one of their deadlines. Hopefully you never see this line.\n");

			PlayerTorso torso = new PlayerTorso();
	
			//Maybe get a list of nearby regions?
			GameObject closest = torso.TorsoHit(source);

			//Debug.Log("closest: " + closest.name + "\n");
			if (closest != null && closest.GetComponent<HapticLocation>() != null)
			{
				return closest.GetComponent<HapticLocation>().MyLocation;
			}
			Debug.LogError("Could not find the closest pad. Returning error location\n" + closest.name);
			return AreaFlag.None;
		}

		/// <summary>
		/// [Note: this does not work]
		/// 
		/// </summary>
		/// <param name="playerBody"></param>
		/// <param name="info"></param>
		public void DynamiteExplosionSample(GameObject playerBody, ExplosionInfo info)
		{
			//Again, we checked what was the closest place to the explosion for starting an impulse.
			AreaFlag loc = FindNearest(playerBody, info.explosionCenter);
			if (loc != AreaFlag.None)
			{
				//This is a little gross but the idea is simple: Closer to the explosion, the longer the effect.
				//If you're close to dynamite the visuals take longer to dissipate and you'll spend longer thinking 'Oh man I screwed up' even though you're unharmed.
				//This is accomplished in a couple of ways:

				//The emanation will traverse farther across the user's body.
				int depth = 8;

				//The emanation will restart several times
				int repeats = 3;

				//The starting strength of the effect will be stronger.
				float strength = 1.0f;

				//The delay between the repetitions will be shorter.
				float delay = .15f;

				//We do a simple bit of checking based on the distance from the blast.
				if (info.dist > 0)
				{
					//We base the depth off of the distance from the explosion.
					//There is some magic-numbering going on here. You can tune it or standardize your distances.

					//The farther, the less depth of the emanation
					depth = (int)(8 / info.dist);
					//The farther, the less initial strength - NOTE: this is just Effect strength, not using the Attenuation which is still a bit incomplete.
					strength = 2 / info.dist;
				}

				//The closer the player is to the explosion, the more the explosion will 'reverberate' by repeating.
				repeats = Mathf.RoundToInt(Mathf.Clamp(7 / info.dist, 0, 7));

				//If we're going to experience it a lot, we'll get a shorter delay between the repeats.
				if (repeats > 4)
				{
					delay = .1f;
				}

				//Start at the correct spot.
				ImpulseGenerator.Impulse impulse = ImpulseGenerator.BeginEmanatingEffect(loc, depth)
						//Give the total effect a short duration (which you could choose to modify)
						.WithDuration(.15f)
						//Start with a natural duration click for the initial hit. This is good because it gets the motors going earlier on
						.WithEffect("click", .00f, strength)
						//Finish with a buzz which will last longer than natural duration. This gives the explosion a residual impact feeling
						.WithEffect("buzz", .15f, strength);

				//Finally, we call the coroutine that will repeatedly create new handles of the impulse with Play().
				//Remember, an Impulse is like a prefab or a building plan for haptics. You can 'instantiate' multiple haptic effects off the same Impulse.
				StartCoroutine(RepeatedEmanations(impulse, delay, repeats));
			}
			else
			{
				Debug.LogWarning("Invalid Hit at " + info.explosionCenter + "\n");
			}
		}

		/// <summary>
		/// Repeatedly calls impulse.Play after X delay Y times.
		/// This sample DOES work. It will get baked into the Impulse's functionality later.
		/// </summary>
		/// <param name="impulse">The impulse to repeat</param>
		/// <param name="delay">How long AFTER the previous one do you want to start the next one</param>
		/// <param name="count">How many times you want to play it. (Ideally you should call .Play before you call RepeatedEmanations, otherwise it won't start when you start this coroutine.)</param>
		/// <returns></returns>
		IEnumerator RepeatedEmanations(ImpulseGenerator.Impulse impulse, float delay, int count)
		{
			impulse.Play();
			for (int i = 0; i < count - 1; i++)
			{
				yield return new WaitForSeconds(delay);
				impulse.Play();
			}
		}

		/// <summary>
		/// This is a simple handful of lines. This plays when Walter (the giant red scorpion of player murdering) lands on the ground.
		/// This sample DOES work. 
		/// The intention is to give an effect that goes up the body.
		/// It has been slightly adapted to take different effect families (click, double-click, hum, etc)
		/// </summary>
		public static void GiantScorpionLanding(string effect = "buzz")
		{
			//TWo different Impulses are used here even though the same one could be re-assigned. Two variables are more readable at neglible CPU cost.
			ImpulseGenerator.Impulse leftUp = ImpulseGenerator.BeginTraversingImpulse(AreaFlag.Lower_Ab_Left, AreaFlag.Forearm_Left)
					.WithDuration(0.5f)
					.WithEffect(effect, 0.1f, 1.0f);
			ImpulseGenerator.Impulse rightUp = ImpulseGenerator.BeginTraversingImpulse(AreaFlag.Lower_Ab_Right, AreaFlag.Forearm_Right)
				.WithDuration(0.5f)
				.WithEffect(effect, 0.1f, 1.0f);


			//Don't forget to play the effects
			leftUp.Play();
			rightUp.Play();

			//We could HapticHandle[] if we wanted to stop these prematurely - however they're very short effects, so it's unlikely that will be needed.
			//HapticHandle's can be Stopped or restarted until they (Finish playing AND go out of scope), after that they're gone.
		}

	}
}