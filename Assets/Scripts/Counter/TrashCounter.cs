using Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Counter
{
	public class TrashCounter : BaseCounter
	{
		public static event UnityAction<Vector3> OnAnyObjectTrashed;

		public override void Interact(Player player)
		{
			if (player.HasKitchenObject)
			{
				player.KitchenObject.DestroySelf();

				OnAnyObjectTrashed?.Invoke(transform.position);
			}
		}
	}
}