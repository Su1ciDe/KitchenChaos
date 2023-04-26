using UnityEngine.Events;

namespace Interfaces
{
	public interface IHasProgress
	{
		public event UnityAction<float, bool> OnProgressChanged;

	}
}