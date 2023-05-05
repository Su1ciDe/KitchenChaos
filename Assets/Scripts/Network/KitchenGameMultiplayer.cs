using System.Collections.Generic;
using Interfaces;
using KitchenObjects;
using Managers;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utility;

namespace Network
{
	public class KitchenGameMultiplayer : NetworkBehaviour
	{
		public static KitchenGameMultiplayer Instance { get; private set; }

		[SerializeField] private KitchenObjectListSO kitchenObjectListSO;
		[Space]
		[SerializeField] private List<Vector3> playerSpawnPositions = new List<Vector3>();
		public List<Vector3> PlayerSpawnPositions => playerSpawnPositions;

		private const int MAX_PLAYERS_COUNT = 4;

		public event UnityAction OnTryingToJoinGame;
		public event UnityAction OnFailedToJoinGame;

		private void Awake()
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public void StartHost()
		{
			NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
			NetworkManager.Singleton.StartHost();
		}

		public void StartClient()
		{
			OnTryingToJoinGame?.Invoke();

			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
			NetworkManager.Singleton.StartClient();
		}

		private void OnClientDisconnected(ulong clientId)
		{
			OnFailedToJoinGame?.Invoke();
		}

		private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
		{
			if (!SceneManager.GetActiveScene().name.Equals(Loader.Scenes.CharacterSelectScene.ToString()))
			{
				response.Approved = false;
				response.Reason = "Game has already started!";
				return;
			}

			if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS_COUNT)
			{
				response.Approved = false;
				response.Reason = "Game has maximum amount of players!";
				return;
			}

			response.Approved = true;

			// if (GameManager.Instance.IsWaitingToStart)
			// {
			// 	response.Approved = true;
			// 	response.CreatePlayerObject = true;
			// }
			// else
			// {
			// 	response.Approved = false;
			// }
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