using UnityEngine;

namespace Core.Modules
{
	[CreateAssetMenu(menuName = "Config/Modules Config", fileName = "ModulesConfig")]
	public class ModulesConfig : ScriptableObject
	{
		[SerializeField]
		private GameModule[] _modules;

		public GameModule[] Modules => _modules;
	}
}


