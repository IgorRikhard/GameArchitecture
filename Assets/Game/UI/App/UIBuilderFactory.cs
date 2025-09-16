using UnityEngine;
using Core.Utils;

namespace Game.UI.App
{
    public static class UIBuilderFactory
    {
        public static UIBuilder CreateUIBuilder(Transform uiRoot, bool enablePooling = true, int maxPoolSize = 10)
        {
            if (uiRoot == null)
            {
                throw new System.ArgumentNullException(nameof(uiRoot));
            }
            
            return new UIBuilder(uiRoot, enablePooling, maxPoolSize);
        }
        
        public static UIBuilder CreateUIBuilderWithDefaultSettings(Transform uiRoot)
        {
            return CreateUIBuilder(uiRoot, enablePooling: true, maxPoolSize: 10);
        }
        
        public static UIBuilder CreateUIBuilderWithoutPooling(Transform uiRoot)
        {
            return CreateUIBuilder(uiRoot, enablePooling: false, maxPoolSize: 0);
        }
        
        // Example method showing how to use UIBuilder with PrefabReference
        public static async Cysharp.Threading.Tasks.UniTask<T> ShowUIWithBuilder<T>(
            UIBuilder uiBuilder, 
            PrefabReference prefabRef, 
            LayerType layerType, 
            BaseViewModel viewModel = null) 
            where T : UnityEngine.Component, IUIView
        {
            if (uiBuilder == null)
            {
                throw new System.ArgumentNullException(nameof(uiBuilder));
            }
            
            if (prefabRef == null)
            {
                throw new System.ArgumentNullException(nameof(prefabRef));
            }
            
            return await uiBuilder.ShowUI<T>(prefabRef, viewModel);
        }
    }
}
