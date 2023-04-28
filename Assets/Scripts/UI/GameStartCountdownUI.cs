using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
	public class GameStartCountdownUI : BaseUI
	{
		[SerializeField] private TMP_Text txtCountdown;

		private void OnEnable()
		{
			GameManager.Instance.OnStateChanged += OnStateChanged;
		}

		private void Update()
		{
			txtCountdown.SetText(Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer()).ToString());
		}

		private void OnStateChanged(GameManager.State state)
		{
			if (state == GameManager.State.CountdownToStart)
				Show();
			else
				Hide();
		}
	}
}