using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class IconUI : MonoBehaviour
	{
		[SerializeField]private Image icon;

		public void SetKitchenObjectIcon(KitchenObjectSO kitchenObjectSO)
		{
			icon.sprite = kitchenObjectSO.Sprite;
		}
	}
}