using UnityEngine;

namespace Gameplay
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private float moveSpeed = 7;
		[SerializeField] private float rotateDamping = 10;
		[SerializeField] private float playerRadius = .7f;
		[SerializeField] private float playerHeight = 2;

		[Space]
		[Space]
		[SerializeField] private GameInput gameInput;

		public bool IsWalking => isWalking;
		private bool isWalking;

		private readonly RaycastHit[] results = new RaycastHit[1];

		private void Update()
		{
			var inputVector = gameInput.GetMovementVectorNormalized();
			var moveDir = new Vector3(inputVector.x, 0, inputVector.y);

			Move(moveDir);
		}

		private void Move(Vector3 moveDir)
		{
			var moveDistance = moveSpeed * Time.deltaTime;

			if (Physics.CapsuleCastNonAlloc(transform.position, transform.position + playerHeight * Vector3.up, playerRadius, moveDir, results, moveDistance) <= 0)
				transform.position += moveDistance * moveDir;

			transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateDamping * Time.deltaTime);

			isWalking = moveDir != Vector3.zero;
		}
	}
}