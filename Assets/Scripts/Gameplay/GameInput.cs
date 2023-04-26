using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utility;

namespace Gameplay
{
	public class GameInput : Singleton<GameInput>
	{
		public enum Binding
		{
			MoveUp,
			MoveDown,
			MoveLeft,
			MoveRight,
			Interact,
			InteractAlt,
			Pause
		}

		private PlayerInputActions playerInputActions;

		private const string BINDINGS_PLAYERPREFS = "InputBindings";

		public static event EventHandler OnInteractAction;
		public static event EventHandler OnInteractAltAction;
		public static event EventHandler OnPauseAction;

		private void Awake()
		{
			playerInputActions = new PlayerInputActions();
			if (PlayerPrefs.HasKey(BINDINGS_PLAYERPREFS))
				playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(BINDINGS_PLAYERPREFS));
			playerInputActions.Player.Enable();

			playerInputActions.Player.Interact.performed += OnInteract;
			playerInputActions.Player.InteractAlt.performed += OnInteractAlt;
			playerInputActions.Player.Pause.performed += OnPause;
		}

		private void OnDestroy()
		{
			playerInputActions.Player.Interact.performed -= OnInteract;
			playerInputActions.Player.InteractAlt.performed -= OnInteractAlt;
			playerInputActions.Player.Pause.performed -= OnPause;

			playerInputActions.Dispose();
		}

		private void OnPause(InputAction.CallbackContext obj)
		{
			OnPauseAction?.Invoke(this, EventArgs.Empty);
		}

		public Vector2 GetMovementVectorNormalized()
		{
			var inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
			inputVector.Normalize();
			return inputVector;
		}

		private void OnInteract(InputAction.CallbackContext context)
		{
			OnInteractAction?.Invoke(this, EventArgs.Empty);
		}

		private void OnInteractAlt(InputAction.CallbackContext context)
		{
			OnInteractAltAction?.Invoke(this, EventArgs.Empty);
		}

		public string GetBindingText(Binding binding)
		{
			return binding switch
			{
				Binding.MoveUp => playerInputActions.Player.Move.bindings[1].ToDisplayString(),
				Binding.MoveDown => playerInputActions.Player.Move.bindings[2].ToDisplayString(),
				Binding.MoveLeft => playerInputActions.Player.Move.bindings[3].ToDisplayString(),
				Binding.MoveRight => playerInputActions.Player.Move.bindings[4].ToDisplayString(),
				Binding.Interact => playerInputActions.Player.Interact.bindings[0].ToDisplayString(),
				Binding.InteractAlt => playerInputActions.Player.InteractAlt.bindings[0].ToDisplayString(),
				Binding.Pause => playerInputActions.Player.Pause.bindings[0].ToDisplayString(),
				_ => throw new ArgumentOutOfRangeException(nameof(binding), binding, null)
			};
		}

		public void Rebind(Binding binding, UnityAction onRebound)
		{
			playerInputActions.Player.Disable();
			InputAction inputAction;
			int bindingIndex;

			switch (binding)
			{
				case Binding.MoveUp:
					inputAction = playerInputActions.Player.Move;
					bindingIndex = 1;
					break;
				case Binding.MoveDown:
					inputAction = playerInputActions.Player.Move;
					bindingIndex = 2;
					break;
				case Binding.MoveLeft:
					inputAction = playerInputActions.Player.Move;
					bindingIndex = 3;
					break;
				case Binding.MoveRight:
					inputAction = playerInputActions.Player.Move;
					bindingIndex = 4;
					break;
				case Binding.Interact:
					inputAction = playerInputActions.Player.Interact;
					bindingIndex = 0;
					break;
				case Binding.InteractAlt:
					inputAction = playerInputActions.Player.InteractAlt;
					bindingIndex = 0;
					break;
				case Binding.Pause:
					inputAction = playerInputActions.Player.Pause;
					bindingIndex = 0;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(binding), binding, null);
			}

			inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
			{
				callback.Dispose();
				playerInputActions.Player.Enable();

				onRebound?.Invoke();

				PlayerPrefs.SetString(BINDINGS_PLAYERPREFS, playerInputActions.SaveBindingOverridesAsJson());
				PlayerPrefs.Save();
			}).Start();
		}
	}
}