using UnityEngine.SceneManagement;

namespace Utility
{
	public static class Loader
	{
		public enum Scenes
		{
			MainMenu,
			GameScene,
			LoadingScene
		}

		private static Scenes targetScene;

		public static void Load(Scenes targetSceneName)
		{
			targetScene = targetSceneName;
			SceneManager.LoadScene((int)Scenes.LoadingScene);
		}

		public static void LoaderCallback()
		{
			SceneManager.LoadSceneAsync((int)targetScene);
		}
	}
}