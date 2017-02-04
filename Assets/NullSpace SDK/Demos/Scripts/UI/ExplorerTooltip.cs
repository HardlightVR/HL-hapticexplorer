using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace NullSpace.SDK.Demos
{
	public class ExplorerTooltip : MonoBehaviour
	{
		public static ExplorerTooltip Tooltip;
		public enum TooltipMode { None, NamesOnly, NameAndDescription }
		public TooltipMode CurrentMode = TooltipMode.NameAndDescription;
		public Image TooltipBackground;
		public Text TooltipNameDisplay;
		public Text TooltipDescriptionDisplay;

		private List<TooltipDescriptor> stack;
		private RectTransform rectTooltip;

		public int StackCount = 0;
		public int TooltipIndex = 0;
		public LayerMask RayCanHit;

		[SerializeField]
		private bool visible;
		public bool Visible
		{
			get { return visible; }
			set
			{
				//Set the background's activity
				TooltipBackground.gameObject.SetActive(value);

				//Set the name and other elements accordingly.
				if (value && CurrentMode != TooltipMode.None)
				{
					TooltipNameDisplay.gameObject.SetActive(true);
					if (CurrentMode == TooltipMode.NameAndDescription)
						TooltipDescriptionDisplay.gameObject.SetActive(true);
					else
						TooltipDescriptionDisplay.gameObject.SetActive(false);
				}
				else
				{
					TooltipNameDisplay.gameObject.SetActive(false);
					TooltipDescriptionDisplay.gameObject.SetActive(false);
				}

				visible = value;
			}
		}

		void Start()
		{
			stack = new List<TooltipDescriptor>();
			rectTooltip = GetComponent<RectTransform>();
			Tooltip = this;
			TooltipBackground = GetComponentInChildren<Image>();
			TooltipNameDisplay = TooltipBackground.transform.FindChild("Tooltip Name").GetComponent<Text>();
			TooltipDescriptionDisplay = TooltipBackground.transform.FindChild("Tooltip Description").GetComponent<Text>();
			Visible = false;
		}

		//void Update()
		//{
		//	//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//	//Debug.Log(Camera.main.name + "\n" + ray.ToString());
		//	//RaycastHit[] hit = Physics.RaycastAll(ray, 1000, RayCanHit);
		//	//Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + ray.direction * 100, Color.green, .1f);

		//	////Code to be place in a MonoBehaviour with a GraphicRaycaster component
		//	//GraphicRaycaster gr = this.GetComponent<GraphicRaycaster>();
		//	////Create the PointerEventData with null for the EventSystem
		//	//PointerEventData ped = new PointerEventData(null);
		//	////Set required parameters, in this case, mouse position
		//	//ped.position = Input.mousePosition;
		//	////Create list to receive all results
		//	//List<RaycastResult> results = new List<RaycastResult>();
		//	////Raycast it
		//	//gr.Raycast(ped, results);

		//	//if (results != null && results.Count > 0)
		//	//{
		//	//	Debug.Log("Hit: " + results.Count + "\n");
		//	//	//Reduce the tooltip to the deepest element?
		//	//	if (TooltipIndex > ped.hovered.Count)
		//	//	{
		//	//		TooltipIndex = ped.hovered.Count - 1;
		//	//	}

		//	//	TooltipDescriptor ttD = ped.hovered[TooltipIndex].gameObject.GetComponent<TooltipDescriptor>();
		//	//	//Get the TooltipDescriptor from the TooltipIndex's element
		//	//	if (ttD)
		//	//	{
		//	//		ShowTooltip(ttD);
		//	//	}
		//	//	//Display the tooltip for the TooltipIndex element.

		//	//	if (Visible)
		//	//	{
		//	//		//Position the tooltip at the mouse.
		//	//	}

		//	//	//If the user presses L/R, adjust the TooltipIndex element.
		//	//}
		//	//else
		//	//{
		//	//	HideTooltip();
		//	//}
		//	//Raycast the mouse
		//	//If we hit something with a tooltipInfo.

		//	//Show a tooltip
		//}

		void Update()
		{
			if (Visible && stack.Count > 0 && stack[stack.Count - 1] != null)
			{
				if (stack[stack.Count - 1].gameObject.activeInHierarchy)
				{
					PositionTooltip(stack[stack.Count - 1]);
				}
				else
				{
					HideTooltip();
				}
			}
		}

		public void PositionTooltip(TooltipDescriptor desc)
		{
			Vector3 pos = Input.mousePosition;

			//Check the 4 directions.
			//if(pos.x + rectTooltip.a

			Vector2 direction = new Vector2(1, -1);

			if (pos.x + rectTooltip.sizeDelta.x < Screen.width)
			{
				direction.x = 1;
			}
			else
			{
				direction.x = -1;
			}
			if (pos.y - rectTooltip.sizeDelta.y < 0)
			{
				direction.y = 1;
			}
			else
			{
				direction.y = -1;
			}

			//Debug.Log(direction + "     " + pos + "\t\t Anchored: " + rectTooltip.anchoredPosition + "\n\tSize Delta: " + rectTooltip.sizeDelta + "\n");
			rectTooltip.anchoredPosition = new Vector2(Input.mousePosition.x + direction.x * (15 + rectTooltip.sizeDelta.x / 2), Input.mousePosition.y + direction.y * (15 + rectTooltip.sizeDelta.y / 2));
			//TooltipRT.anchoredPosition
		}

		public void HideTooltip()
		{
			if (stack.Count > 0)
			{
				stack.RemoveAt(stack.Count - 1);
				StackCount = stack.Count;
			}

			if (stack.Count > 0)
			{
				ShowTooltip(stack[stack.Count - 1]);
			}
			else
			{
				Visible = false;
			}
		}
		public void ShowTooltip()
		{
			Visible = true;
		}
		public void ShowTooltip(TooltipDescriptor descriptor)
		{
			if (!stack.Contains(descriptor))
			{
				stack.Add(descriptor);
				StackCount = stack.Count;
			}	
			//Debug.Log("HIT from : " + descriptor.name + " \n");
			TooltipNameDisplay.text = descriptor.TooltipName;
			TooltipDescriptionDisplay.text = descriptor.DetailedTooltip;
			TooltipBackground.color = descriptor.BackgroundColor;
			PositionTooltip(descriptor);
			ShowTooltip();
		}
		public void ShowTooltip(string nameText)
		{
			TooltipNameDisplay.text = nameText;
			ShowTooltip();
		}
		public void ShowTooltip(string nameText, Color backgroundColor)
		{
			TooltipBackground.color = backgroundColor;
			ShowTooltip(nameText);
		}
		//public void ShowTooltip(string newText, Rect tooltipDimensions, Color backgroundColor)
		//{
		//	background.color = backgroundColor;
		//	ShowTooltip(newText);
		//}
	}
}
