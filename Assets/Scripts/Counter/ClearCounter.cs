using Gameplay;
using KitchenObjects;
using Network;

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
							KitchenObject.DestroyKitchenObject(KitchenObject);
					}
					else
					{
						if (KitchenObject is Plate plateOnCounter)
						{
							if (plateOnCounter.TryAddIngredient(player.KitchenObject.GetKitchenObjectSO()))
								KitchenObject.DestroyKitchenObject(player.KitchenObject);
						}
					}
				}
			}
		}
	}
}