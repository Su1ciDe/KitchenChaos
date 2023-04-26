using Interfaces;
using ScriptableObjects;
using UnityEngine;

namespace KitchenObjects
{
	public class KitchenObject : MonoBehaviour
	{
		private IKitchenObjectParent kitchenObjectParent;
		public IKitchenObjectParent KitchenObjectParent
		{
			get => kitchenObjectParent;
			set
			{
				if (kitchenObjectParent != null)
					kitchenObjectParent.KitchenObject = null;

				kitchenObjectParent = value;
				kitchenObjectParent.KitchenObject = this;

				transform.SetParent(kitchenObjectParent.KitchenObjectPoint);
				transform.localPosition = Vector3.zero;
			}
		}
		[SerializeField] private KitchenObjectSO kitchenObjectSO;

		public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;

		public void DestroySelf()
		{
			kitchenObjectParent = null;
			Destroy(gameObject);
		}

		public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
		{
			var kitchenObj = Instantiate(kitchenObjectSO.Prefab);
			kitchenObj.KitchenObjectParent = kitchenObjectParent;

			return kitchenObj;
		}
	}
}