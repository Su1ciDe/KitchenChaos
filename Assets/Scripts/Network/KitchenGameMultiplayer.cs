using System.Collections.Generic;
using Interfaces;
using KitchenObjects;
using ScriptableObjects;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utility;

namespace Network
{
	public class KitchenGameMultiplayer : NetworkBehaviour
	{
		public static KitchenGameMultiplayer Instance { get; private set; }

		public string PlayerName
		{
			get => PlayerPrefs.GetString("PlayerNameMultiplayer", "Player");
			set => PlayerPrefs.SetString("PlayerNameMultiplayer", value);
		}

		public List<Vector3> PlayerSpawnPositions => playerSpawnPositions;
		public int PlayerColorsCount => playerColors.Count;

		[SerializeField] private KitchenObjectListSO kitchenObjectListSO;
		[Space]
		[SerializeField] private List<Vector3> playerSpawnPositions = new List<Vector3>();
		[Space]
		[SerializeField] private List<Color> playerColors = new List<Color>();

		private NetworkList<PlayerData> playersData;

		public readonly int MAX_PLAYERS_COUNT = 4;

		public event UnityAction OnTryingToJoinGame;
		public event UnityAction OnFailedToJoinGame;
		public event UnityAction OnPlayersDataChanged;

		private void Awake()
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);

			playersData = new NetworkList<PlayerData>();
			playersData.OnListChanged += OnPlayersDataListChanged;
		}

		public void StartHost()
		{
			NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
			NetworkManager.Singleton.OnClientConnectedCallback += OnHost_ClientConnected;
			NetworkManager.Singleton.OnClientDisconnectCallback += OnHost_ClientDisconnected;
			NetworkManager.Singleton.StartHost();
		}

		public void StartClient()
		{
			OnTryingToJoinGame?.Invoke();

			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
			NetworkManager.Singleton.StartClient();
		}

		private void OnHost_ClientConnected(ulong clientId)
		{
			playersData.Add(new PlayerData(clientId, GetFirstUnusedColorId(), PlayerName));
			SetPlayerNameServerRpc(PlayerName);
			SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);

		}

		private void OnClientConnected(ulong clientId)
		{
			SetPlayerNameServerRpc(PlayerName);
			SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
		}

		private void OnClientDisconnected(ulong clientId)
		{
			OnFailedToJoinGame?.Invoke();
		}

		private void OnHost_ClientDisconnected(ulong clientId)
		{
			for (int i = 0; i < playersData.Count; i++)
			{
				var playerData = playersData[i];
				if (playerData.ClientId.Equals(clientId))
					playersData.RemoveAt(i);
			}
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

		private void OnPlayersDataListChanged(NetworkListEvent<PlayerData> changeEvent)
		{
			OnPlayersDataChanged?.Invoke();
		}

		[ServerRpc(RequireOwnership = false)]
		private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
		{
			int playerIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
			var playerData = playersData[playerIndex];
			playerData.PlayerName = playerName;
			playersData[playerIndex] = playerData;
		}

		[ServerRpc(RequireOwnership = false)]
		private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
		{
			int playerIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
			var playerData = playersData[playerIndex];
			playerData.PlayerId = playerId;
			playersData[playerIndex] = playerData;
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

		public bool IsPlayerIndexConnected(int playerIndex) => playerIndex < playersData.Count;

		public PlayerData GetPlayerDataFromIndex(int playerIndex) => playersData[playerIndex];

		public PlayerData GetPlayerDataFromClientId(ulong clientId)
		{
			foreach (var playerData in playersData)
			{
				if (playerData.ClientId.Equals(clientId))
					return playerData;
			}

			return default;
		}

		public PlayerData GetPlayerData() => GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);

		public int GetPlayerIndexFromClientId(ulong clientId)
		{
			for (int i = 0; i < playersData.Count; i++)
			{
				if (playersData[i].ClientId.Equals(clientId))
					return i;
			}

			return -1;
		}

		public Color GetPlayerColor(int colorId) => playerColors[colorId];

		public void ChangePlayerColor(int colorId)
		{
			ChangePlayerColorServerRpc(colorId);
		}

		[ServerRpc(RequireOwnership = false)]
		private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
		{
			if (!IsColorAvailable(colorId)) return;

			int playerIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
			var playerData = playersData[playerIndex];
			playerData.ColorId = colorId;
			playersData[playerIndex] = playerData;
		}

		private bool IsColorAvailable(int colorId)
		{
			foreach (var playerData in playersData)
			{
				if (playerData.ColorId.Equals(colorId))
					return false;
			}

			return true;
		}

		private int GetFirstUnusedColorId()
		{
			for (int i = 0; i < playerColors.Count; i++)
			{
				if (IsColorAvailable(i))
					return i;
			}

			return -1;
		}

		public void KickPlayer(ulong clientId)
		{
			NetworkManager.Singleton.DisconnectClient(clientId, "You have been kicked!");
			OnHost_ClientDisconnected(clientId);
		}
	}
}