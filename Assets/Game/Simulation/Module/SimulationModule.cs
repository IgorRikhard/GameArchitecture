using Core.DI;
using Core.Modules;

namespace UnityEngine
{
    [CreateAssetMenu(menuName = "Config/SimulationModule", fileName = "SimulationModule")]
    public class SimulationModule : GameModule
    {
        public override void Install(ServiceContainer container)
        {
            container.InstantiateAndBind<SimulatedLoadingOperation>(new SimulatedLoadingOperation.Settings("111", 1));
            container.InstantiateAndBind<SimulatedLoadingOperation2>(new SimulatedLoadingOperation2.Settings("222", 1));
            container.InstantiateAndBind<SimulatedLoadingOperation3>(new SimulatedLoadingOperation3.Settings("333", 1));
            container.InstantiateAndBind<SimulatedLoadingOperation4>(new SimulatedLoadingOperation4.Settings("444", 1));
        }
    }
}