using System;
using Counter;
using Interfaces;
using KitchenObjects;
using Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Gameplay
{
	public class Player : NetworkBehaviour, IKitchenObjectParent
	{
		public bool HasKitchenObject => KitchenObject;

		private BaseCounter selectedCounter;

		private KitchenObject kitchenObject;
		public KitchenObject KitchenObject
		{
			get => kitchenObject;
			set
			{
				kitchenObject = value;
				if (kitchenObject) OnPickedSomething?.Invoke(kitchenObject.transform.position);
			}
		}
		public Transform KitchenObjectPoint => kitchenObjectPoint;

		[SerializeField] private Transform kitchenObjectPoint;
		[SerializeField] private LayerMask counterLayerMask;

		public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
		public event UnityAction<Vector3> OnPickedSomething;

		public class OnSelectedCounterChangedEventArgs : EventArgs
		{
			public BaseCounter selectedCounter;
		}

		private void Start()
		{
			GameInput.OnInteractAction += OnInteractAction;
			GameInput.OnInteractAltAction += OnInteractAltAction;
		}

		private void Update()
		{
			HandleInteractions();
		}

		private void HandleInteractions()
		{
			float interactDistance = 2;
			if (Physics.Raycast(transform.position, transform.forward, out var hit, interactDistance, counterLayerMask))
			{
				if (hit.transform.TryGetComponent(out BaseCounter baseCounter))
				{
					if (selectedCounter != baseCounter)
					{
						SetSelectedCounter(baseCounter);
					}
				}
				else
				{
					SetSelectedCounter(null);
				}
			}
			else
			{
				SetSelectedCounter(null);
			}
		}

		private void OnInteractAction(object sender, EventArgs e)
		{
			if (!GameManager.Instance.IsPlaying) return;
			if (selectedCounter)
				selectedCounter.Interact(this);
		}

		private void OnInteractAltAction(object sender, EventArgs e)
		{
			if (!GameManager.Instance.IsPlaying) return;
			if (selectedCounter)
				selectedCounter.InteractAlternate(this);
		}

		private void SetSelectedCounter(BaseCounter selectedCounter)
		{
			this.selectedCounter = selectedCounter;
			OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter });
		}
	}
}