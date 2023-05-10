using Network;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.MainMenu
{
	public class MainMenuUI : MonoBehaviour
	{
		[SerializeField] private Button multiplayerButton;
		[SerializeField] private Button singleplayerButton;
		[SerializeField] private Button quitButton;

		private void Awake()
		{
			multiplayerButton.onClick.AddListener(MultiplayerClicked);
			singleplayerButton.onClick.AddListener(SingleplayerClicked);
			quitButton.onClick.AddListener(QuitClicked);
			Time.timeScale = 1;
		}

		private void MultiplayerClicked()
		{
			KitchenGameMultiplayer.IsMultiplayer = true;
			Loader.Load(Loader.Scenes.LobbyScene);
		}

		private void SingleplayerClicked()
		{
			KitchenGameMultiplayer.IsMultiplayer = false;
			Loader.Load(Loader.Scenes.LobbyScene);
		}

		private void QuitClicked()
		{
			Application.Quit();
		}
	}
}