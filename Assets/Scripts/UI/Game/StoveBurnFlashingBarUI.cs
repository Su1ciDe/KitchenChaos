using Counter;
using UnityEngine;

namespace UI
{
	public class StoveBurnFlashingBarUI : MonoBehaviour
	{
		private StoveCounter stoveCounter;
		private const float burnShowProgressAmount = .5f;

		private Animator animator;
		private static readonly int isFlashing = Animator.StringToHash("IsFlashing");

		private void Awake()
		{
			stoveCounter = GetComponentInParent<StoveCounter>();
			animator = GetComponent<Animator>();
		}

		private void Start()
		{
			stoveCounter.OnProgressChanged += OnStoveProgressChanged;
			animator.SetBool(isFlashing, false);
		}

		private void OnStoveProgressChanged(float progress, bool isAnimated)
		{
			animator.SetBool(isFlashing, stoveCounter.IsFried && progress >= burnShowProgressAmount);
		}
	}
}