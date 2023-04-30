using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "KitchenObjectList", menuName = "KitchenChaos/KitchenObjectList", order = 4)]
	public class KitchenObjectListSO : ScriptableObject
	{
		public List<KitchenObjectSO> KitchenObjectSoList;
	}
}