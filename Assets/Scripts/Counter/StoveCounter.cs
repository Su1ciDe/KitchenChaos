using System;
using Gameplay;
using Interfaces;
using KitchenObjects;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Counter
{
	public class StoveCounter : BaseCounter, IHasProgress
	{
		[Header("Stove")]
		[SerializeField] private FryingRecipeSO[] fryingRecipeSOs;
		[SerializeField] private BurningRecipeSO[] burningRecipeSOs;

		private float fryingTimer;
		private float burningTimer;
		private FryingRecipeSO currentRecipeSO;
		private BurningRecipeSO burningRecipeSO;

		public enum State
		{
			Idle,
			Frying,
			Fried,
			Burned
		}

		private State currentState;
		private State CurrentState
		{
			get => currentState;
			set
			{
				currentState = value;
				OnStateChanged?.Invoke(currentState);
			}
		}

		public event UnityAction<State> OnStateChanged;
		public event UnityAction<float, bool> OnProgressChanged;

		private void Start()
		{
			CurrentState = State.Idle;
		}

		private void Update()
		{
			if (!HasKitchenObject) return;

			switch (CurrentState)
			{
				case State.Idle:
					break;
				case State.Frying:
					fryingTimer += Time.deltaTime;
					OnProgressChanged?.Invoke(fryingTimer / currentRecipeSO.FryingTimerMax, false);

					if (fryingTimer > currentRecipeSO.FryingTimerMax)
					{
						KitchenObject.DestroySelf();
						KitchenObjects.KitchenObject.SpawnKitchenObject(currentRecipeSO.Output, this);

						CurrentState = State.Fried;
						burningTimer = 0;

						burningRecipeSO = GetBurningRecipeSOWithInput(KitchenObject.GetKitchenObjectSO());
					}

					break;
				case State.Fried:
					burningTimer += Time.deltaTime;
					OnProgressChanged?.Invoke(burningTimer / burningRecipeSO.BurningTimerMax, false);

					if (burningTimer > burningRecipeSO.BurningTimerMax)
					{
						KitchenObject.DestroySelf();
						KitchenObjects.KitchenObject.SpawnKitchenObject(burningRecipeSO.Output, this);

						CurrentState = State.Burned;
					}

					break;
				case State.Burned:
					OnProgressChanged?.Invoke(0, false);

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void Interact(Player player)
		{
			if (!HasKitchenObject)
			{
				if (player.HasKitchenObject)
				{
					if (HasRecipe(player.KitchenObject.GetKitchenObjectSO()))
					{
						player.KitchenObject.KitchenObjectParent = this;
						currentRecipeSO = GetFryingRecipeSOWithInput(KitchenObject.GetKitchenObjectSO());

						CurrentState = State.Frying;
						fryingTimer = 0;

						OnProgressChanged?.Invoke(fryingTimer / currentRecipeSO.FryingTimerMax, false);
					}
				}
			}
			else
			{
				if (!player.HasKitchenObject)
				{
					KitchenObject.KitchenObjectParent = player;

					CurrentState = State.Idle;
					OnProgressChanged?.Invoke(0, false);
				}
				else
				{
					if (player.KitchenObject is Plate plate)
					{
						if (plate.TryAddIngredient(KitchenObject.GetKitchenObjectSO()))
							KitchenObject.DestroySelf();

						CurrentState = State.Idle;
						OnProgressChanged?.Invoke(0, false);
					}
				}
			}
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