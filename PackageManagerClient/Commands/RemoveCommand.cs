using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PackageManagerClient.Commands
{
	public class RemoveCommand : ICommand
	{
		public string[] Aliases { get; } =
		{
			"remove"
		};

		public string Description { get; } = "Safely removes a package.";
		public string GetUsage(string alias) => $"{alias} [package]";

		public Task Execute(string[] args)
		{
			switch (args.Length)
			{
				case 1:
					Package toRemove = null;
					foreach (Package package in Program.InstalledPackages)
					{
						if (package.PackageName == args[0])
						{
							toRemove = package;
						}
					}

					if (toRemove == null)
					{
						Logger.WriteLine("Package not found.");
						return Task.CompletedTask;
					}
					foreach (Package package in Program.InstalledPackages)
					{
						if (package.CurrentVersion.Dependencies != null)
						{
							foreach (Dependency dep in package.CurrentVersion.Dependencies)
							{
								if (dep.PackageName == toRemove.PackageName)
								{
									Logger.WriteLine($"Unable to remove, package is a dependency of {dep.PackageName}.", ConsoleColor.Red);
									return Task.CompletedTask;
								}
							}
						}
					}
					try
					{
						File.Delete(Environment.CurrentDirectory + Program.InstallationDirectory + "/" + toRemove.PackageName + ".dll");
					}
					catch (FileNotFoundException)
					{
						Logger.WriteLine("Package Already Removed?", ConsoleColor.Green);
					}

					Program.InstalledPackages.Remove(toRemove);
					Program.SaveInstalled();

					Logger.WriteLine("Package removed successfully.", ConsoleColor.Green);
					break;

				default:
					Logger.WriteLine("Invalid arguments.", ConsoleColor.Red);
					break;
			}

			return Task.CompletedTask;
		}
	}

}
