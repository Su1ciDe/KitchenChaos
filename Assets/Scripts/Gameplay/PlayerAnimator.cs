using Unity.Netcode;
using UnityEngine;

namespace Gameplay
{
	public class PlayerAnimator : NetworkBehaviour
	{
		[SerializeField] private PlayerMovement playerMovement;

		private Animator animator;
		private static readonly int isWalking = Animator.StringToHash("IsWalking");

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			if (!IsOwner) return;
			animator.SetBool(isWalking, playerMovement.IsWalking);
		}
	}
}