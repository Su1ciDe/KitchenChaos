using Gameplay;
using Interfaces;
using KitchenObjects;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Counter
{
	public class CuttingCounter : BaseCounter, IHasProgress
	{
		[SerializeField] private CuttingRecipeSO[] cuttingRecipeSOs;

		private int cuttingProgress;

		public event UnityAction<float, bool> OnProgressChanged;
		public event UnityAction OnCut;
		public static event UnityAction<Vector3> OnAnyCut;

		public override void Interact(Player player)
		{
			if (!HasKitchenObject)
			{
				if (player.HasKitchenObject)
				{
					if (HasRecipe(player.KitchenObject.GetKitchenObjectSO()))
					{
						player.KitchenObject.KitchenObjectParent = this;

						cuttingProgress = 0;
						OnProgressChanged?.Invoke(cuttingProgress, false);
					}
					else
					{
						// TODO: you can't place
					}
				}
			}
			else
			{
				if (!player.HasKitchenObject)
				{
					KitchenObject.KitchenObjectParent = player;
				}
				else
				{
					if (player.KitchenObject is Plate plate)
					{
						if (plate.TryAddIngredient(KitchenObject.GetKitchenObjectSO()))
							KitchenObject.DestroySelf();
					}
				}
			}
		}

		public override void InteractAlternate(Player player)
		{
			if (HasKitchenObject && HasRecipe(KitchenObject.GetKitchenObjectSO()))
			{
				var cuttingRecipeSO = GetCuttingRecipeSOWithInput(KitchenObject.GetKitchenObjectSO());

				cuttingProgress++;
				OnCut?.Invoke();
				OnAnyCut?.Invoke(transform.position);
				OnProgressChanged?.Invoke((float)cuttingProgress / cuttingRecipeSO.CuttingProgressMax, true);

				if (cuttingProgress >= cuttingRecipeSO.CuttingProgressMax)
				{
					var outputKitchenObjectSO = FindOutputFromInput(KitchenObject.GetKitchenObjectSO());
					KitchenObject.DestroySelf();
					KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
				}
			}
		}

		private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
		{
			foreach (var recipeSO in cuttingRecipeSOs)
			{
				if (recipeSO.Input.Equals(inputKitchenObjectSO))
					return recipeSO;
			}

			return null;
		}

		private bool HasRecipe(KitchenObjectSO inputKitchenObjectSO)
		{
			return GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
		}

		private KitchenObjectSO FindOutputFromInput(KitchenObjectSO inputKitchenObjectSO)
		{
			var cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
			return cuttingRecipeSO ? cuttingRecipeSO.Output : null;
		}
	}
}