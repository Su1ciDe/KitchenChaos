using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
	public class HostDisconnectUI : BaseUI
	{
		[SerializeField] private Button playAgainButton;

		private void Start()
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
			playAgainButton.onClick.AddListener(PlayAgain);

			Hide();
		}

		private void OnClientDisconnect(ulong clientId)
		{
			if (clientId == NetworkManager.ServerClientId)
			{
				Show();
			}
		}

		private void PlayAgain()
		{
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.Scenes.MainMenu);
		}
	}
}