using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Core.Utils;
using Game.UI.Api;
using R3;

namespace Game.UI.App
{
    public class UIService : IUIService, IUIServiceInitialize, IDisposable
    {
        private UIBuilder _uiBuilder;
        
        public UIBuilder UIBuilder => _uiBuilder;

        public ReactiveCommand<IUIView> OnUIShown { get; } = new();
        public ReactiveCommand<IUIView> OnUIHidden { get; } = new();
        
        public void InitializeUIBuilder(UIBuilder uiBuilder, Transform uiRoot = null, bool enablePooling = true, int maxPoolSize = 10)
        {
            if (_uiBuilder != null)
            {
                _uiBuilder.OnUIShown -= OnUIBuilderUIShown;
                _uiBuilder.OnUIHidden -= OnUIBuilderUIHidden;
                _uiBuilder.Dispose();
            }

            _uiBuilder = uiBuilder;
            _uiBuilder.OnUIShown += OnUIBuilderUIShown;
            _uiBuilder.OnUIHidden += OnUIBuilderUIHidden;
        }
        
        public async UniTask<T> ShowUIAsync<T>(PrefabReference prefabRef, BaseViewModel viewModel = null, CancellationToken cancellationToken = default) 
            where T : Component, IUIView
        {
            if (_uiBuilder == null)
            {
                Debug.LogError("UIBuilder is not assigned to UIService");
                return null;
            }
            
            return await _uiBuilder.ShowUI<T>(prefabRef, viewModel, cancellationToken);
        }

        public void ShowUI(PrefabReference prefabRef, BaseViewModel viewModel = null)
        {
            throw new NotImplementedException();
        }

        public void HideUI<T>(LayerType layerType) where T : IUIView
        {
            _uiBuilder?.HideUI<T>(layerType);
        }
        
        public void HideUI(IUIView uiView, LayerType layerType)
        {
            _uiBuilder?.HideUI(uiView, layerType);
        }
        
        public void HideAllUI(LayerType layerType)
        {
            _uiBuilder?.HideAllUI(layerType);
        }
        
        public void HideAllUI()
        {
            _uiBuilder?.HideAllUI();
        }
 
        public System.Collections.Generic.List<T> GetActiveUIs<T>(LayerType layerType) where T : IUIView
        {
            return _uiBuilder?.GetActiveUIs<T>(layerType);
        }
        
        private void OnUIBuilderUIShown(LayerType layerType, IUIView uiView)
        {
            OnUIShown?.Execute(uiView);
        }
        
        private void OnUIBuilderUIHidden(LayerType layerType, IUIView uiView)
        {
            OnUIHidden?.Execute(uiView);
        }

        public void Dispose()
        {
            if (_uiBuilder != null)
            {
                _uiBuilder.OnUIShown -= OnUIBuilderUIShown;
                _uiBuilder.OnUIHidden -= OnUIBuilderUIHidden;
                _uiBuilder.Dispose();
                _uiBuilder = null;
            }
            
            OnUIShown.Dispose();
            OnUIHidden.Dispose();

        }
    }
}
