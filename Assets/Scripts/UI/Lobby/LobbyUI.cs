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
	}
}