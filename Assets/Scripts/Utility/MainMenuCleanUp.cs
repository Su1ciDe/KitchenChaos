using Network;
using Unity.Netcode;
using UnityEngine;

namespace Utility
{
	public class MainMenuCleanUp : MonoBehaviour
	{
		private void Awake()
		{
			if (NetworkManager.Singleton)
				Destroy(NetworkManager.Singleton.gameObject);

			if (KitchenGameMultiplayer.Instance)
				Destroy(KitchenGameMultiplayer.Instance.gameObject);

			if (KitchenLobby.Instance)
				Destroy(KitchenLobby.Instance.gameObject);
		}
	}
}