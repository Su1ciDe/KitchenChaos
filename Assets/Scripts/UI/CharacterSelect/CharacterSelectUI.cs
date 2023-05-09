using Managers;
using Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.CharacterSelect
{
	public class CharacterSelectUI : BaseUI
	{
		[SerializeField] private Button btnReady;
		[SerializeField] private Button btnMainMenu;

		[Space]
		[SerializeField] private TMP_Text txtLobbyName;
		[SerializeField] private TMP_InputField inputLobbyCode;

		private void Awake()
		{
			btnReady.onClick.AddListener(ClickReady);
			btnMainMenu.onClick.AddListener(ClickMainMenu);
		}

		private void Start()
		{
			var lobby = KitchenLobby.Instance.Lobby;
			txtLobbyName.SetText("Lobby Name: " + lobby.Name);
			inputLobbyCode.SetTextWithoutNotify(lobby.LobbyCode);
		}

		private void ClickMainMenu()
		{
			KitchenLobby.Instance.LeaveLobby();
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.Scenes.MainMenu);
		}

		private void ClickReady()
		{
			CharacterSelectManager.Instance.SetPlayerReady();
		}
	}
}