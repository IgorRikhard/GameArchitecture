using System.Collections.Generic;

namespace Loading
{
	public interface ILoadingOperationFactory
	{
		System.Type DataType { get; }
		ILoadingOperation Create(ILoadingOperationData data);
		bool CanCreate(System.Type dataType);
	}
}
