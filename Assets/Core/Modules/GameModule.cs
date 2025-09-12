using UnityEngine;
using Core.DI;

namespace Core.Modules
{
	public abstract class GameModule : ScriptableObject
	{
		[SerializeField]
		private string _moduleName = "Unnamed Module";

		[SerializeField]
		private bool _enabled = true;

		[SerializeField]
		private int _priority = 0;

		public string ModuleName => _moduleName;
		public bool Enabled => _enabled;
		public int Priority => _priority;

		public abstract void Install(ServiceContainer container);
	}
}