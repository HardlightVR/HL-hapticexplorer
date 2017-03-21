/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://nullspacevr.com/?wpdmpro=nullspace-developer-agreement
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace NullSpace.SDK.Demos
{
	public class LibraryHapticControls : MonoBehaviour
	{
		Rigidbody myRB;
		public Camera cam;

		public SuitDemo[] AllDemos;
		/// <summary>
		/// The demo currently used.
		/// We deactivate the old demo and activate the new one if you call SelectSuitDemo
		/// This lets us configure the UI (based on the ActiveObjects/ActiveIfDisabledObjects set in the Inspector)
		/// </summary>
		public SuitDemo CurrentDemo;
		/// <summary>
		/// This is controlled based on the suit and contents within NSEnums.
		/// This number exists for easier testing of experimental hardware.
		/// </summary>
		private bool massage = false;

		/// <summary>
		/// Boundary confines for the green box.
		/// </summary>
		public float Extent = 5f;

		void Start()
		{
			AllDemos = FindObjectsOfType<SuitDemo>();

			//So we can move the green box around
			myRB = LibraryManager.Inst.greenBox.GetComponent<Rigidbody>();

			//If we have a demo
			if (CurrentDemo != null)
			{
				//Turn it on. (To ensure it's needed elements are on)
				SelectSuitDemo(CurrentDemo);
			}
		}

		//Move the massaging green box up and down.
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
			GetInput();
		}

		public void GetInput()
		{
			#region [1-9] SuitDemos
			for (int i = 0; i < AllDemos.Length; i++)
			{
				if (AllDemos[i] != null)
				{
					if (Input.GetKeyDown(AllDemos[i].ActivateHotkey))
					{
						SelectSuitDemo(AllDemos[i]);
					}
				}
			}
			#endregion

			#region [7] Massage Toggle
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				ToggleMassage();
			}
			#endregion

			#region [Space] Clear all effects
			if (Input.GetKeyDown(KeyCode.Space))
			{
				ClearAllEffects();
			} 
			#endregion

			#region [Arrows] Direction Controls
			bool moving = false;
			float velVal = 350;

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

			#region Clicking on SuitBodyCollider
			if (Input.GetMouseButtonDown(0))
			{
				//Where the mouse is 
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				//Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue, 3.5f);

				//Raycast to see if we hit
				if (Physics.Raycast(ray, out hit, 100))
				{
					//Get the clicked SuitBodyCollider
					SuitBodyCollider clicked = hit.collider.gameObject.GetComponent<SuitBodyCollider>();

					//Assuming there is one
					if (clicked != null)
					{
						//Do whatever our current demo wants to do with that click info.
						CurrentDemo.OnSuitClicked(clicked, hit);
					}
				}
			}
			#endregion

			#region Clicking on SuitBodyCollider
			if (Input.GetMouseButton(0))
			{
				//Where the mouse is 
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				//Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue, 3.5f);

				//Raycast to see if we hit
				if (Physics.Raycast(ray, out hit, 100))
				{
					//Get the clicked SuitBodyCollider
					SuitBodyCollider clicked = hit.collider.gameObject.GetComponent<SuitBodyCollider>();

					//Assuming there is one
					if (clicked != null)
					{
						//Do whatever our current demo wants to do with that click info.
						CurrentDemo.OnSuitClicking(clicked, hit);
					}
				}
			}
			else
			{
				if (CurrentDemo != null)
				{
					CurrentDemo.OnSuitNoInput();
				}
			}
			#endregion

			#region [Esc] Application Quit Code
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
			#endregion
		}

		/// <summary>
		/// Library Haptic Controls is set up to take SuitDemos, a simple class for controlling different modes of interaction with the scene
		/// Examples: Impulse Emanation, Impulse Traversal, Region Selection, Tracking, Click to Test
		/// Each SuitDemo enables/disables its critical items (which are set via inspector)
		/// </summary>
		/// <param name="demo"></param>
		public void SelectSuitDemo(SuitDemo demo)
		{
			if (CurrentDemo != null)
			{
				//Debug.Log("Enabling: " + CurrentDemo.GetType().ToString() + "\t\t" + demo.GetType().ToString() + "\n");
				CurrentDemo.DeactivateDemoOverhead();
				CurrentDemo.enabled = false;
			}
			if (demo != null)
			{
				CurrentDemo = demo;
				CurrentDemo.enabled = true;
				CurrentDemo.ActivateDemoOverhead();
			}
		}

		public void ToggleMassage()
		{
			//For moving the green box to auto-play the last played sequence.
			//You can probably do something more inspired for an 'actual massage'
			massage = !massage;
			StartCoroutine(MoveFromTo(new Vector3(0, -3.5f, 0), new Vector3(0, 5.8f, 0), .8f));
		}
		//Hotkey: Spacebar
		public void ClearAllEffects()
		{
			//This stops all haptic effects and clears them out.
			NSManager.Instance.ClearAllEffects();
		}
		public void ReloadScene()
		{
			//The goal of this function is to reload the plugin so we can support mid-exploring editing of haptics files
			Application.LoadLevel(Application.loadedLevel);

			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		//Hotkey: Escape
		public void QuitScene()
		{
			Application.Quit();
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
