using Core.DI;
using Core.Modules;
using Game.Loading.Api;
using Game.Loading.App;
using UnityEngine;

namespace Game.Loading.Module
{
    [CreateAssetMenu(menuName = "Config/LoadingModule", fileName = "LoadingModule")]
    public class LoadingModule : GameModule
    {
        [SerializeField]
    private LoadingScreenUIService.Settings _uiSettings;
        public override void Install(ServiceContainer container)
        {
            container.BindCollection<ILoadingOperation>();
            container.InstantiateAndBind<LoadingScreenUIService>(_uiSettings);
            container.InstantiateAndBind<LoadingRunner>();
            container.InstantiateAndBind<LoadingService>();
        }
    }
}