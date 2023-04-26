using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Recipe", menuName = "KitchenChaos/Recipe", order = 0)]
	public class RecipeSO : ScriptableObject
	{
		public List<KitchenObjectSO> KitchenObjectSOs;
		public string RecipeName;
	}
}