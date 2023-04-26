using System.Collections.Generic;
using KitchenObjects;
using ScriptableObjects;
using UnityEngine;

namespace Utility
{
	public class PlateCompleteVisual : MonoBehaviour
	{
		[System.Serializable]
		public struct KitchenObjectSO_GameObject
		{
			public KitchenObjectSO KitchenObjectSO;
			public GameObject GameObject;
		}

		[SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjects;

		private Plate plate;

		private void Awake()
		{
			plate = GetComponentInParent<Plate>();
		}

		private void Start()
		{
			plate.OnIngredientAdded += OnIngredientAdded;
			foreach (var kitchenObjectSOGameObject in kitchenObjectSOGameObjects)
			{
				kitchenObjectSOGameObject.GameObject.SetActive(false);
			}
		}

		private void OnIngredientAdded(KitchenObjectSO kitchenObjectSO)
		{
			foreach (var kitchenObjectSOGameObject in kitchenObjectSOGameObjects)
			{
				if (kitchenObjectSOGameObject.KitchenObjectSO.Equals(kitchenObjectSO))
				{
					kitchenObjectSOGameObject.GameObject.SetActive(true);
				}
			}
		}
	}
}