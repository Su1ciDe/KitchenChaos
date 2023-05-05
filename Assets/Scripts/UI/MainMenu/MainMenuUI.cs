using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.MainMenu
{
	public class MainMenuUI : MonoBehaviour
	{
		[SerializeField] private Button playButton;
		[SerializeField] private Button quitButton;

		private void Awake()
		{
			playButton.onClick.AddListener(ClickPlay);
			quitButton.onClick.AddListener(ClickQuit);
			Time.timeScale = 1;
		}

		private void ClickPlay()
		{
			Loader.Load(Loader.Scenes.GameScene);
		}

		private void ClickQuit()
		{
			Application.Quit();
		}
	}
}