using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
	public class GameOverUI : MonoBehaviour
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

		private void Show()
		{
			txtRecipesDelivered.SetText(DeliveryManager.Instance.SuccessfulRecipesAmount.ToString());
			gameObject.SetActive(true);
		}

		private void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}