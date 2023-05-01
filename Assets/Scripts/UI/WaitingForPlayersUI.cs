using Managers;

namespace UI
{
	public class WaitingForPlayersUI : BaseUI
	{
		private void Start()
		{
			GameManager.Instance.OnLocalPlayerReadyChanged += OnLocalPlayerReadyChanged;
			GameManager.Instance.OnStateChanged += OnGameStateChanged;
			Hide();
		}

		private void OnGameStateChanged(GameManager.State state)
		{
			if (state != GameManager.State.CountdownToStart) return;
			Hide();
		}

		private void OnLocalPlayerReadyChanged()
		{
			if (!GameManager.Instance.IsLocalPlayerReady) return;
			Show();
		}
	}
}