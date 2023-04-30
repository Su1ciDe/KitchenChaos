using Managers;
using UnityEngine;

namespace Gameplay
{
	public class PlayerSounds : MonoBehaviour
	{
		[SerializeField] private float footstepVolume = .5f;

		private Player player;
		private PlayerMovement playerMovement;

		private float footstepTimer;
		private readonly float footstepTimerMax = .25f;

		private void Awake()
		{
			player = GetComponent<Player>();
			playerMovement = player.GetComponent<PlayerMovement>();
		}

		private void Update()
		{
			footstepTimer -= Time.deltaTime;
			if (footstepTimer < 0)
			{
				if (playerMovement.IsWalking)
				{
					footstepTimer = footstepTimerMax;
					SoundManager.Instance.PlayFootstepSound(transform.position, footstepVolume);
				}
			}
		}
	}
}