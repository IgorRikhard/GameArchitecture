using System.Linq;
using System.Threading;
using Core.DI;
using Cysharp.Threading.Tasks;
using Game.Loading.Api;

namespace Game.Loading.App
{
    public class LoadingService
    {
        private readonly LoadingRunner _loadingRunner;
        private readonly DICollection<ILoadingOperation> _loadingOperations;
        
        public LoadingService(LoadingRunner loadingRunner,
            DICollection<ILoadingOperation> loadingOperations)
        {
            _loadingRunner = loadingRunner;
            _loadingOperations = loadingOperations;
        }

        public async UniTask StartLoading(CancellationToken token)
        {
            await _loadingRunner.Run(_loadingOperations.ToList(), token);
        }
    }
}