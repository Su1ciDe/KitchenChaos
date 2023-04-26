using Gameplay;
using Interfaces;
using KitchenObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Counter
{
	public class BaseCounter : MonoBehaviour, IKitchenObjectParent
	{
		public bool HasKitchenObject => KitchenObject;

		private KitchenObject kitchenObject;
		public KitchenObject KitchenObject
		{
			get => kitchenObject;
			set
			{
				kitchenObject = value;
				if (kitchenObject) OnAnyObjectPlaced?.Invoke(transform.position);
			}
		}
		public Transform KitchenObjectPoint => topPoint;

		[SerializeField] private Transform topPoint;

		public static event UnityAction<Vector3> OnAnyObjectPlaced;

		public virtual void Interact(Player player)
		{
		}

		public virtual void InteractAlternate(Player player)
		{
		}
	}
}