using R3;
using UnityEngine;

namespace Game.Simulation.Utils
{
    public static class SimulateUtils
    {
        public static Observable<Unit> GetSpaceClickedObservable()
        {
            return Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Space));
        }
    }
}