using UnityEngine;

namespace _UPM.HappyDI.Runtime
{
	public abstract class DIBehaviour : MonoBehaviour
	{
		protected virtual void Awake()
		{
			ServiceProvider.Container.Bind(this);
		}
	}
}


