using Interfaces;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
	public class KitchenGameMultiplayer : NetworkBehaviour
	{
		public static KitchenGameMultiplayer Instance { get; private set; }

		[SerializeField] private KitchenObjectListSO kitchenObjectListSO;

		private void Awake()
		{
			Instance = this;
		}

		public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
		{
			SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
		}

		[ServerRpc(RequireOwnership = false)]
		private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
		{
			var kitchenObjectSO = GetKitchenObjectSOAt(kitchenObjectSOIndex);
			var kitchenObj = Instantiate(kitchenObjectSO.Prefab);
			kitchenObj.NetworkObject.Spawn(true);

			if (kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject))
				if (kitchenObjectParentNetworkObject.TryGetComponent(out IKitchenObjectParent kitchenObjectParent))
					kitchenObj.KitchenObjectParent = kitchenObjectParent;
		}

		private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
		{
			return kitchenObjectListSO.KitchenObjectSoList.IndexOf(kitchenObjectSO);
		}

		private KitchenObjectSO GetKitchenObjectSOAt(int index)
		{
			return kitchenObjectListSO.KitchenObjectSoList[index];
		}
	}
}