using Network;
using UnityEngine;

namespace UI.CharacterSelect
{
	public class CharacterColorSelectUI : MonoBehaviour
	{
		[SerializeField] private CharacterColorSelectButtonUI colorSelectButtonPrefab;

		private void Awake()
		{
			for (int i = 0; i < KitchenGameMultiplayer.Instance.PlayerColorsCount; i++)
			{
				var colorSelect = Instantiate(colorSelectButtonPrefab, transform);
				colorSelect.Init(i);
			}
		}
	}
}