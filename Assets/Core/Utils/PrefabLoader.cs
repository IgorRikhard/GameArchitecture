using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace Core.Utils
{
	public static class PrefabLoader
	{
		public static async UniTask<GameObject> LoadPrefabByAddressables(string addressablesName, CancellationToken cancellationToken = default)
		{
			var handle = Addressables.LoadAssetAsync<GameObject>(addressablesName);
			var prefab = await handle.ToUniTask(cancellationToken: cancellationToken);
			if (prefab == null)
			{
				Debug.LogError($"Failed to load prefab from address: {addressablesName}");
				return null;
			}
			return Object.Instantiate(prefab);
		}
		
		public static async UniTask<GameObject> LoadPrefabByRef(PrefabReference reference, CancellationToken cancellationToken = default)
		{
			var addressablesName = reference.name + "@Prefab";
			return await LoadPrefabByAddressables(addressablesName, cancellationToken);
		}

		public static async UniTask<TComponent> LoadComponent<TComponent>(string address, CancellationToken cancellationToken = default)
			where TComponent : Component
		{
			GameObject instance = null;
			try
			{
				instance = await LoadPrefabByAddressables(address, cancellationToken);
				if (instance == null)
				{
					return null;
				}

				var component = instance.GetComponent<TComponent>();
				if (component == null)
				{
					Debug.LogError($"Component {typeof(TComponent).Name} not found on prefab at address: {address}");
					Object.Destroy(instance);
					return null;
				}

				return component;
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error loading {typeof(TComponent).Name} from Addressables: {ex.Message}");
				if (instance != null)
				{
					Object.Destroy(instance);
				}
				throw;
			}
		}
	}
}