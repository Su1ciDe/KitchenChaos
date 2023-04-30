using System;
using Gameplay;
using Interfaces;
using KitchenObjects;
using Network;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Counter
{
	public class StoveCounter : BaseCounter, IHasProgress
	{
		[Header("Stove")]
		[SerializeField] private FryingRecipeSO[] fryingRecipeSOs;
		[SerializeField] private BurningRecipeSO[] burningRecipeSOs;

		private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0);
		private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0);
		private FryingRecipeSO currentRecipeSO;
		private BurningRecipeSO burningRecipeSO;

		public enum State
		{
			Idle,
			Frying,
			Fried,
			Burned
		}

		private NetworkVariable<State> currentState = new NetworkVariable<State>(State.Idle);
		private State CurrentState
		{
			get => currentState.Value;
			set => currentState.Value = value;
		}

		public bool IsFried => CurrentState == State.Fried;

		public event UnityAction<State> OnStateChanged;
		public event UnityAction<float, bool> OnProgressChanged;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			fryingTimer.OnValueChanged += OnFryingTimerValueChanged;
			burningTimer.OnValueChanged += OnBurningTimerValueChanged;
			currentState.OnValueChanged += OnStateValueChanged;
		}

		private void Update()
		{
			if (!IsServer) return;
			if (!HasKitchenObject) return;

			switch (CurrentState)
			{
				case State.Idle:
					break;
				case State.Frying:
					fryingTimer.Value += Time.deltaTime;

					if (fryingTimer.Value > currentRecipeSO.FryingTimerMax)
					{
						KitchenObject.DestroyKitchenObject(KitchenObject);
						KitchenObject.SpawnKitchenObject(currentRecipeSO.Output, this);

						CurrentState = State.Fried;
						burningTimer.Value = 0;

						SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(KitchenObject.GetKitchenObjectSO()));
					}

					break;
				case State.Fried:
					burningTimer.Value += Time.deltaTime;

					if (burningTimer.Value > burningRecipeSO.BurningTimerMax)
					{
						KitchenObject.DestroyKitchenObject(KitchenObject);
						KitchenObject.SpawnKitchenObject(burningRecipeSO.Output, this);

						CurrentState = State.Burned;
					}

					break;
				case State.Burned:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnFryingTimerValueChanged(float previousValue, float newValue)
		{
			float fryingTimerMax = currentRecipeSO ? currentRecipeSO.FryingTimerMax : 1;
			OnProgressChanged?.Invoke(fryingTimer.Value / fryingTimerMax, false);
		}

		private void OnBurningTimerValueChanged(float previousValue, float newValue)
		{
			float burningTimerMax = burningRecipeSO ? burningRecipeSO.BurningTimerMax : 1;
			OnProgressChanged?.Invoke(burningTimer.Value / burningTimerMax, false);
		}

		private void OnStateValueChanged(State previousValue, State newValue)
		{
			OnStateChanged?.Invoke(CurrentState);
			if (CurrentState is State.Burned or State.Idle)
				OnProgressChanged?.Invoke(0, false);
		}

		public override void Interact(Player player)
		{
			if (!HasKitchenObject)
			{
				if (player.HasKitchenObject)
				{
					if (!HasRecipe(player.KitchenObject.GetKitchenObjectSO())) return;
					var kitchenObject = player.KitchenObject;
					kitchenObject.KitchenObjectParent = this;

					InteractLogicPlaceObjectOnStoveServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
				}
			}
			else
			{
				if (!player.HasKitchenObject)
				{
					KitchenObject.KitchenObjectParent = player;

					SetStateIdleServerRpc();
					OnProgressChanged?.Invoke(0, false);
				}
				else
				{
					if (player.KitchenObject is not Plate plate) return;
					if (plate.TryAddIngredient(KitchenObject.GetKitchenObjectSO()))
						KitchenObject.DestroyKitchenObject(KitchenObject);

					SetStateIdleServerRpc();
					OnProgressChanged?.Invoke(0, false);
				}
			}
		}

		[ServerRpc(RequireOwnership = false)]
		private void InteractLogicPlaceObjectOnStoveServerRpc(int kitchenObjectSOIndex)
		{
			fryingTimer.Value = 0;
			CurrentState = State.Frying;

			SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
		}

		[ClientRpc]
		private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
		{
			currentRecipeSO = GetFryingRecipeSOWithInput(KitchenGameMultiplayer.Instance.GetKitchenObjectSOAt(kitchenObjectSOIndex));
		}

		[ClientRpc]
		private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
		{
			burningRecipeSO = GetBurningRecipeSOWithInput(KitchenGameMultiplayer.Instance.GetKitchenObjectSOAt(kitchenObjectSOIndex));
		}

		[ServerRpc(RequireOwnership = false)]
		private void SetStateIdleServerRpc()
		{
			CurrentState = State.Idle;
		}

		private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
		{
			foreach (var recipeSO in fryingRecipeSOs)
			{
				if (recipeSO.Input.Equals(inputKitchenObjectSO))
					return recipeSO;
			}

			return null;
		}

		private bool HasRecipe(KitchenObjectSO inputKitchenObjectSO)
		{
			return GetFryingRecipeSOWithInput(inputKitchenObjectSO);
		}

		private KitchenObjectSO FindOutputFromInput(KitchenObjectSO inputKitchenObjectSO)
		{
			var fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
			return fryingRecipeSO ? fryingRecipeSO.Output : null;
		}

		private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
		{
			foreach (var recipeSO in burningRecipeSOs)
			{
				if (recipeSO.Input.Equals(inputKitchenObjectSO))
					return recipeSO;
			}

			return null;
		}
	}
}