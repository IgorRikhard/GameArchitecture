using System.Threading;
using Cysharp.Threading.Tasks;

namespace Loading
{
	public interface ILoadingOperation
	{
		string Description { get; }
		UniTask Load(CancellationToken cancellationToken = default);
	}
}
