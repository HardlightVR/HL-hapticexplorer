/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://www.hardlightvr.com/wp-content/uploads/2017/01/NullSpace-SDK-License-Rev-3-Jan-2016-2.pdf
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using UnityEngine;

using System.Collections;
using NullSpace.SDK;

namespace NullSpace.SDK.Demos
{
	public class TestHaptics : MonoBehaviour
	{
		Rigidbody myRB;
	
		/// <summary>
		/// This is controlled based on the suit and contents within NSEnums.
		/// This number exists for easier testing of experimental hardware.
		/// </summary>
		private bool massage = false;
		public float CodeHapticDuration = 5.5f;
		Sequence clicker;
		HapticHandle clickerHandle;
		int[] playingIDs;
		//	public Sequence s;

		void Awake()
		{
			
		}
		void Start()
		{
			myRB = GameObject.Find("Haptic Trigger").GetComponent<Rigidbody>();

			//var a = new Sequence("ns.basic.click_click_click");
			//a.CreateHandle(AreaFlag.All_Areas).Play();	

			clicker = new Sequence("ns.demos.five_second_hum");
			clickerHandle = clicker.CreateHandle(AreaFlag.All_Areas);
			//clicker.CreateHandle(AreaFlag.All_Areas).Play();
		}


		IEnumerator MoveFromTo(Vector3 pointA, Vector3 pointB, float time)
		{
			while (massage)
			{

				float t = 0f;
				while (t < 1f)
				{
					t += Time.deltaTime / time; // sweeps from 0 to 1 in time seconds
					myRB.transform.position = Vector3.Lerp(pointA, pointB, t); // set position proportional to t
					yield return 0; // leave the routine and return here in the next frame
				}
				t = 0f;

				while (t < 1f)
				{
					t += Time.deltaTime / time; // sweeps from 0 to 1 in time seconds
					myRB.transform.position = Vector3.Lerp(pointB, pointA, t); // set position proportional to t
					yield return 0; // leave the routine and return here in the next frame
				}


			}
		}

		void Update()
		{
			bool moving = false;
			float velVal = 350;

			#region Direction Controls
			if (Input.GetKey(KeyCode.LeftArrow) && myRB.transform.position.x > -8)
			{
				myRB.AddForce(Vector3.left * velVal);
			}
			if (Input.GetKey(KeyCode.RightArrow) && myRB.transform.position.x < 8)
			{
				myRB.AddForce(Vector3.right * velVal);
			}
			if (Input.GetKey(KeyCode.UpArrow) && myRB.transform.position.y < 8)
			{
				myRB.AddForce(Vector3.up * velVal);
			}
			if (Input.GetKey(KeyCode.DownArrow) && myRB.transform.position.y > -8)
			{
				myRB.AddForce(Vector3.down * velVal);
			}

			if (!moving)
			{
				myRB.velocity = Vector3.zero;
			}
			#endregion

			#region Application Quit Code
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
			#endregion
		}

		public void OnGUI()
		{
			if (GUI.Button(new Rect(50, 50, 120, 50), "Test Experience"))
			{
				new Experience("ns.demos.recharge_demo").CreateHandle().Play();
			}
			if (GUI.Button(new Rect(50, 100, 120, 50), "Test Pattern"))
			{
				new Pattern("ns.demos.recharge_reverse").CreateHandle().Play();
			}

			if (GUI.Button(new Rect(50, 150, 150, 50), "Test Sequence"))
			{
				new Sequence("ns.demos.five_second_hum").CreateHandle(AreaFlag.Chest_Both).Play();
			}

			if (GUI.Button(new Rect(400, 100, 100, 50), "Play Hum"))
			{
				clickerHandle.Play();
			}
			if (Input.GetKeyDown(KeyCode.I))
			{
				//	new Sequence("ns.click").CreateHandle(AreaFlag.Lower_Ab_Both).Play();
				new Pattern("ns.demos.pulse").CreateHandle().Play();
			}
			if (Input.GetKeyDown(KeyCode.O))
			{
				//	new Sequence("ns.click").CreateHandle(AreaFlag.Lower_Ab_Both).Play();
				//new Pattern("ns.demos.pulse").CreateHandle().Play();
			}
			if (GUI.Button(new Rect(500, 100, 100,50), "Pause Hum")) {
				clickerHandle.Pause();
			}
			if (GUI.Button(new Rect(600, 100, 100, 50), "Reset Hum"))
			{
				clickerHandle.Reset();
			}
			if (GUI.Button(new Rect(740, 100, 120, 50), "Clear All Effects"))
			{
				NSManager.Instance.ClearAllEffects();
			}
			if (GUI.Button(new Rect(50, 250, 150, 50), "Massage"))
			{
				massage = !massage;
				StartCoroutine(MoveFromTo(new Vector3(0, -3.5f, 0), new Vector3(0, 4.5f, 0), .8f));
			}

			if (GUI.Button(new Rect(50, 200, 100, 40), "Jolt Left Body"))
			{
				new Sequence("ns.click").CreateHandle(AreaFlag.Left_All).Play();
			}
			if (GUI.Button(new Rect(150, 200, 100, 40), "Jolt Full Body"))
			{
				new Sequence("ns.click").CreateHandle(AreaFlag.All_Areas).Play();
			}
			if (GUI.Button(new Rect(250, 200, 100, 40), "Jolt Right Body"))
			{
				new Sequence("ns.click").CreateHandle(AreaFlag.Right_All).Play();
			}
		}
	}
}
