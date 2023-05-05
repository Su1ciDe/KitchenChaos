using System.Collections.Generic;
using Unity.Netcode;
using Utility;

namespace Managers
{
	public class CharacterSelectManager : NetworkBehaviour
	{
		public static CharacterSelectManager Instance { get; private set; }
		private Dictionary<ulong, bool> playersReadyDictionary;

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
			// Handling the client id on server side to combat cheaters
			playersReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

			bool allClientsReady = true;
			foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				if (playersReadyDictionary.ContainsKey(clientId) && playersReadyDictionary[clientId]) continue;
				// This player is not ready
				allClientsReady = false;
				break;
			}

			if (allClientsReady)
				Loader.LoadNetwork(Loader.Scenes.GameScene);
		}
	}
}