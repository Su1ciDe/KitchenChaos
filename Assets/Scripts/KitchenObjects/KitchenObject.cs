using Interfaces;
using Network;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using Utility;

namespace KitchenObjects
{
	public class KitchenObject : NetworkBehaviour
	{
		private IKitchenObjectParent kitchenObjectParent;
		public IKitchenObjectParent KitchenObjectParent
		{
			get => kitchenObjectParent;
			set => SetKitchenObjectParentServerRpc(value.GetNetworkObject());
		}
		[SerializeField] private KitchenObjectSO kitchenObjectSO;

		public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;

		private FollowTransform followTransform;

		protected virtual void Awake()
		{
			followTransform = GetComponent<FollowTransform>();
		}

		public void DestroySelf()
		{
			kitchenObjectParent = null;
			Destroy(gameObject);
		}

		public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
		{
			KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
		}

		[ServerRpc(RequireOwnership = false)]
		private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
		{
			SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
		}

		[ClientRpc]
		private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
		{
			if (!kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject)) return;
			if (!kitchenObjectParentNetworkObject.TryGetComponent(out IKitchenObjectParent _kitchenObjectParent)) return;

			if (kitchenObjectParent != null)
				kitchenObjectParent.KitchenObject = null;

			kitchenObjectParent = _kitchenObjectParent;
			kitchenObjectParent.KitchenObject = this;

			followTransform.SetTargetTransform(_kitchenObjectParent.KitchenObjectPoint);
		}
	}
}