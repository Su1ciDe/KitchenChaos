using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
	public class GameStartCountdownUI : BaseUI
	{
		[SerializeField] private TMP_Text txtCountdown;

		private int previousCountdownNumber;
		private Animator animator;
		private static readonly int numberPopup = Animator.StringToHash("NumberPopup");

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Start()
		{			Hide();

		}

		private void OnEnable()
		{
			GameManager.Instance.OnStateChanged += OnStateChanged;
		}

		private void Update()
		{
			int countdownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
			txtCountdown.SetText(countdownNumber.ToString());

			if (!previousCountdownNumber.Equals(countdownNumber))
			{
				previousCountdownNumber = countdownNumber;
				animator.SetTrigger(numberPopup);
				SoundManager.Instance.PlayCountdownSound();
			}
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