using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Loading.Api;
using UnityEngine.SceneManagement;

namespace Game.Loading.App
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
