using Gameplay;
using UnityEngine;

namespace Counter
{
	public class SelectedCounter : MonoBehaviour
	{
		[SerializeField] private GameObject[] visuals;

		private BaseCounter clearCounter;

		private void Awake()
		{
			clearCounter = GetComponentInParent<BaseCounter>();
		}

		private void Start()
		{
			if (Player.LocalInstance)
				Player.LocalInstance.OnSelectedCounterChanged += OnSelectedCounterChanged;
			else
				Player.OnAnyPlayerSpawned += OnAnyPlayerSpawned;
		}

		private void OnDisable()
		{
			Player.OnAnyPlayerSpawned -= OnAnyPlayerSpawned;
		}

		private void OnAnyPlayerSpawned()
		{
			if (!Player.LocalInstance) return;
			Player.LocalInstance.OnSelectedCounterChanged -= OnSelectedCounterChanged;
			Player.LocalInstance.OnSelectedCounterChanged += OnSelectedCounterChanged;
		}

		private void OnSelectedCounterChanged(BaseCounter selectedCounter)
		{
			if (selectedCounter.Equals(clearCounter))
				Show();
			else
				Hide();
		}

		private void Show()
		{
			foreach (var visual in visuals)
				visual.SetActive(true);
		}

		private void Hide()
		{
			foreach (var visual in visuals)
				visual.SetActive(false);
		}
	}
}