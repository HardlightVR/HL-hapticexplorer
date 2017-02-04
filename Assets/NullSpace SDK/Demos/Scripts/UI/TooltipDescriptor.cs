using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace NullSpace.SDK.Demos
{
	[RequireComponent(typeof(EventTrigger))]
	public class TooltipDescriptor : MonoBehaviour
	{
		public string TooltipName;
		[Multiline]
		public string DetailedTooltip;
		private Color32 backgroundColor = new Color32(255, 255, 255, 200);
		public Color32 BackgroundColor
		{
			get { return backgroundColor; }
			set { backgroundColor = value; }
		}

		EventTrigger et;

		void Start()
		{
			et = GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener((eventData) =>
				{
					//Debug.Log("Show " + TooltipName + "\n");
					ExplorerTooltip.Tooltip.ShowTooltip(this);
				}
			);
			et.triggers.Add(entry);


			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerExit;
			entry.callback.AddListener((eventData) =>
			{
				//Debug.Log("Left " + TooltipName + "\n");

				ExplorerTooltip.Tooltip.HideTooltip();
			}
			);
			et.triggers.Add(entry);
		}

		public static TooltipDescriptor AddDescriptor(GameObject go, string name, string description)
		{
			TooltipDescriptor desc = go.AddComponent<TooltipDescriptor>();
			desc.TooltipName = name;
			desc.DetailedTooltip = description;
			return desc;
		}
	}
}