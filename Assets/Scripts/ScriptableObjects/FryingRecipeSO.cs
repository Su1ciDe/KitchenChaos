using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "FryingRecipe", menuName = "KitchenChaos/FryingRecipe", order = 1)]
	public class FryingRecipeSO : ScriptableObject
	{
		public KitchenObjectSO Input;
		public KitchenObjectSO Output;

		public int FryingTimerMax;
	}
}