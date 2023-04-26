using UnityEngine;
using Utility;

namespace Managers
{
	public class MusicManager : Singleton<MusicManager>
	{
		public float Volume => volume;
		private float volume = .5f;

		private AudioSource audioSource;

		private const string MUSIC_VOLUME_PLAYERPREFS = "MusicVolume";

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_PLAYERPREFS, .5f);
			audioSource.volume = volume;
		}

		public void ChangeVolume(float volumeValue)
		{
			volume = volumeValue;
			audioSource.volume = volume;
			PlayerPrefs.SetFloat(MUSIC_VOLUME_PLAYERPREFS, volume);
		}
	}
}