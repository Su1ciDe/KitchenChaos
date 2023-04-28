using Managers;
using UnityEngine;

namespace Counter
{
	public class StoveCounterSound : MonoBehaviour
	{
		private AudioSource audioSource;
		private StoveCounter stoveCounter;

		private const float burnShowProgressAmount = .5f;

		private float warningSoundTimer;
		private const float warningSoundTimerMax = .2f;
		private bool shouldPlayWarningSound;

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			stoveCounter = GetComponentInParent<StoveCounter>();
		}

		private void Start()
		{
			stoveCounter.OnStateChanged += OnStoveCounterStateChanged;
			stoveCounter.OnProgressChanged += OnStoveProgressChanged;
		}

		private void Update()
		{
			PlayWarningSound();
		}

		private void PlayWarningSound()
		{
			if (!shouldPlayWarningSound) return;

			warningSoundTimer -= Time.deltaTime;
			if (warningSoundTimer <= 0)
			{
				warningSoundTimer = warningSoundTimerMax;
				SoundManager.Instance.PlayWarningSound(transform.position);
			}
		}

		private void OnStoveProgressChanged(float progress, bool isAnimated)
		{
			shouldPlayWarningSound = stoveCounter.IsFried && progress >= burnShowProgressAmount;
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