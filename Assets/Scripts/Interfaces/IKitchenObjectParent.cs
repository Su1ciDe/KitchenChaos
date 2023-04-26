using KitchenObjects;
using UnityEngine;

namespace Interfaces
{
	public interface IKitchenObjectParent
	{
		public bool HasKitchenObject { get; }
		public KitchenObject KitchenObject { get; set; }
		public Transform KitchenObjectPoint { get; }
	}
}