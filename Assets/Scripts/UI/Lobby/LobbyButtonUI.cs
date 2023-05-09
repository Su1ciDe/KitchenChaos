using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby
{
	public class LobbyButtonUI : MonoBehaviour
	{
		[SerializeField] private TMP_Text txtLobbyName;
		
		private Unity.Services.Lobbies.Models.Lobby lobby;

		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(LobbyButtonClicked);
		}

		private void LobbyButtonClicked()
		{
			KitchenLobby.Instance.JoinById(lobby.Id);
		}

		public void SetLobby(Unity.Services.Lobbies.Models.Lobby lobby)
		{
			this.lobby = lobby;
			txtLobbyName.SetText(lobby.Name);
		}
	}
}