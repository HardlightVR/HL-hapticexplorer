using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace NullSpace.SDK.Demos
{
	public class ContainerItem : MonoBehaviour
	{
		public int index = -1;
		public void Index(int ind)
		{
			this.index = ind;
		}

		public void RemoveFromContainer()
		{
			var container = transform.GetComponentInParent<PopulateContainer>();
			if (container) container.RemoveFromContainer(transform);
		}
	}
}