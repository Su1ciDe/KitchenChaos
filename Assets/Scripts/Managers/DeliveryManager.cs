using System.Collections.Generic;
using KitchenObjects;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Managers
{
	public class DeliveryManager : Singleton<DeliveryManager>
	{
		[SerializeField] private RecipeListSO recipeListSO;

		public List<RecipeSO> WaitingRecipeSOs { get; } = new List<RecipeSO>();

		public int SuccessfulRecipesAmount => successfulRecipesAmount;
		private int successfulRecipesAmount;

		private float spawnRecipeTimer;
		private readonly float spawnRecipeTimerMax = 4;
		private readonly int waitingRecipeMax = 4;

		public event UnityAction OnRecipeSpawned;
		public event UnityAction OnRecipeCompleted;
		public event UnityAction<Vector3> OnRecipeSuccess;
		public event UnityAction<Vector3> OnRecipeFail;

		private void Update()
		{
			spawnRecipeTimer -= Time.deltaTime;
			if (spawnRecipeTimer <= 0)
			{
				spawnRecipeTimer = spawnRecipeTimerMax;
				if (WaitingRecipeSOs.Count < waitingRecipeMax)
				{
					var waitingRecipeSo = recipeListSO.RecipeSOs[Random.Range(0, recipeListSO.RecipeSOs.Count)];
					WaitingRecipeSOs.Add(waitingRecipeSo);

					OnRecipeSpawned?.Invoke();
				}
			}
		}

		public void DeliverRecipe(Plate plate)
		{
			for (int i = 0; i < WaitingRecipeSOs.Count; i++)
			{
				var waitingRecipeSO = WaitingRecipeSOs[i];
				if (waitingRecipeSO.KitchenObjectSOs.Count.Equals(plate.KitchenObjectSOs.Count))
				{
					bool plateContentMatchesRecipe = true;
					foreach (var recipeKitchenObjectSO in waitingRecipeSO.KitchenObjectSOs)
					{
						bool ingredientFound = false;
						foreach (var plateKitchenObjectSO in plate.KitchenObjectSOs)
						{
							if (plateKitchenObjectSO.Equals(recipeKitchenObjectSO))
							{
								ingredientFound = true;
								break;
							}
						}

						if (!ingredientFound)
						{
							plateContentMatchesRecipe = false;
						}
					}

					if (plateContentMatchesRecipe)
					{
						successfulRecipesAmount++;

						WaitingRecipeSOs.RemoveAt(i);
						OnRecipeCompleted?.Invoke();
						OnRecipeSuccess?.Invoke(plate.transform.position);
						return;
					}
				}
			}

			OnRecipeFail?.Invoke(plate.transform.position);
		}
	}
}