using Unity.Netcode;
using UnityEngine;

namespace Gameplay
{
	public class PlayerMovement : NetworkBehaviour
	{
		[SerializeField] private float moveSpeed = 7;
		[SerializeField] private float rotateDamping = 10;
		[SerializeField] private float playerRadius = .7f;
		[SerializeField] private float playerHeight = 2;

		public bool IsWalking => isWalking;
		private bool isWalking;

		private readonly RaycastHit[] results = new RaycastHit[1];

		private void Update()
		{
			if (!IsOwner) return;
			var inputVector = GameInput.Instance.GetMovementVectorNormalized();
			var moveDir = new Vector3(inputVector.x, 0, inputVector.y);

			Move(moveDir);
		}

		private void Move(Vector3 moveDir)
		{
			var moveDistance = moveSpeed * Time.deltaTime;

			bool canMove = Physics.CapsuleCastNonAlloc(transform.position, transform.position + playerHeight * Vector3.up, playerRadius, moveDir, results, moveDistance) <= 0;
			if (!canMove)
			{
				var moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
				canMove = moveDir.x is < -.5f or > .5f && Physics.CapsuleCastNonAlloc(transform.position, transform.position + playerHeight * Vector3.up,
					playerRadius, moveDirX, results, moveDistance) <= 0;
				if (canMove)
					moveDir = moveDirX;
				else
				{
					var moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
					canMove = moveDir.z is < -.5f or > .5f && Physics.CapsuleCastNonAlloc(transform.position, transform.position + playerHeight * Vector3.up, playerRadius,
						moveDirZ, results, moveDistance) <= 0;
					if (canMove)
						moveDir = moveDirZ;
				}
			}

			if (canMove)
			{
				transform.position += moveDistance * moveDir;
			}

			transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateDamping * Time.deltaTime);

			isWalking = moveDir != Vector3.zero;
		}
	}
}