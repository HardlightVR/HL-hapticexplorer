/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://nullspacevr.com/?wpdmpro=nullspace-developer-agreement
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NullSpace.SDK;

public class RaycastHaptics : MonoBehaviour
{
	Sequence five_second_hum;

    void Start()
    {
		five_second_hum = new Sequence("ns.demos.five_second_hum");
    }


    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 120, 40), "Stop Everything!"))
		{
			NSManager.Instance.ClearAllEffects();
		}
    }

    public IEnumerator ChangeColorDelayed(GameObject g, Color c, float timeout)
    {
        yield return new WaitForSeconds(timeout);
        g.GetComponent<MeshRenderer>().material.color = c;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue, 3.5f);

            if (Physics.Raycast(ray, out hit, 100))
            {

                if (hit.collider.gameObject.tag == "Haptic Region")
                {
					SuitBodyCollider haptic = hit.collider.gameObject.GetComponent<SuitBodyCollider>();

                    if (haptic != null)
                    {
                        Debug.LogFormat("Starting Haptic: Region ID {0}\n", haptic.regionID);
						five_second_hum.CreateHandle(haptic.regionID).Play();
					

						hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
                        StartCoroutine(ChangeColorDelayed(
                            hit.collider.gameObject, 
                            new Color(227/255f, 227/255f, 227/255f,1f), 
                            5.0f));
                    }
                }
            }
        }
		
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		
	}
}
