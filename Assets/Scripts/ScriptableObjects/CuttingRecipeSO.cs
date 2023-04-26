using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "CuttingRecipe", menuName = "KitchenChaos/CuttingRecipe", order = 0)]
	public class CuttingRecipeSO : ScriptableObject
	{
		public KitchenObjectSO Input;
		public KitchenObjectSO Output;

		public int CuttingProgressMax;
	}
}