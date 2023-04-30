using UnityEngine;
using UnityEngine.Animations;

namespace Utility
{
	public class FollowTransform : MonoBehaviour
	{
		private Transform targetTransform;

		private PositionConstraint positionConstraint;

		private void Awake()
		{
			positionConstraint = GetComponent<PositionConstraint>();
		}

		public void SetTargetTransform(Transform target)
		{
			targetTransform = target;

			var source = new ConstraintSource { sourceTransform = targetTransform, weight = 1 };
			positionConstraint.AddSource(source);
			positionConstraint.constraintActive = true;
		}
	}
}