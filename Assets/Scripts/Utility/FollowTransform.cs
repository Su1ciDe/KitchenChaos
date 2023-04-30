using UnityEngine;
using UnityEngine.Animations;

namespace Utility
{
	[RequireComponent(typeof(PositionConstraint))]
	public class FollowTransform : MonoBehaviour
	{
		private Transform targetTransform;

		private PositionConstraint positionConstraint;
		private ConstraintSource source;

		private void Awake()
		{
			positionConstraint = GetComponent<PositionConstraint>();
		}

		public void SetTargetTransform(Transform target)
		{
			if (positionConstraint.sourceCount > 0)
				positionConstraint.RemoveSource(0);
			targetTransform = target;

			source = new ConstraintSource { sourceTransform = targetTransform, weight = 1 };
			positionConstraint.AddSource(source);
			positionConstraint.constraintActive = true;
		}
	}
}