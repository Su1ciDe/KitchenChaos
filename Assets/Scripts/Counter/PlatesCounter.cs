using Gameplay;
using Interfaces;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Counter
{
	public class PlatesCounter : BaseCounter, IHasProgress
	{
		[Header("Plate Counter")]
		[SerializeField] private KitchenObjectSO plateKitchenObjectSO;
		[SerializeField] private float spawnPlateTimeMax = 4f;
		[SerializeField] private int platesSpawnedAmountMax = 4;

		private float spawnPlateTimer;
		private int platesSpawnedAmount;

		public event UnityAction OnPlateSpawned;
		public event UnityAction OnPlateRemoved;
		public event UnityAction<float, bool> OnProgressChanged;

		private void Update()
		{
			if (platesSpawnedAmount < platesSpawnedAmountMax)
			{
				spawnPlateTimer += Time.deltaTime;
				OnProgressChanged?.Invoke(spawnPlateTimer / spawnPlateTimeMax, false);
				if (spawnPlateTimer > spawnPlateTimeMax)
				{
					spawnPlateTimer = 0;
					platesSpawnedAmount++;
					OnPlateSpawned?.Invoke();
				}
			}
		}

		public override void Interact(Player player)
		{
			if (!player.HasKitchenObject)
			{
				if (platesSpawnedAmount > 0)
				{
					platesSpawnedAmount--;

					KitchenObjects.KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
					OnPlateRemoved?.Invoke();
				}
			}
		}
	}
}