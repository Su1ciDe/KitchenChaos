using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "BurningRecipe", menuName = "KitchenChaos/BurningRecipe", order = 2)]
	public class BurningRecipeSO : ScriptableObject
	{
		public KitchenObjectSO Input;
		public KitchenObjectSO Output;

		public int BurningTimerMax;
	}
}