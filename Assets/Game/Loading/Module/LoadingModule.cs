using Core.DI;
using Core.Modules;
using Loading;

namespace UnityEngine
{
    [CreateAssetMenu(menuName = "Config/LoadingModule", fileName = "LoadingModule")]
    public class LoadingModule : GameModule
    {
        public override void Install(ServiceContainer container)
        {
            container.BindCollection<ILoadingOperation>();
            container.Bind<LoadingRunner>();
            container.Bind<LoadingService>();
        }
    }
}