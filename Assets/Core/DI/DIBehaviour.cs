using UnityEngine;

namespace Core.DI
{
	public abstract class DIBehaviour : MonoBehaviour
	{
		protected virtual void Awake()
		{
			ServiceProvider.Container.Bind(this);
		}
	}
}


