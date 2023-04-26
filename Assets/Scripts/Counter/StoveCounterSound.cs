using UnityEngine;

namespace Counter
{
	public class StoveCounterSound : MonoBehaviour
	{
		private AudioSource audioSource;
		private StoveCounter stoveCounter;

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			stoveCounter = GetComponentInParent<StoveCounter>();
		}

		private void Start()
		{
			stoveCounter.OnStateChanged += OnStoveCounterStateChanged;
		}

		private void OnStoveCounterStateChanged(StoveCounter.State state)
		{
			if (state is StoveCounter.State.Frying or StoveCounter.State.Fried)
				audioSource.Play();
			else
				audioSource.Pause();
		}
	}
}