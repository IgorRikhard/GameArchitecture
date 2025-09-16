using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Loading.Api;
using Game.Loading.UI.ViewModels;
using R3;
using UnityEngine;

namespace Game.Loading.App
{
    public class LoadingRunner
    {
        private readonly ILoadingScreenUIService _loadingScreenUIService;

        public LoadingRunner(ILoadingScreenUIService loadingScreenUIService)
        {
            _loadingScreenUIService = loadingScreenUIService;
        }
        
        public async UniTask Run(List<ILoadingOperation> operations, CancellationToken cancellationToken = default)
        {
            if (operations == null || operations.Count == 0)
            {
                return;
            }
            
            var progressReactiveProperty = new ReactiveProperty<float>(0f);
            var progressTextProperty = new ReactiveProperty<string>();
            
            using var loadingScreenView = new LoadingViewModel(progressReactiveProperty, progressTextProperty);
            _loadingScreenUIService.ShowLoadingScreen(loadingScreenView);
            
            float completed = 0f;
            for (int i = 0; i < operations.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var op = operations[i];
                progressTextProperty.Value = op.Description;
                progressReactiveProperty.Value = completed / operations.Count;
                
                await op.Load(cancellationToken);

                completed += 1f;
                progressReactiveProperty.Value = completed / operations.Count;
                Debug.Log("completed / operations.Count: " + (completed / operations.Count));
            }
        }
    }
}