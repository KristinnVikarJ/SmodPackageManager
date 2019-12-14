using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManagerClient.Commands
{
	class UpdateCommand : ICommand
	{

		public string[] Aliases { get; } =
		{
			"update"
		};

		public string Description { get; } = "Updates a Package.";

		public string GetUsage(string alias) => alias + " <package>";

		public async Task Execute(string[] args)
		{
			switch (args.Length)
			{
				case 1:
					if (string.IsNullOrEmpty(args[0]) || string.IsNullOrWhiteSpace(args[0]))
					{
						Logger.WriteLine($"Correct Usage: {GetUsage("update")}", ConsoleColor.Red);
						return;
					}
					string package = args[0].ToLower();

					if (!Program.InstalledPackages.Any(x => x.PackageName == package))
					{
						Logger.WriteLine($"Plugin Not Installed!", ConsoleColor.Red);
					}

					Package currentInstalledPackage = Program.InstalledPackages.First(x => x.PackageName == package);

					Response<Package> response = Client.Get<Package>(InstallCommand.kPackEndpoint + "?PackageName=" + package);

					if (response)
					{
						if(currentInstalledPackage.CurrentVersion.VersionID == response.Data.GetNewestVersion().VersionID)
						{
							Logger.WriteLine($"Already Newest Version!", ConsoleColor.Red);
							return;
						}

						(Package installedPackage, bool success) = await InstallCommand.Install(package, true);
						if (!success)
						{
							Logger.WriteLine($"Error while upgrading package {package}.", ConsoleColor.Red);
							return;
						}
						if (installedPackage.Readme != null)
						{
							Program.ReadMarkup(installedPackage.Readme);
						}


					}
					else
					{
						Logger.WriteLine("Unknown Package!", ConsoleColor.Red);
					}

					break;

				default:
					Logger.WriteLine($"Correct Usage: {GetUsage(Aliases[0])}", ConsoleColor.Red);
					break;
			}
		}
	}
}
