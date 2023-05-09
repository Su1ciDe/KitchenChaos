using System.Collections.Generic;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Lobby
{
	public class LobbyUI : MonoBehaviour
	{
		[SerializeField] private TMP_InputField inputPlayerName;
		[SerializeField] private Button btnMainMenu;
		[SerializeField] private Button btnCreateLobby;
		[SerializeField] private Button btnQuickJoin;
		[Space]
		[SerializeField] private TMP_InputField inputLobbyCode;
		[SerializeField] private Button btnJoinViaCode;
		[Header("Lobby List")]
		[SerializeField] private ScrollRect scrollViewLobbyList;
		[SerializeField] private LobbyButtonUI btnLobbyPrefab;
		[Space]
		[SerializeField] private LobbyCreateUI lobbyCreateUI;

		private void Awake()
		{
			btnMainMenu.onClick.AddListener(MainMenuClicked);
			btnCreateLobby.onClick.AddListener(CreateLobbyClicked);
			btnQuickJoin.onClick.AddListener(QuickJoinClicked);
			btnJoinViaCode.onClick.AddListener(JoinViaCodeClicked);
		}

		private void Start()
		{
			inputPlayerName.SetTextWithoutNotify(KitchenGameMultiplayer.Instance.PlayerName);
			inputPlayerName.onValueChanged.AddListener(PlayerNameValueChanged);

			KitchenLobby.Instance.OnLobbyListChanged += LobbyListChanged;

			UpdateLobbyList(new List<Unity.Services.Lobbies.Models.Lobby>());
		}

		private void OnDestroy()
		{
			KitchenLobby.Instance.OnLobbyListChanged -= LobbyListChanged;
		}

		private void LobbyListChanged(List<Unity.Services.Lobbies.Models.Lobby> lobbyList)
		{
			UpdateLobbyList(lobbyList);
		}

		private void PlayerNameValueChanged(string playerName)
		{
			KitchenGameMultiplayer.Instance.PlayerName = playerName;
		}

		private void MainMenuClicked()
		{
			KitchenLobby.Instance.LeaveLobby();
			Loader.Load(Loader.Scenes.MainMenu);
		}

		private void CreateLobbyClicked()
		{
			lobbyCreateUI.Show();
		}

		private void QuickJoinClicked()
		{
			KitchenLobby.Instance.QuickJoin();
		}

		private void JoinViaCodeClicked()
		{
			KitchenLobby.Instance.JoinByCode(inputLobbyCode.text);
		}

		private void UpdateLobbyList(List<Unity.Services.Lobbies.Models.Lobby> lobbyList)
		{
			foreach (Transform child in scrollViewLobbyList.content.transform)
				Destroy(child.gameObject);

			foreach (var lobby in lobbyList)
			{
				var lobbyButton = Instantiate(btnLobbyPrefab, scrollViewLobbyList.content.transform);
				lobbyButton.SetLobby(lobby);
			}
		}
	}
}