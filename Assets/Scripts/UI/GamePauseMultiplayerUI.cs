using Managers;

namespace UI
{
	public class GamePauseMultiplayerUI : BaseUI
	{
		private void Start()
		{
			GameManager.Instance.OnMultiplayerGamePaused += OnMultiplayerGamePaused;
			GameManager.Instance.OnMultiplayerGameUnpaused += OnMultiplayerGameUnpaused;
			Hide();
		}

		private void OnMultiplayerGameUnpaused()
		{
			Hide();
		}

		private void OnMultiplayerGamePaused()
		{
			Show();
		}
	}
}