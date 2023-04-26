using KitchenObjects;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "KitchenObject", menuName = "KitchenChaos/KitchenObject", order = 3)]
	public class KitchenObjectSO : ScriptableObject
	{
		public KitchenObject Prefab;
		public Sprite Sprite;
		public string ObjectName;
	}
}