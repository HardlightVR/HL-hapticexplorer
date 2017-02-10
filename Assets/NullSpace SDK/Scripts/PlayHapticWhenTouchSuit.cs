using UnityEngine;
using System.Collections;

namespace NullSpace.SDK
{
	/// <summary>
	/// A simple haptic playing class that can be used with SuitBodyCollider
	/// Attach this object to your projectiles or colliders that can hit parts of the suit
	/// </summary>
	public class PlayHapticWhenTouchSuit : MonoBehaviour
	{
		[Header("The Namespace the file is located in")]
		public string HapticNamespace = "Ns";

		[Header("The file you want to play (no file extensions)")]
		public string HapticFileName = "click";

		public enum HapticFileType { Sequence, Pattern, Experience}
		[Header("The file you want to play (no file extensions)")]
		public HapticFileType TypeOfFile = HapticFileType.Sequence;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collidedSuit"></param>
		public void PlayHaptic(SuitBodyCollider collidedSuit)
		{
			if (TypeOfFile == HapticFileType.Sequence)
			{
				PlayHapticSequence(collidedSuit.regionID);
			}
			else
			{
				PlayHapticOnBody();
			}
		}

		/// <summary>
		/// Plays the current haptic sequence on the specified area
		/// </summary>
		/// <param name="Where"></param>
		public void PlayHapticSequence(AreaFlag Where)
		{
			if (TypeOfFile == HapticFileType.Sequence)
			{
				Sequence seq = new Sequence(HapticNamespace + "." + HapticFileName);
				seq.CreateHandle(Where).Play();
			}
		}

		/// <summary>
		/// Plays the current haptic effect on all areas of the body.
		/// Sequences will play on all pads.
		/// Experiences/Patterns will play normally.
		/// </summary>
		public void PlayHapticOnBody()
		{
			if (TypeOfFile == HapticFileType.Pattern)
			{
				Pattern pattern = new Pattern(HapticNamespace + "." + HapticFileName);
				pattern.CreateHandle().Play();
			}
			if (TypeOfFile == HapticFileType.Experience)
			{
				Experience exp = new Experience(HapticNamespace + "." + HapticFileName);
				exp.CreateHandle().Play();
			}
			else
			{
				//Default to Play All
				Sequence seq = new Sequence(HapticNamespace + "." + HapticFileName);
				seq.CreateHandle(AreaFlag.All_Areas).Play();
			}
		}
	}
}