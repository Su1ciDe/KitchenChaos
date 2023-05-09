using System.Collections.Generic;
using Network;
using Unity.Netcode;
using UnityEngine.Events;
using Utility;

namespace Managers
{
	public class CharacterSelectManager : NetworkBehaviour
	{
		public static CharacterSelectManager Instance { get; private set; }

		private Dictionary<ulong, bool> playersReadyDictionary;

		public event UnityAction OnReadyChanged;

		private void Awake()
		{
			Instance = this;
			playersReadyDictionary = new Dictionary<ulong, bool>();
		}

		public void SetPlayerReady()
		{
			SetPlayerReadyServerRpc();
		}

		[ServerRpc(RequireOwnership = false)]
		private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
		{
			var clientId = serverRpcParams.Receive.SenderClientId;
			SetPlayerReadyClientRpc(clientId);

			// Handling the client id on server side to combat cheaters
			playersReadyDictionary[clientId] = true;

			bool allClientsReady = true;
			foreach (var connectedClientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				if (playersReadyDictionary.ContainsKey(connectedClientId) && playersReadyDictionary[connectedClientId]) continue;
				// This player is not ready
				allClientsReady = false;
				break;
			}

			// TODO: Could change this so host would start the game manually
			if (allClientsReady)
				StartGame();
		}

		private void StartGame()
		{
			KitchenLobby.Instance.DeleteLobby();
			Loader.LoadNetwork(Loader.Scenes.GameScene);
		}

		[ClientRpc]
		private void SetPlayerReadyClientRpc(ulong clientId)
		{
			playersReadyDictionary[clientId] = true;

			OnReadyChanged?.Invoke();
		}

		public bool IsPlayerReady(ulong clientId) => playersReadyDictionary.ContainsKey(clientId) && playersReadyDictionary[clientId];
		public bool IsPlayerReady(int playerIndex) => IsPlayerReady(KitchenGameMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex).ClientId);
	}
}