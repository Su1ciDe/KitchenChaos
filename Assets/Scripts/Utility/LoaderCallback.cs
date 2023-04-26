﻿using UnityEngine;

namespace Utility
{
	public class LoaderCallback : MonoBehaviour
	{
		private bool isFirstUpdate = true;

		private void Update()
		{
			if (!isFirstUpdate) return;
			isFirstUpdate = false;
			Loader.LoaderCallback();
		}
	}
}