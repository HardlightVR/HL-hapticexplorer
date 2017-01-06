/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://nullspacevr.com/?wpdmpro=nullspace-developer-agreement
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullSpace.SDK;

public class SuitRegionSelectorDemo : MonoBehaviour
{
	public Camera cam;
	private Color selectedColor = new Color(127 / 255f, 127 / 255f, 227 / 255f, 1f);
	private Color unselectedColor = new Color(227 / 255f, 227 / 255f, 227 / 255f, 1f);
	public List<SuitBodyCollider> selected;

	void Start()
	{
		selected = new List<SuitBodyCollider>();
		selected = FindObjectsOfType<SuitBodyCollider>().ToList();
		for (int i = 0; i < selected.Count; i++)
		{
			selected[i].GetComponent<MeshRenderer>().material.color = selectedColor;
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
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue, 3.5f);

			if (Physics.Raycast(ray, out hit, 100))
			{
				SuitBodyCollider suit = hit.collider.gameObject.GetComponent<SuitBodyCollider>();
				if (suit != null)
				{
					if (selected.Contains(suit))
					{
						selected.Remove(suit);
						StartCoroutine(ChangeColorDelayed(
						hit.collider.gameObject,
						unselectedColor,
						0.0f));
					}
					else
					{
						hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = selectedColor;
						selected.Add(suit);
					}
				}
			}
		}
	}

	public void SelectAllSuitColliders()
	{
		selected.Clear();
		selected = FindObjectsOfType<SuitBodyCollider>().ToList();
		for (int i = 0; i < selected.Count; i++)
		{
			selected[i].GetComponent<MeshRenderer>().material.color = selectedColor;
		}
	}

	public void DeselectAllSuitColliders()
	{
		for (int i = 0; i < selected.Count; i++)
		{
			selected[i].GetComponent<MeshRenderer>().material.color = unselectedColor;
		}
		selected.Clear();
	}
}
