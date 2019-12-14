using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManagerClient.Commands
{
	public class InstallCommand : ICommand
	{
		public const string kPackEndpoint = "/GetPackageInfo";
		public const string kDepEndpoint = "/GetDependencies";
		public const string kDownEndpoint = "/Download";

		public string[] Aliases { get; } =
		{
			"install"
		};

		public string Description { get; } = "Installs a package.";

		public static async Task<(Package package, bool success)> Install(string package, bool update = false)
		{

			if (Program.InstalledPackages.Any(x => x.PackageName == package) && update == false)
			{
				Logger.WriteLine($"Package {package} Already Installed", ConsoleColor.Cyan);
				return (Program.InstalledPackages.First(x => x.PackageName == package), true);
			}

			if (!Directory.Exists(Environment.CurrentDirectory + Program.InstallationDirectory))
			{
				Logger.WriteLine("Installation directory not found.", ConsoleColor.Red);
				return (default(Package), false);
			}

			Response<Package> response = Client.Get<Package>(kPackEndpoint + "?PackageName=" + package);

			if (response)
			{
				Package info = response.Data;
				PackageInfo Package = null;

				foreach (PackageInfo version in response.Data.Versions)
				{
					if (Package == null || version.VersionID > Package.VersionID)
						Package = version;
				}

				List<Dependency> depends = Package.Dependencies;
				Logger.WriteLine("Getting Dependancies ...", ConsoleColor.Green);
				if (depends != null && depends.Count > 0)
				{
					foreach (Dependency dependency in depends)
					{
						Logger.WriteLine($"Installing dependency {dependency.PackageName}...", ConsoleColor.Yellow);
						if (!(await Install(dependency.PackageName)).success)
						{
							Logger.WriteLine($"Error while installing dependency {dependency}. Aborting installation.", ConsoleColor.Red);
							return (default(Package), false);
						}
					}
				}

				Logger.WriteLine($"Downlading Package {package} ...", ConsoleColor.Cyan);
				try
				{
					Client.Download("http://piebot.xyz/api/smod" + kDownEndpoint + "?PackageName=" + package + "&Version=" + Package.Version, Environment.CurrentDirectory + Program.InstallationDirectory + @"\" + package + ".dll");
				}
				catch(Exception e)
				{
					Logger.WriteLine($"Download Failed: {e.Message}", ConsoleColor.Red);
					return (default(Package), false);
				}
				response.Data.Versions = null;
				response.Data.CurrentVersion = Package;

				if (Program.InstalledPackages.Any(x => x.PackageName == package))
				{
					Package installed = Program.InstalledPackages.First(x => x.PackageName == package);
					Program.InstalledPackages.Remove(installed);
				}
				Program.InstalledPackages.Add(response.Data);

				Program.SaveInstalled();

				return (response.Data, true);
			}
			else
			{
				Logger.WriteLine("Unknown Package!", ConsoleColor.Red);
			}

			Logger.WriteLine("Unknown Package!", ConsoleColor.Red);
			return (default(Package), false);
		}

		public string GetUsage(string alias) => $"{alias} <package>";

		public async Task Execute(string[] args)
		{
			switch (args.Length)
			{
				case 1:
					if (string.IsNullOrEmpty(args[0]) || string.IsNullOrWhiteSpace(args[0]))
					{
						Logger.WriteLine($"Correct Usage: {GetUsage("install")}", ConsoleColor.Red);
						return;
					}

					Logger.WriteLine("Installing...", ConsoleColor.Gray);

					(Package package, bool success) = await Install(args[0]);

					if (success)
					{
						Logger.WriteLine("Package installed successfully." + (package.Readme != null ? "Readme: " : string.Empty), ConsoleColor.Green);

						if (package.Readme != null)
						{
							Program.ReadMarkup(package.Readme);
						}
					}
					else
					{
						Logger.WriteLine("Error Installing Package :(", ConsoleColor.Red);
					}
					break;

				default:
					Logger.WriteLine($"Correct Usage: {GetUsage("install")}", ConsoleColor.Red);
					break;
			}
		}
	}

}
