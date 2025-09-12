using System;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.UI.App
{
    public class UIInitService
    {
        private readonly Settings _settings;

        public UIInitService(Settings settings)
        {
            _settings = settings;
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

                var mainUI = go.GetComponent<MainUI>();
                if (mainUI == null)
                {
                    Debug.LogError("EntryPoint: MainUI component not found on instantiated prefab");
                    UnityEngine.Object.Destroy(go);
                    return;
                }

                UnityEngine.Object.DontDestroyOnLoad(go);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"EntryPoint: Error while loading MainUI - {ex.Message}");
            }
        }

        [Serializable]
        public class Settings
        {
            [SerializeField] private PrefabReference _mainUIPrefabRef;

            public PrefabReference mainUIPrefabRef => _mainUIPrefabRef;
        }
    }
}