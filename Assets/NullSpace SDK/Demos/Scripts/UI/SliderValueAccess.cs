using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NullSpace.SDK.Demos
{
	public class SliderValueAccess : MonoBehaviour
	{
		private Text MyText;

		public enum ForceType { Integer, TwoDecimals, Effect, String };
		public ForceType DisplayType = ForceType.Integer;
		private float textValue;
		public float TextValue
		{
			get { return textValue; }
			set
			{
				textValue = value;
				if (DisplayType == ForceType.Integer)
				{
					MyText.text = TextValue.ToString();
				}
				else if (DisplayType == ForceType.TwoDecimals)
				{
					MyText.text = ((float)((int)(TextValue * 100)) / 100).ToString();
				}
				else if (DisplayType == ForceType.Effect)
				{
					MyText.text = SuitImpulseDemo.effectOptions[(Mathf.RoundToInt(textValue))];
				}
				else if (DisplayType == ForceType.String)
				{
					MyText.text = SuitImpulseDemo.SampleCodeSequence[(Mathf.RoundToInt(textValue))];
				}
			}
		}

		private void Start()
		{
			MyText = GetComponent<Text>();
		}
	}
}