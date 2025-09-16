using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Utils;
using Cysharp.Threading.Tasks;
using System.Threading;
using Game.Layers;

namespace Game.UI.App
{
    public class UIBuilder
    {
        private readonly Transform _uiRoot;
        private readonly bool _enablePooling;
        private readonly int _maxPoolSize;

        private Dictionary<LayerType, Transform> _layerContainers = new();
        private Dictionary<LayerType, List<IUIView>> _activeUIViews = new();
        private Dictionary<string, Queue<GameObject>> _uiPool = new();
        private Dictionary<IUIView, string> _uiViewPoolKeys = new();

        public event Action<LayerType, IUIView> OnUIShown;
        public event Action<LayerType, IUIView> OnUIHidden;

        public UIBuilder(Transform uiRoot, bool enablePooling = true, int maxPoolSize = 10)
        {
            _uiRoot = uiRoot ?? throw new ArgumentNullException(nameof(uiRoot));
            _enablePooling = enablePooling;
            _maxPoolSize = maxPoolSize;

            InitializeLayers();
        }

        private void InitializeLayers()
        {
            var layers = _uiRoot.GetComponentsInChildren<LayerBinding>();
            // Create layer containers for each LayerType
            foreach (var layer in layers)
            {
                _layerContainers[layer.LayerType] = layer.transform;
                _activeUIViews[layer.LayerType] = new List<IUIView>();
            }
        }

        public async UniTask<T> ShowUI<T>(PrefabReference prefabRef, BaseViewModel viewModel = null,
            CancellationToken cancellationToken = default)
            where T : Component, IUIView
        {
            var poolKey = GeneratePoolKey<T>(prefabRef, viewModel);
            var uiView = await GetOrCreateUI<T>(prefabRef, viewModel, cancellationToken);
            if (uiView == null) return null;

            // Store pool key for later use
            _uiViewPoolKeys[uiView] = poolKey;

            // Set viewmodel if provided
            if (viewModel != null && uiView is IViewModelContainer container)
            {
                container.SetViewModel(viewModel);
            }

            // Set parent to appropriate layer
            uiView.transform.root.SetParent(_layerContainers[prefabRef.LayerType], false);

            // Show the UI
            uiView.Show();

            // Add to active views
            _activeUIViews[prefabRef.LayerType].Add(uiView);

            OnUIShown?.Invoke(prefabRef.LayerType, uiView);

            return uiView as T;
        }

        public void HideUI<T>(LayerType layerType) where T : IUIView
        {
            var uiView = _activeUIViews[layerType].FirstOrDefault(v => v is T);
            if (uiView != null)
            {
                HideUI(uiView, layerType);
            }
        }

        public void HideUI(IUIView uiView, LayerType layerType)
        {
            if (uiView == null) return;

            uiView.Hide();
            _activeUIViews[layerType].Remove(uiView);

            if (_enablePooling)
            {
                ReturnToPool(uiView);
            }
            else
            {
                UnityEngine.Object.Destroy(((MonoBehaviour)uiView).gameObject);
            }

            OnUIHidden?.Invoke(layerType, uiView);
        }

        public void HideAllUI(LayerType layerType)
        {
            var viewsToHide = _activeUIViews[layerType].ToList();
            foreach (var view in viewsToHide)
            {
                HideUI(view, layerType);
            }
        }

        public void HideAllUI()
        {
            foreach (LayerType layerType in Enum.GetValues(typeof(LayerType)))
            {
                HideAllUI(layerType);
            }
        }

        public T GetActiveUI<T>(LayerType layerType) where T : IUIView
        {
            return _activeUIViews[layerType].OfType<T>().FirstOrDefault();
        }

        public List<T> GetActiveUIs<T>(LayerType layerType) where T : IUIView
        {
            return _activeUIViews[layerType].OfType<T>().ToList();
        }

        private async UniTask<T> GetOrCreateUI<T>(PrefabReference prefabRef, BaseViewModel viewModel,
            CancellationToken cancellationToken) where T : Component, IUIView
        {
            if (_enablePooling)
            {
                var poolKey = GeneratePoolKey<T>(prefabRef, viewModel);
                var pooledUI = GetFromPool<T>(poolKey);
                if (pooledUI != null)
                {
                    return pooledUI;
                }
            }

            return await CreateNewUI<T>(prefabRef, cancellationToken);
        }

        private string GeneratePoolKey<T>(PrefabReference prefabRef, BaseViewModel viewModel)
            where T : Component, IUIView
        {
            var baseKey = $"{typeof(T).Name}_{prefabRef.name}";

            if (viewModel != null)
            {
                // Create a unique key based on viewmodel type and content
                var viewModelType = viewModel.GetType().Name;
                var viewModelHash = viewModel.GetHashCode();
                return $"{baseKey}_{viewModelType}_{viewModelHash}";
            }

            return baseKey;
        }
        

        private T GetFromPool<T>(string poolKey) where T : Component, IUIView
        {
            if (!_uiPool.TryGetValue(poolKey, out var pool) || pool.Count == 0)
            {
                return null;
            }

            var pooledObject = pool.Dequeue();
            if (pooledObject == null)
            {
                return null;
            }

            return pooledObject.GetComponentInChildren<T>();
        }

        private void ReturnToPool(IUIView uiView)
        {
            // Get the pool key for this UI view
            if (!_uiViewPoolKeys.TryGetValue(uiView, out var poolKey))
            {
                // If no pool key found, destroy the object
                UnityEngine.Object.Destroy(((MonoBehaviour)uiView).gameObject);
                return;
            }

            if (!_uiPool.ContainsKey(poolKey))
            {
                _uiPool[poolKey] = new Queue<GameObject>();
            }

            var pool = _uiPool[poolKey];

            // Check pool size limit
            if (pool.Count >= _maxPoolSize)
            {
                UnityEngine.Object.Destroy(((MonoBehaviour)uiView).gameObject);
                _uiViewPoolKeys.Remove(uiView);
                return;
            }

            // Remove from parent and hide
            ((MonoBehaviour)uiView).transform.SetParent(null);
            uiView.Hide();

            pool.Enqueue(((MonoBehaviour)uiView).gameObject);

            // Keep the pool key for potential reuse
        }

        private async UniTask<T> CreateNewUI<T>(PrefabReference prefabRef, CancellationToken cancellationToken)
            where T : Component, IUIView
        {
            if (prefabRef == null)
            {
                Debug.LogError($"PrefabReference is null for UI type: {typeof(T).Name}");
                return null;
            }

            try
            {
                var prefab = await PrefabLoader.LoadPrefabByRef(prefabRef, cancellationToken);
                if (prefab == null)
                {
                    Debug.LogError($"Failed to load prefab for UI type: {typeof(T).Name}");
                    return null;
                }

                var uiComponent = prefab.GetComponentInChildren<T>(true);
                if (uiComponent == null)
                {
                    Debug.LogError($"UI component {typeof(T).Name} not found on prefab");
                    UnityEngine.Object.Destroy(prefab);
                    return null;
                }

                return uiComponent;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating UI {typeof(T).Name}: {ex.Message}");
                return null;
            }
        }

        public void ClearPool()
        {
            foreach (var pool in _uiPool.Values)
            {
                while (pool.Count > 0)
                {
                    var obj = pool.Dequeue();
                    if (obj != null)
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                }
            }

            _uiPool.Clear();
            _uiViewPoolKeys.Clear();
        }

        public void ClearPool<T>() where T : IUIView
        {
            var keysToRemove = new List<string>();

            foreach (var kvp in _uiPool)
            {
                var poolKey = kvp.Key;
                var pool = kvp.Value;

                // Check if this pool contains objects of type T
                if (poolKey.StartsWith(typeof(T).Name + "_"))
                {
                    while (pool.Count > 0)
                    {
                        var obj = pool.Dequeue();
                        if (obj != null)
                        {
                            UnityEngine.Object.Destroy(obj);
                        }
                    }

                    keysToRemove.Add(poolKey);
                }
            }

            foreach (var key in keysToRemove)
            {
                _uiPool.Remove(key);
            }
        }

        public void Dispose()
        {
            ClearPool();
            OnUIShown = null;
            OnUIHidden = null;
        }
    }
}