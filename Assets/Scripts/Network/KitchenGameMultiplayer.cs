﻿using System.Collections.Generic;
using Interfaces;
using KitchenObjects;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
	public class KitchenGameMultiplayer : NetworkBehaviour
	{
		public static KitchenGameMultiplayer Instance { get; private set; }

		[SerializeField] private KitchenObjectListSO kitchenObjectListSO;
		[Space]
		[SerializeField] private List<Vector3> playerSpawnPositions = new List<Vector3>();
		public List<Vector3> PlayerSpawnPositions => playerSpawnPositions;

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

		public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
		{
			return kitchenObjectListSO.KitchenObjectSoList.IndexOf(kitchenObjectSO);
		}

		public KitchenObjectSO GetKitchenObjectSOAt(int index)
		{
			return kitchenObjectListSO.KitchenObjectSoList[index];
		}

		public void DestroyKitchenObject(KitchenObject kitchenObject)
		{
			DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
		}

		[ServerRpc(RequireOwnership = false)]
		private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
		{
			if (kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
			{
				if (kitchenObjectNetworkObject.TryGetComponent(out KitchenObject kitchenObject))
				{
					ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);
					kitchenObject.DestroySelf();
				}
			}
		}

		[ClientRpc]
		private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
		{
			if (kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
			{
				if (kitchenObjectNetworkObject.TryGetComponent(out KitchenObject kitchenObject))
					kitchenObject.ClearKitchenObjectOnParent();
			}
		}
	}
}