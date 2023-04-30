using Gameplay;
using KitchenObjects;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Counter
{
	public class ContainerCounter : BaseCounter
	{
		[Header("Container")]
		[SerializeField] private KitchenObjectSO kitchenObjectSO;

		public event UnityAction OnPlayerGrabbedObject;

		public override void Interact(Player player)
		{
			if (player.HasKitchenObject) return;

			KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

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
			OnPlayerGrabbedObject?.Invoke();
		}
	}
}