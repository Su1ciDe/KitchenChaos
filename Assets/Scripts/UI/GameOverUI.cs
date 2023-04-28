using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
	public class GameOverUI : BaseUI
	{
		[SerializeField] private TMP_Text txtRecipesDelivered;

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