using Counter;

namespace UI
{
	public class StoveBurningWarningUI : BaseUI
	{
		private StoveCounter stoveCounter;
		private const float burnShowProgressAmount = .5f;

		private void Awake()
		{
			stoveCounter = GetComponentInParent<StoveCounter>();
		}

		private void Start()
		{
			stoveCounter.OnProgressChanged += OnStoveProgressChanged;

			Hide();
		}

		private void OnStoveProgressChanged(float progress, bool isAnimated)
		{
			if (stoveCounter.IsFried && progress >= burnShowProgressAmount)
				Show();
			else
				Hide();
		}
	}
}