using Network;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Lobby
{
	public class TestingLobbyUI : MonoBehaviour
	{
		[SerializeField] private Button btnCreateGame;
		[SerializeField] private Button btnJoinGame;

		private void Awake()
		{
			btnCreateGame.onClick.AddListener(CreateGame);
			btnJoinGame.onClick.AddListener(JoinGame);
		}

		private void CreateGame()
		{
			KitchenGameMultiplayer.Instance.StartHost();
			Loader.LoadNetwork(Loader.Scenes.CharacterSelectScene);
		}

		private void JoinGame()
		{
			KitchenGameMultiplayer.Instance.StartClient();
		}
	}
}