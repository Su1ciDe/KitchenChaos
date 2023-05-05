using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Utility
{
	public static class Loader
	{
		public enum Scenes
		{
			MainMenu,
			GameScene,
			LoadingScene,
			LobbyScene,
			CharacterSelectScene,
		}

		private static Scenes targetScene;

		public static void Load(Scenes targetSceneName)
		{
			targetScene = targetSceneName;
			SceneManager.LoadScene((int)Scenes.LoadingScene);
		}

		public static void LoadNetwork(Scenes targetSceneName)
		{
			NetworkManager.Singleton.SceneManager.LoadScene(targetSceneName.ToString(), LoadSceneMode.Single);
		}

		public static void LoaderCallback()
		{
			SceneManager.LoadSceneAsync((int)targetScene);
		}
	}
}