using System.Linq;
using Core.DI;
using Cysharp.Threading.Tasks;

namespace Loading
{
    public class LoadingService
    {
        private readonly LoadingRunner _loadingRunner;
        private readonly DICollection<ILoadingOperation> _loadingOperations;

        public LoadingService(LoadingRunner loadingRunner, DICollection<ILoadingOperation> loadingOperations)
        {
            _loadingRunner = loadingRunner;
            _loadingOperations = loadingOperations;
        }
        
        public UniTask StartLoading()
        {
            return _loadingRunner.Run(_loadingOperations.ToList(), null);
        }
    }
}