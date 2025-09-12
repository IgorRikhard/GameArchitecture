using System.Threading;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Loading
{
	public class SceneLoadingOperation : ILoadingOperation
	{
		private readonly string _sceneName;

		public SceneLoadingOperation(string sceneName)
		{
			_sceneName = sceneName;
		}

		public string Description => $"Loading scene '{_sceneName}'";

		public async UniTask Load(CancellationToken cancellationToken = default)
		{
			var async = SceneManager.LoadSceneAsync(_sceneName);
			await async.ToUniTask(cancellationToken: cancellationToken);
		}
	}
}
