using UnityEngine;

namespace Counter
{
	public class StoveCounterVisual : MonoBehaviour
	{
		[SerializeField] private GameObject stoveOnGo;
		[SerializeField] private GameObject stoveOnParticles;

		private void Awake()
		{
			GetComponentInParent<StoveCounter>().OnStateChanged += OnStateChanged;
		}

		private void OnStateChanged(StoveCounter.State state)
		{
			bool showVisual = state is StoveCounter.State.Frying or StoveCounter.State.Fried;
			stoveOnGo.SetActive(showVisual);
			stoveOnParticles.SetActive(showVisual);
		}
	}
}