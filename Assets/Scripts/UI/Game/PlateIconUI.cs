using KitchenObjects;
using ScriptableObjects;
using UnityEngine;

namespace UI
{
	public class PlateIconUI : MonoBehaviour
	{
		[SerializeField] private IconUI iconTemplatePrefab;
		private Plate plate;

		private void Awake()
		{
			plate = GetComponentInParent<Plate>();
		}

		private void Start()
		{
			plate.OnIngredientAdded += OnIngredientAdded;
		}

		private void OnIngredientAdded(KitchenObjectSO kitchenObjectSO)
		{
			UpdateIcons();
		}

		private void UpdateIcons()
		{
			ClearIcons();
			foreach (var kitchenObjectSO in plate.KitchenObjectSOs)
			{
				var icon = Instantiate(iconTemplatePrefab, transform);
				icon.SetKitchenObjectIcon(kitchenObjectSO);
			}
		}

		private void ClearIcons()
		{
			foreach (Transform child in transform)
				Destroy(child.gameObject);
		}
	}
}