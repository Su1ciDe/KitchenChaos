using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
	public class OptionsUI : Singleton<OptionsUI>
	{
		[SerializeField] private Slider soundEffectSlider;
		[SerializeField] private Slider musicSlider;
		[SerializeField] private Button closeButton;

		private void Start()
		{
			soundEffectSlider.onValueChanged.AddListener(SoundEffectChanged);
			musicSlider.onValueChanged.AddListener(MusicChanged);
			closeButton.onClick.AddListener(Hide);

			soundEffectSlider.value = SoundManager.Instance.Volume * 10;
			musicSlider.value = MusicManager.Instance.Volume * 10;
			GameManager.Instance.OnGameUnpaused += OnGameUnpaused;
			Hide();
		}

		private void OnGameUnpaused()
		{
			Hide();
		}

		private void SoundEffectChanged(float value)
		{
			SoundManager.Instance.ChangeVolume(value / 10f);
		}

		private void MusicChanged(float value)
		{
			MusicManager.Instance.ChangeVolume(value / 10f);
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}

		private void Hide()
		{
			PlayerPrefs.Save();
			gameObject.SetActive(false);
		}
	}
}