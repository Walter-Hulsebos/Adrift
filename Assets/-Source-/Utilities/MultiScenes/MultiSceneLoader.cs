using UnityEngine.SceneManagement;

public static class MultiSceneLoader
{
	public static void LoadMultiScene(in MultiScene multiScene, LoadSceneMode mode = LoadSceneMode.Additive)
	{
		foreach (MultiScene.SceneInfo __scene in multiScene.sceneAssets)
		{
			SceneManager.LoadScene(__scene.sceneReference, mode);
		}
	}

}