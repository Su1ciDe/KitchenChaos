using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "RecipeList", menuName = "KitchenChaos/Recipe List", order = 0)]
	public class RecipeListSO : ScriptableObject
	{
		public List<RecipeSO> RecipeSOs;
	}
}