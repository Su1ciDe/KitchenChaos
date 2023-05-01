using Managers;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
	public class GameOverUI : BaseUI
	{
		[SerializeField] private TMP_Text txtRecipesDelivered;
		[SerializeField] private Button playAgainButton;

		private void Start()
		{
			playAgainButton.onClick.AddListener(PlayAgain);
			Hide();
		}

		private void PlayAgain()
		{
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.Scenes.MainMenu);
		}

		private void OnEnable()
		{
			GameManager.Instance.OnStateChanged += OnStateChanged;
		}

		private void OnStateChanged(GameManager.State state)
		{
			if (state == GameManager.State.GameOver)
				Show();
			else
				Hide();
		}

		protected override void Show()
		{
			txtRecipesDelivered.SetText(DeliveryManager.Instance.SuccessfulRecipesAmount.ToString());
			base.Show();
		}
	}
}