using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "AudioClips", menuName = "KitchenChaos/AudioClips", order = 50)]
	public class AudioClipRefsSO : ScriptableObject
	{
		public AudioClip[] Chop;
		public AudioClip[] DeliveryFail;
		public AudioClip[] DeliverySuccess;
		public AudioClip[] Footsteps;
		public AudioClip[] ObjectDrop;
		public AudioClip[] ObjectPickup;
		public AudioClip StoveSizzle;
		public AudioClip[] Trash;
		public AudioClip[] Warning;
		
	}
}