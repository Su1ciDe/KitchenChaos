using UnityEngine;

namespace Counter
{
	public class CuttingCounterVisual : MonoBehaviour
	{
		private Animator animator;
		private static readonly int cut = Animator.StringToHash("Cut");

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Start()
		{
			GetComponentInParent<CuttingCounter>().OnCut += OnCut;
		}

		private void OnCut()
		{
			animator.SetTrigger(cut);
		}
	}
}