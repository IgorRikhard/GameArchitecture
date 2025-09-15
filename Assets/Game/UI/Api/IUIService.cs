using System.Threading;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Game.UI.App;
using R3;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Api
{
    public interface IUIService
    {
        ReactiveCommand<IUIView> OnUIShown { get; }
        ReactiveCommand<IUIView> OnUIHidden { get; }

        UniTask<T> ShowUIAsync<T>(PrefabReference prefabRef, BaseViewModel viewModel = null,
            CancellationToken cancellationToken = default) 
            where T : Component, IUIView;

        void ShowUI(PrefabReference prefabRef, BaseViewModel viewModel = null);
        void HideUI<T>(LayerType layerType) where T : IUIView;
        void HideUI(IUIView uiView, LayerType layerType);
        void HideAllUI(LayerType layerType);
        void HideAllUI();
        List<T> GetActiveUIs<T>(LayerType layerType) where T : IUIView;
    }
}