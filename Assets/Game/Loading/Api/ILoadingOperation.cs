using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Loading.Api
{
	public interface ILoadingOperation
	{
		string Description { get; }
		UniTask Load(CancellationToken cancellationToken = default);
	}
}
