using Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Lobby
{
	public class LobbyMessageUI : BaseUI
	{
		[SerializeField] private TMP_Text txtMessage;
		[SerializeField] private Button btnClose;

		private void Awake()
		{
			btnClose.onClick.AddListener(CloseClicked);
		}

		private void Start()
		{
			KitchenGameMultiplayer.Instance.OnFailedToJoinGame += FailedToJoinGame;
			KitchenLobby.Instance.OnCreatingLobby += CreatingLobby;
			KitchenLobby.Instance.OnCreatingLobbyFailed += CreatingLobbyFailed;
			KitchenLobby.Instance.OnJoining += Joining;
			KitchenLobby.Instance.OnJoiningFailed += JoiningFailed;

			Hide();
		}

		private void OnDestroy()
		{
			KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= FailedToJoinGame;
			KitchenLobby.Instance.OnCreatingLobby -= CreatingLobby;
			KitchenLobby.Instance.OnCreatingLobbyFailed -= CreatingLobbyFailed;
			KitchenLobby.Instance.OnJoining -= Joining;
			KitchenLobby.Instance.OnJoiningFailed -= JoiningFailed;
		}

		private void ShowMessage(string message)
		{
			Show();
			txtMessage.SetText(message);
		}

		private void CreatingLobby()
		{
			ShowMessage("Creating Lobby...");
		}

		private void CreatingLobbyFailed(string message)
		{
			ShowMessage(message);
		}

		private void FailedToJoinGame()
		{
			ShowMessage(NetworkManager.Singleton.DisconnectReason.Equals("") ? "Failed to connect!" : NetworkManager.Singleton.DisconnectReason);
		}

		private void Joining()
		{
			ShowMessage("Joining Lobby...");
		}

		private void JoiningFailed(string message)
		{
			ShowMessage(message);
		}

		private void CloseClicked()
		{
			Hide();
			Loader.Load(Loader.Scenes.MainMenu);
		}
	}
}