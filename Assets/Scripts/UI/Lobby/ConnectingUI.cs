using Network;

namespace UI.Lobby
{
	public class ConnectingUI : BaseUI
	{
		private void Start()
		{
			KitchenGameMultiplayer.Instance.OnTryingToJoinGame += OnTryingToJoinGame;
			KitchenGameMultiplayer.Instance.OnFailedToJoinGame += OnFailedToJoinGame;
			Hide();
		}

		private void OnDestroy()
		{
			KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= OnTryingToJoinGame;
			KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= OnFailedToJoinGame;
		}

		private void OnTryingToJoinGame()
		{
			Show();
		}

		private void OnFailedToJoinGame()
		{
			Hide();
		}
	}
}