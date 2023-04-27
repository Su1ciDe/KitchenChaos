using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
	public class GamePauseUI : MonoBehaviour
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
			GameManager.Instance.OnGamePaused += OnGamePaused;
			GameManager.Instance.OnGameUnpaused += OnGameUnpaused;
		}

		private void ClickResume()
		{
			GameManager.Instance.TogglePauseGame();
		}

		private void ClickMainMenu()
		{
			Loader.Load(Loader.Scenes.MainMenu);
		}

		private void ClickOptions()
		{
			Hide();
			OptionsUI.Instance.Show(Show);
		}

		private void OnGameUnpaused()
		{
			Hide();
		}

		private void OnGamePaused()
		{
			Show();
		}

		private void Show()
		{
			gameObject.SetActive(true);
			btnResume.Select();
		}

		private void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}