using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace UI
{
	public class RecipeUI : MonoBehaviour
	{
		[SerializeField] private TMP_Text recipeNameText;
		[Header("Icons")]
		[SerializeField] private Transform iconContainer;
		[SerializeField] private IconUI iconTemplate;

		public void SetRecipe(RecipeSO recipeSO)
		{
			recipeNameText.SetText(recipeSO.RecipeName);
			UpdateIcons(recipeSO);
		}

		private void UpdateIcons(RecipeSO recipeSO)
		{
			ClearIcons();

			foreach (var kitchenObjectSO in recipeSO.KitchenObjectSOs)
			{
				var icon = Instantiate(iconTemplate, iconContainer);
				icon.SetKitchenObjectIcon(kitchenObjectSO);
			}
		}

		private void ClearIcons()
		{
			foreach (Transform icon in iconContainer)
				Destroy(icon.gameObject);
		}
	}
}