using System.Linq;
using System.Threading;
using Core.DI;
using Cysharp.Threading.Tasks;
using Game.Loading.Api;
using Game.Loading.UI.ViewModels;

namespace Game.Loading.App
{
    public class LoadingService
    {
        private readonly LoadingRunner _loadingRunner;
        private readonly DICollection<ILoadingOperation> _loadingOperations;

        private readonly ILoadingScreenUIService _loadingScreenUIService;

        public LoadingService(LoadingRunner loadingRunner,
            DICollection<ILoadingOperation> loadingOperations,
            ILoadingScreenUIService loadingScreenUIService)
        {
            _loadingRunner = loadingRunner;
            _loadingOperations = loadingOperations;
            _loadingScreenUIService = loadingScreenUIService;
        }

        public async UniTask StartLoading(CancellationToken token)
        {
            await _loadingRunner.Run(_loadingOperations.ToList(), token);
        }
    }
}