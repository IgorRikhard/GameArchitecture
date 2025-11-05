using System;
using System.Threading;
using _UPM.HappyDI.Runtime;
using Core.Modules;
using Cysharp.Threading.Tasks;
using Game.Loading.App;
using Game.UI.App;
using UnityEngine;

namespace Game.EntryPoint.UI
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private ModulesConfig _modulesConfig;

        private CancellationTokenSource _loadingCts = new();
        
        private async UniTaskVoid RunAsync()
        {
            var di = ServiceProvider.Container;
            // Install enabled modules
            ModuleBuilder.InstallAll(_modulesConfig, di);

            // Resolve and wait for MainUI to load
            var uiInit = di.Resolve<UIInitService>();
            await uiInit.LoadMainUIAsync();

            var loadingService = di.Resolve<LoadingService>();
            await loadingService.StartLoading(_loadingCts.Token);
        }
    }
}