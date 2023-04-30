using Gameplay;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Counter
{
	public class TrashCounter : BaseCounter
	{
		public static event UnityAction<Vector3> OnAnyObjectTrashed;

		public override void Interact(Player player)
		{
			if (!player.HasKitchenObject) return;

			KitchenObjects.KitchenObject.DestroyKitchenObject(player.KitchenObject);
			InteractLogicServerRpc();
		}

		[ServerRpc(RequireOwnership = false)]
		private void InteractLogicServerRpc()
		{
			InteractLogicClientRpc();
		}

		[ClientRpc]
		private void InteractLogicClientRpc()
		{
			OnAnyObjectTrashed?.Invoke(transform.position);
		}
	}
}