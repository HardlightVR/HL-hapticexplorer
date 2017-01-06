using UnityEngine;
using System.Collections;

namespace NullSpace.SDK.Demos
{
	//Helper class that adds prefabs to the container
	//Can be used to clear/remove items
	public class PopulateContainer : MonoBehaviour
	{
		public GameObject prefab;
		public RectTransform container;

		public int count;

		public GameObject AddPrefabToContainerReturn(bool worldPositionStays = false)
		{
			var instance = Instantiate<GameObject>(prefab);
			instance.transform.SetParent(container, worldPositionStays);

			var indexer = instance.GetComponent<ContainerItem>();
			if (indexer) indexer.Index(count++);

			return instance;
		}

		public void AddPrefabToContainer()
		{
			AddPrefabToContainerReturn();
		}

		public void RemoveFromContainer(Transform item)
		{
			item.SetParent(null);
			Destroy(item.gameObject);

			count = 0;
			foreach (Transform child in container)
			{
				var indexer = child.GetComponent<ContainerItem>();
				if (indexer) indexer.Index(count++);
			}
		}

		public void Clear()
		{
			count = 0;
			foreach (Transform child in container)
			{
				var indexer = child.GetComponent<ContainerItem>();
				if (indexer) Destroy(child.gameObject);
			}
		}
	}
}