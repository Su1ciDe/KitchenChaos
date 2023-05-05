using Network;

namespace UI.Lobby
{
	public class ConnectingUI : BaseUI
	{
		private void Start()
		{
			Hide();
		}

		private void OnEnable()
		{
			KitchenGameMultiplayer.Instance.OnTryingToJoinGame += OnTryingToJoinGame;
			KitchenGameMultiplayer.Instance.OnFailedToJoinGame += OnFailedToJoinGame;
		}

		private void OnDisable()
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