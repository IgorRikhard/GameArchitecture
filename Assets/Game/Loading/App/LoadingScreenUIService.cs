using System;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Game.Loading.Api;
using Game.Loading.UI.Bindings;
using Game.Loading.UI.ViewModels;
using Game.UI.App;
using UnityEngine;

namespace Game.Loading.App
{
    public class LoadingScreenUIService : ILoadingScreenUIService
    {
        private readonly UIService _uiService;
        private readonly Settings _settings;

        public LoadingScreenUIService(UIService uiService, Settings settings)
        {
            _uiService = uiService;
            _settings = settings;
        }
        
        public async UniTask<LoadingViewModelBinding> ShowLoadingScreen(LoadingViewModel viewModel)
        {
            return await _uiService.ShowUIAsync<LoadingViewModelBinding>(_settings.PrefabReference, viewModel);
        }

        public void HideLoadingScreen()
        {
            
        }

        [Serializable]
        public class Settings
        {
            [SerializeField] private PrefabReference _prefabReference;

            public PrefabReference PrefabReference => _prefabReference;
        }
    }
}