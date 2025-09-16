using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Loading.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SceneLoader.Module.App
{
    public class SceneLoadingOperation : ILoadingOperation
    {
        private readonly string _sceneName;

        public SceneLoadingOperation(Settings settings)
        {
            _sceneName = settings.sceneName;
        }

        public string Description => $"Loading scene '{_sceneName}'";

        public async UniTask Load(CancellationToken cancellationToken = default)
        {
            var async = SceneManager.LoadSceneAsync(_sceneName);
            await async.ToUniTask(cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            [SerializeField] private string _sceneName;

            public string sceneName => _sceneName;
        }
    }
}