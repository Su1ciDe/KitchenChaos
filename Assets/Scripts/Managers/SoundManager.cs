using Counter;
using Gameplay;
using ScriptableObjects;
using UnityEngine;
using Utility;

namespace Managers
{
	public class SoundManager : Singleton<SoundManager>
	{
		[SerializeField] private AudioClipRefsSO audioClipRefsSO;

		public float Volume => volume;
		private float volume = 1;

		private const string SOUNDS_EFFECT_VOLUME_PLAYERPREFS = "SoundEffectVolume";

		private void Awake()
		{
			volume = PlayerPrefs.GetFloat(SOUNDS_EFFECT_VOLUME_PLAYERPREFS, 1f);
		}

		private void Start()
		{
			DeliveryManager.Instance.OnRecipeSuccess += OnRecipeSuccess;
			DeliveryManager.Instance.OnRecipeFail += OnRecipeFailed;
			CuttingCounter.OnAnyCut += OnCut;
			Player.Instance.OnPickedSomething += OnPlayerPickedSomething;
			BaseCounter.OnAnyObjectPlaced += CounterOnAnyObjectPlaced;
			TrashCounter.OnAnyObjectTrashed += OnAnyObjectTrashed;
		}

		private void OnDestroy()
		{
			CuttingCounter.OnAnyCut -= OnCut;
			BaseCounter.OnAnyObjectPlaced -= CounterOnAnyObjectPlaced;
			TrashCounter.OnAnyObjectTrashed -= OnAnyObjectTrashed;
		}

		public void PlayFootstepSound(Vector3 position, float volume)
		{
			PlaySound(audioClipRefsSO.Footsteps, position, volume);
		}

		public void PlayCountdownSound()
		{
			PlaySound(audioClipRefsSO.Warning, Vector3.zero);
		}

		public void PlayWarningSound(Vector3 position)
		{
			PlaySound(audioClipRefsSO.Warning, position);
		}

		private void OnAnyObjectTrashed(Vector3 position)
		{
			PlaySound(audioClipRefsSO.Trash, position);
		}

		private void CounterOnAnyObjectPlaced(Vector3 position)
		{
			PlaySound(audioClipRefsSO.ObjectDrop, position);
		}

		private void OnPlayerPickedSomething(Vector3 position)
		{
			PlaySound(audioClipRefsSO.ObjectPickup, position);
		}

		private void OnCut(Vector3 position)
		{
			PlaySound(audioClipRefsSO.Chop, position);
		}

		private void OnRecipeSuccess(Vector3 position)
		{
			PlaySound(audioClipRefsSO.DeliverySuccess, position);
		}

		private void OnRecipeFailed(Vector3 position)
		{
			PlaySound(audioClipRefsSO.DeliveryFail, position);
		}

		private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
		{
			if (volume <= 0) return;
			AudioSource.PlayClipAtPoint(audioClip, position, volume * volumeMultiplier);
		}

		private void PlaySound(AudioClip[] audioClips, Vector3 position, float volumeMultiplier = 1f)
		{
			PlaySound(audioClips[Random.Range(0, audioClips.Length)], position, volumeMultiplier);
		}

		public void ChangeVolume(float volumeValue)
		{
			volume = volumeValue;
			PlayerPrefs.SetFloat(SOUNDS_EFFECT_VOLUME_PLAYERPREFS, volume);
		}
	}
}