using Managers;
using UnityEngine;

namespace UI
{
	public class DeliveryManagerUI : MonoBehaviour
	{
		[SerializeField] private Transform container;
		[SerializeField] private RecipeUI recipeTemplatePrefab;

		private void Start()
		{
			DeliveryManager.Instance.OnRecipeSpawned += OnRecipeSpawned;
			DeliveryManager.Instance.OnRecipeCompleted += OnRecipeCompleted;
			
			UpdateVisual();
		}

		private void OnRecipeCompleted()
		{
			UpdateVisual();
		}

		private void OnRecipeSpawned()
		{
			UpdateVisual();
		}

		private void UpdateVisual()
		{
			ClearVisual();
			foreach (var recipeSO in DeliveryManager.Instance.WaitingRecipeSOs)
			{
				var recipeTemp = Instantiate(recipeTemplatePrefab, container);
				recipeTemp.SetRecipe(recipeSO);
			}
		}

		private void ClearVisual()
		{
			foreach (Transform child in container)
				Destroy(child.gameObject);
		}
	}
}