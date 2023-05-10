using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby
{
	public class LobbyCreateUI : BaseUI
	{
		[SerializeField] private TMP_InputField inputLobbyName;
		[SerializeField] private Toggle togglePrivate;
		[SerializeField] private Button btnCreateLobby;
		[Space]
		[SerializeField] private Button btnClose;

		private void Awake()
		{
			btnCreateLobby.onClick.AddListener(CreateLobby);
			btnClose.onClick.AddListener(CloseClicked);
		}

		private void Start()
		{
			Hide();
		}

		private void CreateLobby()
		{
			KitchenLobby.Instance.CreateLobby(inputLobbyName.text, togglePrivate.isOn);
		}

		private void CloseClicked()
		{
			Hide();
		}

		public override void Show()
		{
			base.Show();
			btnCreateLobby.Select();
		}
	}
}