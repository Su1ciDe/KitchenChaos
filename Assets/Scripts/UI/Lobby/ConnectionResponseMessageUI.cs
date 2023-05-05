using Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby
{
	public class ConnectionResponseMessageUI : BaseUI
	{
		[SerializeField] private TMP_Text txtMessage;
		[SerializeField] private Button btnClose;

		private void Awake()
		{
			btnClose.onClick.AddListener(Hide);
		}

		private void Start()
		{
			Hide();
		}

		private void OnEnable()
		{
			KitchenGameMultiplayer.Instance.OnFailedToJoinGame += OnFailedToJoinGame;
		}

		private void OnDestroy()
		{
			KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= OnFailedToJoinGame;
		}

		private void OnFailedToJoinGame()
		{
			Show();
			string reason = NetworkManager.Singleton.DisconnectReason.Equals("") ? "Failed to connect!" : NetworkManager.Singleton.DisconnectReason;
			txtMessage.SetText(reason);
		}
	}
}