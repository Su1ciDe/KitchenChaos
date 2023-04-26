using UnityEngine;

namespace Gameplay
{
	public class PlayerAnimator : MonoBehaviour
	{
		[SerializeField] private PlayerMovement playerMovement;

		private Animator animator;

		private const string IS_WALKING = "IsWalking";

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			animator.SetBool(IS_WALKING, playerMovement.IsWalking);
		}
	}
}