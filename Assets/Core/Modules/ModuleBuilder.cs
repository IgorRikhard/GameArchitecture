using System.Linq;
using Core.DI;

namespace Core.Modules
{
	public static class ModuleBuilder
	{
		public static void InstallAll(ModulesConfig config, ServiceContainer container)
		{
			if (config == null || config.Modules == null) return;
			foreach (var module in config.Modules.Where(m => m != null && m.Enabled).OrderBy(m => m.Priority))
			{
				module.Install(container);
			}
		}
	}
}


