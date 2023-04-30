using Gameplay;
using Interfaces;
using KitchenObjects;
using Managers;
using ScriptableObjects;
using Unity.Netcode;
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
			Countdown();
		}

		private void Countdown()
		{
			if (!IsServer) return;
			if (!GameManager.Instance.IsPlaying || platesSpawnedAmount >= platesSpawnedAmountMax) return;

			spawnPlateTimer += Time.deltaTime;
			OnProgressChanged?.Invoke(spawnPlateTimer / spawnPlateTimeMax, false);
			if (spawnPlateTimer > spawnPlateTimeMax)
			{
				spawnPlateTimer = 0;

				SpawnPlateServerRpc();
			}
		}

		[ServerRpc]
		private void SpawnPlateServerRpc()
		{
			SpawnPlateClientRpc();
		}

		[ClientRpc]
		private void SpawnPlateClientRpc()
		{
			platesSpawnedAmount++;
			OnPlateSpawned?.Invoke();
		}

		public override void Interact(Player player)
		{
			if (!player.HasKitchenObject)
			{
				if (platesSpawnedAmount <= 0) return;

				KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

				InteractLogicServerRpc();
			}
			else if (player.KitchenObject is not Plate)
			{
				if (platesSpawnedAmount <= 0) return;
				var kitchenObjectAtHand = player.KitchenObject;
				KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
				if (((Plate)player.KitchenObject).TryAddIngredient(kitchenObjectAtHand.GetKitchenObjectSO()))
				{
					KitchenObject.DestroyKitchenObject(kitchenObjectAtHand);
					InteractLogicServerRpc();
				}
				else
				{
					KitchenObject.DestroyKitchenObject(player.KitchenObject);
					player.KitchenObject = kitchenObjectAtHand;
				}
			}
		}

		[ServerRpc(RequireOwnership = false)]
		private void InteractLogicServerRpc()
		{
			InteractLogicClientRpc();
		}

		[ClientRpc]
		private void InteractLogicClientRpc()
		{
			platesSpawnedAmount--;
			OnPlateRemoved?.Invoke();
		}
	}
}