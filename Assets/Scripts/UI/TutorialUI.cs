using Gameplay;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
	public class TutorialUI : BaseUI
	{
		[SerializeField] private TMP_Text moveUpKeyText;
		[SerializeField] private TMP_Text moveLeftKeyText;
		[SerializeField] private TMP_Text moveRightKeyText;
		[SerializeField] private TMP_Text moveDownKeyText;
		[SerializeField] private TMP_Text interactKeyText;
		[SerializeField] private TMP_Text interactAltKeyText;
		[SerializeField] private TMP_Text pauseKeyText;
		[SerializeField] private TMP_Text gamepadInteractKeyText;
		[SerializeField] private TMP_Text gamepadInteractAltKeyText;
		[SerializeField] private TMP_Text gamepadPauseKeyText;

		private void Start()
		{
			UpdateVisual();
			Show();
		}

		private void OnEnable()
		{
			GameInput.OnBindingRebound += OnBindingRebound;
			GameManager.Instance.OnStateChanged += OnGameStateChanged;
		}

		private void OnDisable()
		{
			GameInput.OnBindingRebound -= OnBindingRebound;
		}

		private void OnBindingRebound()
		{
			UpdateVisual();
		}

		private void UpdateVisual()
		{
			moveUpKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp));
			moveLeftKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft));
			moveRightKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight));
			moveDownKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown));
			interactKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Interact));
			interactAltKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown));
			pauseKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown));
			gamepadInteractKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact));
			gamepadInteractAltKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlt));
			gamepadPauseKeyText.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause));
		}

		private void OnGameStateChanged(GameManager.State state)
		{
			if (state != GameManager.State.CountdownToStart) return;

			Hide();
		}
	}
}