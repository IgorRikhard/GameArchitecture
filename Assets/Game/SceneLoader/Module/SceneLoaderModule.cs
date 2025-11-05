using Core.DI;
using Core.Modules;
using Game.SceneLoader.Module.App;
using UnityEngine;
using ServiceContainer = _UPM.HappyDI.Runtime.ServiceContainer;

namespace Game.SceneLoader.Module
{
    [CreateAssetMenu(menuName = "Config/SceneLoader", fileName = "SceneLoader")]
    public class SceneLoader : GameModule
    {
        [SerializeField]
        private SceneLoadingOperation.Settings _sceneLoadingOperationSettings;
        public override void Install(ServiceContainer container)
        {
            container.InstantiateAndBind<SceneLoadingOperation>(_sceneLoadingOperationSettings);
        }
    }
}