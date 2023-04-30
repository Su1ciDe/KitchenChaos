using Gameplay;
using KitchenObjects;
using Managers;

namespace Counter
{
	public class DeliveryCounter : BaseCounter
	{
		public override void Interact(Player player)
		{
			if (!player.HasKitchenObject) return;
			if (player.KitchenObject is not Plate plate) return;

			DeliveryManager.Instance.DeliverRecipe(plate);
			KitchenObject.DestroyKitchenObject(plate);
		}
	}
}