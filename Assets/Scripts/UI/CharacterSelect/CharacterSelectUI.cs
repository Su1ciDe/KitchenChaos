using Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.CharacterSelect
{
	public class CharacterSelectUI : BaseUI
	{
		[SerializeField] private Button btnReady;
		[SerializeField] private Button btnMainMenu;

		private void Awake()
		{
			btnReady.onClick.AddListener(ClickReady);
			btnMainMenu.onClick.AddListener(ClickMainMenu);
		}

		private void ClickMainMenu()
		{
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.Scenes.MainMenu);
		}

		private void ClickReady()
		{
			CharacterSelectManager.Instance.SetPlayerReady();
		}
	}
}