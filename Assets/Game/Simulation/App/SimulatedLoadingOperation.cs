using System.Threading;
using Cysharp.Threading.Tasks;
using Loading;

namespace UnityEngine
{
    public class SimulatedLoadingOperation : ILoadingOperation
    {
        private readonly Settings _settings;

        public SimulatedLoadingOperation(Settings settings)
        {
            _settings = settings;
        }

        public string Description => _settings.description;

        public async UniTask Load(CancellationToken cancellationToken = default)
        {
            await UniTask.Delay((int)(_settings.duration * 1000), cancellationToken: cancellationToken);
        }
        public class Settings
        {
            public string description;
            public float duration;
            
            public Settings(string description, float duration)
            {
                this.description = description;
                this.duration = duration;
            }
        }
    }
}