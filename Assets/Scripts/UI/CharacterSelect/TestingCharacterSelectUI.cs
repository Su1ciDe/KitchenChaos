using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelect
{
	public class TestingCharacterSelectUI : MonoBehaviour
	{
		[SerializeField] private Button btnReady;

		private void Awake()
		{
			btnReady.onClick.AddListener(Ready);
		}

		private void Ready()
		{
			CharacterSelectManager.Instance.SetPlayerReady();
		}
	}
}