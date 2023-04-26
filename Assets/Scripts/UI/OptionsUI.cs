﻿using Gameplay;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class OptionsUI : Utility.Singleton<OptionsUI>
	{
		[SerializeField] private Button closeButton;

		[Header("Audio")]
		[SerializeField] private Slider soundEffectSlider;
		[SerializeField] private Slider musicSlider;

		[Header("Controls")]
		[SerializeField] private Button btnMoveUp;
		[SerializeField] private Button btnMoveDown;
		[SerializeField] private Button btnMoveLeft;
		[SerializeField] private Button btnMoveRight;
		[SerializeField] private Button btnInteract;
		[SerializeField] private Button btnInteractAlt;
		[SerializeField] private Button btnPause;
		private TMP_Text txtMoveUp;
		private TMP_Text txtMoveDown;
		private TMP_Text txtMoveLeft;
		private TMP_Text txtMoveRight;
		private TMP_Text txtInteract;
		private TMP_Text txtInteractAlt;
		private TMP_Text txtPause;
		[SerializeField] private GameObject pressToRebindPanel;

		private void Awake()
		{
			InitControlButtons();
		}

		private void Start()
		{
			soundEffectSlider.onValueChanged.AddListener(SoundEffectChanged);
			musicSlider.onValueChanged.AddListener(MusicChanged);
			closeButton.onClick.AddListener(Hide);

			btnMoveUp.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveUp));
			btnMoveDown.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveDown));
			btnMoveLeft.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveLeft));
			btnMoveRight.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveRight));
			btnInteract.onClick.AddListener(() => RebindBinding(GameInput.Binding.Interact));
			btnInteractAlt.onClick.AddListener(() => RebindBinding(GameInput.Binding.InteractAlt));
			btnPause.onClick.AddListener(() => RebindBinding(GameInput.Binding.Pause));

			soundEffectSlider.value = SoundManager.Instance.Volume * 10;
			musicSlider.value = MusicManager.Instance.Volume * 10;

			GameManager.Instance.OnGameUnpaused += OnGameUnpaused;

			UpdateVisuals();
			HidePressToRebindPanel();
			Hide();
		}

		private void OnGameUnpaused()
		{
			Hide();
		}

		private void UpdateVisuals()
		{
			txtMoveUp.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp));
			txtMoveDown.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown));
			txtMoveLeft.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft));
			txtMoveRight.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight));
			txtInteract.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Interact));
			txtInteractAlt.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt));
			txtPause.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Pause));
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

		private void ShowPressToRebindPanel()
		{
			pressToRebindPanel.SetActive(true);
		}

		private void HidePressToRebindPanel()
		{
			pressToRebindPanel.SetActive(false);
		}

		private void RebindBinding(GameInput.Binding binding)
		{
			ShowPressToRebindPanel();
			GameInput.Instance.Rebind(binding, () =>
			{
				HidePressToRebindPanel();
				UpdateVisuals();
			});
		}

		private void InitControlButtons()
		{
			txtMoveUp = btnMoveUp.GetComponentInChildren<TMP_Text>();
			txtMoveDown = btnMoveDown.GetComponentInChildren<TMP_Text>();
			txtMoveLeft = btnMoveLeft.GetComponentInChildren<TMP_Text>();
			txtMoveRight = btnMoveRight.GetComponentInChildren<TMP_Text>();
			txtInteract = btnInteract.GetComponentInChildren<TMP_Text>();
			txtInteractAlt = btnInteractAlt.GetComponentInChildren<TMP_Text>();
			txtPause = btnPause.GetComponentInChildren<TMP_Text>();
		}
	}
}