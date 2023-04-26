using System;
using UnityEngine;

namespace Utility
{
	public class LookAtCamera : MonoBehaviour
	{
		private enum LookMode
		{
			LookAt,
			LookAtInverted,
			CameraForward,
			CameraForwardInverted,
		}

		[SerializeField] private LookMode lookMode;

		private Camera mainCamera;

		private void Awake()
		{
			mainCamera = Camera.main;
			if (TryGetComponent(out Canvas canvas))
				canvas.worldCamera = mainCamera;
		}

		private void LateUpdate()
		{
			switch (lookMode)
			{
				case LookMode.LookAt:
					transform.LookAt(mainCamera.transform);
					break;
				case LookMode.LookAtInverted:
					var dirFromCamera = transform.position - mainCamera.transform.position;
					transform.LookAt(transform.position + dirFromCamera);
					break;
				case LookMode.CameraForward:
					transform.forward = mainCamera.transform.forward;
					break;
				case LookMode.CameraForwardInverted:
					transform.forward = -mainCamera.transform.forward;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}