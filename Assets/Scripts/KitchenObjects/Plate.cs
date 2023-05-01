using System.Collections.Generic;
using Network;
using ScriptableObjects;
using Unity.Netcode;
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
				AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
				return true;
			}
		}

		[ServerRpc(RequireOwnership = false)]
		private void AddIngredientServerRpc(int kitchenObjectSOIndex)
		{
			AddIngredientClientRpc(kitchenObjectSOIndex);
		}

		[ClientRpc]
		private void AddIngredientClientRpc(int kitchenObjectSOIndex)
		{
			var kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOAt(kitchenObjectSOIndex);
			kitchenObjectSOs.Add(kitchenObjectSO);
			OnIngredientAdded?.Invoke(kitchenObjectSO);
		}
	}
}