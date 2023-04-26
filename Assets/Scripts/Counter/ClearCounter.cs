using Gameplay;
using KitchenObjects;

namespace Counter
{
	public class ClearCounter : BaseCounter
	{
		public override void Interact(Player player)
		{
			if (!HasKitchenObject)
			{
				if (player.HasKitchenObject)
				{
					player.KitchenObject.KitchenObjectParent = this;
				}
			}
			else
			{
				if (!player.HasKitchenObject)
				{
					KitchenObject.KitchenObjectParent = player;
				}
				else
				{
					if (player.KitchenObject is Plate plate)
					{
						if (plate.TryAddIngredient(KitchenObject.GetKitchenObjectSO()))
							KitchenObject.DestroySelf();
					}
					else
					{
						if (KitchenObject is Plate plateOnCounter)
						{
							if (plateOnCounter.TryAddIngredient(player.KitchenObject.GetKitchenObjectSO()))
								player.KitchenObject.DestroySelf();
						}
					}
				}
			}
		}
	}
}