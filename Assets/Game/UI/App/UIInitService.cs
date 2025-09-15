using System;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Game.UI.Api;
using UnityEngine;

namespace Game.UI.App
{
    public class UIInitService
    {
        private readonly Settings _settings;
        private readonly UIConfiguration _configuration;
        private readonly IUIServiceInitialize _uiServiceInitializer;
        private MainUI _mainUI;

        public UIInitService(Settings settings,
            UIConfiguration configuration,
            IUIServiceInitialize uiServiceInitializer)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _configuration = configuration;
            _uiServiceInitializer = uiServiceInitializer;
        }

        public async UniTask LoadMainUIAsync()
        {
            if (_settings.mainUIPrefabRef == null)
            {
                Debug.LogError("EntryPoint: _mainUIPrefabRef is not assigned");
                return;
            }

            try
            {
                var go = await PrefabLoader.LoadPrefabByRef(_settings.mainUIPrefabRef);
                if (go == null)
                {
                    Debug.LogError("EntryPoint: Failed to instantiate MainUI prefab");
                    return;
                }

                _mainUI = go.GetComponent<MainUI>();
                if (_mainUI == null)
                {
                    Debug.LogError("EntryPoint: MainUI component not found on instantiated prefab");
                    UnityEngine.Object.Destroy(go);
                    return;
                }
                UnityEngine.Object.DontDestroyOnLoad(go);

                SetupUIService();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"EntryPoint: Error while loading MainUI - {ex.Message}");
            }
        }

        private void SetupUIService()
        {
            var builder = _configuration.CreateUIBuilder(_mainUI.transform);
            _uiServiceInitializer.InitializeUIBuilder(builder, _mainUI.transform, _configuration.EnablePooling, _configuration.MaxPoolSize);
        }

        [Serializable]
        public class Settings
        {
            [SerializeField]
            private PrefabReference _mainUIPrefabRef;

            public PrefabReference mainUIPrefabRef => _mainUIPrefabRef;
        }
    }
}