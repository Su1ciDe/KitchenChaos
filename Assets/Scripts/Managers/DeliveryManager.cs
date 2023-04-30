using System.Collections.Generic;
using KitchenObjects;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Managers
{
	public class DeliveryManager : NetworkBehaviour
	{
		public static DeliveryManager Instance { get; private set; }

		[SerializeField] private RecipeListSO recipeListSO;

		public List<RecipeSO> WaitingRecipeSOs { get; } = new List<RecipeSO>();

		public int SuccessfulRecipesAmount => successfulRecipesAmount;
		private int successfulRecipesAmount;

		private float spawnRecipeTimer;
		private readonly float spawnRecipeTimerMax = 4;
		private readonly int waitingRecipeMax = 4;

		public event UnityAction OnRecipeSpawned;
		public event UnityAction OnRecipeCompleted;
		public event UnityAction<Vector3> OnRecipeSuccess; // plate position
		public event UnityAction<Vector3> OnRecipeFail; // plate position

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			if (!IsServer) return;

			spawnRecipeTimer -= Time.deltaTime;
			if (!(spawnRecipeTimer <= 0)) return;

			spawnRecipeTimer = spawnRecipeTimerMax;
			if (GameManager.Instance.IsPlaying && WaitingRecipeSOs.Count < waitingRecipeMax)
			{
				int waitingRecipeSOIndex = Random.Range(0, recipeListSO.RecipeSOs.Count);
				SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
			}
		}

		[ClientRpc]
		private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
		{
			var waitingRecipeSO = recipeListSO.RecipeSOs[waitingRecipeSOIndex];
			WaitingRecipeSOs.Add(waitingRecipeSO);

			OnRecipeSpawned?.Invoke();
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
						DeliverCorrectRecipeServerRpc(i, plate.transform.position);
						return;
					}
				}
			}

			DeliverIncorrectRecipeServerRpc(plate.transform.position);
		}

		[ServerRpc(RequireOwnership = false)]
		private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOIndex, Vector3 platePosition)
		{
			DeliverCorrectRecipeClientRpc(waitingRecipeSOIndex, platePosition);
		}

		[ClientRpc]
		private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOIndex, Vector3 platePosition)
		{
			successfulRecipesAmount++;

			WaitingRecipeSOs.RemoveAt(waitingRecipeSOIndex);
			OnRecipeCompleted?.Invoke();
			OnRecipeSuccess?.Invoke(platePosition);
		}

		[ServerRpc(RequireOwnership = false)]
		private void DeliverIncorrectRecipeServerRpc(Vector3 platePosition)
		{
			DeliverIncorrectRecipeClientRpc(platePosition);
		}

		[ClientRpc]
		private void DeliverIncorrectRecipeClientRpc(Vector3 platePosition)
		{
			OnRecipeFail?.Invoke(platePosition);
		}
	}
}