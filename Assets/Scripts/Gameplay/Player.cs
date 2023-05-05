using System;
using Counter;
using Interfaces;
using KitchenObjects;
using Managers;
using Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
	public class Player : NetworkBehaviour, IKitchenObjectParent
	{
		public static Player LocalInstance { get; private set; }
		public bool HasKitchenObject => KitchenObject;

		private BaseCounter selectedCounter;

		private KitchenObject kitchenObject;
		public KitchenObject KitchenObject
		{
			get => kitchenObject;
			set
			{
				kitchenObject = value;
				if (kitchenObject)
				{
					OnPickedSomething?.Invoke(kitchenObject.transform.position);
					OnAnyPlayerPickedSomething?.Invoke(kitchenObject.transform.position);
				}
			}
		}
		public Transform KitchenObjectPoint => kitchenObjectPoint;

		[SerializeField] private Transform kitchenObjectPoint;
		[SerializeField] private LayerMask counterLayerMask;

		private PlayerVisual playerVisual;

		public event UnityAction<BaseCounter> OnSelectedCounterChanged;
		public event UnityAction<Vector3> OnPickedSomething; // picked object position
		public static event UnityAction OnAnyPlayerSpawned;
		public static event UnityAction<Vector3> OnAnyPlayerPickedSomething;

		private void Awake()
		{
			playerVisual = GetComponentInChildren<PlayerVisual>();
		}

		private void Start()
		{
			GameInput.OnInteractAction += OnInteractAction;
			GameInput.OnInteractAltAction += OnInteractAltAction;

			var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
			playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.ColorId));
		}

		public override void OnNetworkSpawn()
		{
			if (IsOwner)
				LocalInstance = this;

			base.OnNetworkSpawn();

			transform.position = KitchenGameMultiplayer.Instance.PlayerSpawnPositions[KitchenGameMultiplayer.Instance.GetPlayerIndexFromClientId(OwnerClientId)];
			OnAnyPlayerSpawned?.Invoke();

			if (IsServer)
				NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
		}

		private void Update()
		{
			if (!IsOwner) return;
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
			OnSelectedCounterChanged?.Invoke(selectedCounter);
		}

		public NetworkObject GetNetworkObject() => NetworkObject;

		private void OnClientDisconnected(ulong clientId)
		{
			if (clientId.Equals(OwnerClientId) && HasKitchenObject)
			{
				KitchenObject.DestroyKitchenObject(KitchenObject);
			}
		}
	}
}