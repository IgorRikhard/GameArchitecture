namespace Core.DI
{
	public static class ServiceProvider
	{
		private static ServiceContainer _container;

		public static ServiceContainer Container => _container ??= new ServiceContainer();

		public static void SetContainer(ServiceContainer container)
		{
			_container = container;
		}
	}
}


