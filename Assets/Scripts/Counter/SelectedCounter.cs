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
			Player.Instance.OnSelectedCounterChanged += OnSelectedCounterChanged;
		}

		private void OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
		{
			if (e.selectedCounter == clearCounter)
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