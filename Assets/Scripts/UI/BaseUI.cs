using UnityEngine;

namespace UI
{
	public class BaseUI : MonoBehaviour
	{
		protected virtual void Show()
		{
			gameObject.SetActive(true);
		}

		protected virtual void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}