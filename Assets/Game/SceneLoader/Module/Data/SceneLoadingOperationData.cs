using Game.Loading.Api;

namespace Game.SceneLoader.Module.Data
{
	public class SceneLoadingOperationData : ILoadingOperationData
	{
		public string OperationType => "Scene Loading Operation";

		public string SceneName { get; }

		public SceneLoadingOperationData(string sceneName)
		{
			SceneName = sceneName;
		}

	}
}


