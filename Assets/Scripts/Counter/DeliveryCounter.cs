using Gameplay;
using KitchenObjects;
using Managers;

namespace Counter
{
	public class DeliveryCounter : BaseCounter
	{
		public override void Interact(Player player)
		{
			if (player.HasKitchenObject)
			{
				if (player.KitchenObject is Plate plate)
				{
					DeliveryManager.Instance.DeliverRecipe(plate);
					plate.DestroySelf();
				}
			}
		}
	}
}