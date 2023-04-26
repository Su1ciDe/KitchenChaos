using UnityEngine;

namespace Counter
{
	public class ContainerCounterVisual : MonoBehaviour
	{
		private Animator animator;
		private static readonly int openClose = Animator.StringToHash("OpenClose");

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Start()
		{
			GetComponentInParent<ContainerCounter>().OnPlayerGrabbedObject += OnPlayerGrabbedObject;
		}

		private void OnPlayerGrabbedObject()
		{
			animator.SetTrigger(openClose);
		}
	}
}