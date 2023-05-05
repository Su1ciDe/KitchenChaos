using DG.Tweening;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ProgressBarUI : BaseUI
	{
		[SerializeField] private Image barImage;

		private IHasProgress hasProgressCounter;

		private void Awake()
		{
			hasProgressCounter = GetComponentInParent<IHasProgress>();
		}

		private void Start()
		{
			hasProgressCounter.OnProgressChanged += OnProgressChanged;
			barImage.fillAmount = 0;
			Hide();
		}

		private void OnProgressChanged(float fillAmount, bool isAnimated = false)
		{
			fillAmount = Mathf.Clamp01(fillAmount);
			if (isAnimated)
			{
				barImage.DOComplete();
				barImage.DOFillAmount(fillAmount, .075f).SetEase(Ease.InOutSine);
			}
			else
			{
				barImage.fillAmount = fillAmount;
			}

			if (fillAmount.Equals(0) || fillAmount.Equals(1))
				Hide();
			else
				Show();
		}
	}
}