using Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
	public class GamePauseUI : BaseUI
	{
		[SerializeField] private Button btnResume;
		[SerializeField] private Button btnOptions;
		[SerializeField] private Button btnMainMenu;

		private void Start()
		{
			btnResume.onClick.AddListener(ClickResume);
			btnOptions.onClick.AddListener(ClickOptions);
			btnMainMenu.onClick.AddListener(ClickMainMenu);

			Hide();
		}

		private void OnEnable()
		{
			GameManager.Instance.OnLocalGamePaused += OnLocalGamePaused;
			GameManager.Instance.OnLocalGameUnpaused += OnLocalGameUnpaused;
		}

		private void ClickResume()
		{
			GameManager.Instance.TogglePauseGame();
		}

		private void ClickMainMenu()
		{
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.Scenes.MainMenu);
		}

		private void ClickOptions()
		{
			Hide();
			OptionsUI.Instance.Show(Show);
		}

		private void OnLocalGameUnpaused()
		{
			Hide();
		}

		private void OnLocalGamePaused()
		{
			Show();
		}

		protected override void Show()
		{
			base.Show();
			btnResume.Select();
		}
	}
}