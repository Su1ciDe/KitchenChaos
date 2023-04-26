using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace KitchenObjects
{
	public class Plate : KitchenObject
	{
		[SerializeField] private List<KitchenObjectSO> validIngredients = new List<KitchenObjectSO>();

		public List<KitchenObjectSO> KitchenObjectSOs => kitchenObjectSOs;
		private List<KitchenObjectSO> kitchenObjectSOs = new List<KitchenObjectSO>();

		public event UnityAction<KitchenObjectSO> OnIngredientAdded;

		public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
		{
			if (!validIngredients.Contains(kitchenObjectSO)) return false;

			if (kitchenObjectSOs.Contains(kitchenObjectSO))
			{
				return false;
			}
			else
			{
				kitchenObjectSOs.Add(kitchenObjectSO);
				OnIngredientAdded?.Invoke(kitchenObjectSO);
				return true;
			}
		}
	}
}