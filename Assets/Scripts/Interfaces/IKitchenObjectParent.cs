using KitchenObjects;
using Unity.Netcode;
using UnityEngine;

namespace Interfaces
{
	public interface IKitchenObjectParent
	{
		public bool HasKitchenObject { get; }
		public KitchenObject KitchenObject { get; set; }
		public Transform KitchenObjectPoint { get; }
		public NetworkObject GetNetworkObject();
	}
}