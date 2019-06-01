using System.Reflection;
using Autofac;
using NLog;
using Thesaurus;

namespace ThesaurusRunner
{
	internal class Program
	{
		private  static readonly Logger Log = LogManager.GetCurrentClassLogger();

		static void Main(string[] args)
		{
			Log.Info("Starting application...");

			// Note: I've designed the IThesaurus library to make use of dependency injection.
			// This means that this project does not need to know about the implementation at all.
			var container = CreateContainer();

			var app = container.Resolve<App>();
			app.Run();

			Log.Info("Exiting application...");
		}

		private static IContainer CreateContainer()
		{
			var containerBuilder = new ContainerBuilder();

			// Note: in a larger project we might want ask AutoFac to make the thesaurus into a single instance.
			containerBuilder
				.RegisterAssemblyTypes(
					Assembly.GetCallingAssembly(),
					typeof(IThesaurus).Assembly)
				.AsImplementedInterfaces()
				.AsSelf();

			return containerBuilder.Build();
		}
	}
}
