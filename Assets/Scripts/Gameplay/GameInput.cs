using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
	public class GameInput : MonoBehaviour
	{
		private PlayerInputActions playerInputActions;

		public static event EventHandler OnInteractAction;
		public static event EventHandler OnInteractAltAction;
		public static event EventHandler OnPauseAction;

		private void Awake()
		{
			playerInputActions = new PlayerInputActions();
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
	}
}