using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace NullSpace.SDK.Demos
{
	public class HardlightSuit : MonoBehaviour
	{
		public SuitDefinition _definition;
		[SerializeField]
		public SuitDefinition Definition
		{
			set { _definition = value; }
			get
			{
				if (_definition == null)
				{
					_definition = ScriptableObject.CreateInstance<SuitDefinition>();
					_definition.Init();
				}
				return _definition;
			}
		}
	}
}