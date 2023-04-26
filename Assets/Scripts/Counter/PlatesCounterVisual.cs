using System.Collections.Generic;
using UnityEngine;

namespace Counter
{
	public class PlatesCounterVisual : MonoBehaviour
	{
		[SerializeField] private Transform counterTopPoint;
		[SerializeField] private Transform plateVisualPrefab;

		private List<GameObject> spawnedPlates = new List<GameObject>();

		private float offset = .1f;

		private void Start()
		{
			var platesCounter = GetComponentInParent<PlatesCounter>();
			platesCounter.OnPlateSpawned += OnPlateSpawned;
			platesCounter.OnPlateRemoved += OnPlateRemoved;
		}

		private void OnPlateRemoved()
		{
			var removedPlate = spawnedPlates[^1];
			spawnedPlates.Remove(removedPlate);
			Destroy(removedPlate);
		}

		private void OnPlateSpawned()
		{
			var plate = Instantiate(plateVisualPrefab, counterTopPoint);
			plate.localPosition = new Vector3(0, offset * spawnedPlates.Count, 0);

			spawnedPlates.Add(plate.gameObject);
		}
	}
}