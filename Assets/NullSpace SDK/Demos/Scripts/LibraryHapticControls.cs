/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://nullspacevr.com/?wpdmpro=nullspace-developer-agreement
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/


using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;

namespace NullSpace.SDK.Demos
{
	public class LibraryHapticControls : MonoBehaviour
	{
		Rigidbody myRB;
		/// <summary>
		/// This is controlled based on the suit and contents within NSEnums.
		/// This number exists for easier testing of experimental hardware.
		/// </summary>
		private bool massage = false;
		public float Extent = 5f;
	
		void Start()
		{
			myRB = LibraryManager.Inst.greenBox.GetComponent<Rigidbody>();
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
			if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && myRB.transform.position.x > -Extent)
			{
				myRB.AddForce(Vector3.left * velVal);
			}
			if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && myRB.transform.position.x < Extent)
			{
				myRB.AddForce(Vector3.right * velVal);
			}
			if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && myRB.transform.position.y < Extent)
			{
				myRB.AddForce(Vector3.up * velVal);
			}
			if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && myRB.transform.position.y > -Extent)
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

		public void ToggleMassage()
		{
			massage = !massage;
			StartCoroutine(MoveFromTo(new Vector3(0, -3.5f, 0), new Vector3(0, 5.8f, 0), .8f));
		}
		public void ClearAllEffects()
		{
			NSManager.Instance.ClearAllEffects();
		}
		public void ReloadScene()
		{
			Application.LoadLevel(Application.loadedLevel);
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		public void QuitScene()
		{
			Application.Quit();
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
