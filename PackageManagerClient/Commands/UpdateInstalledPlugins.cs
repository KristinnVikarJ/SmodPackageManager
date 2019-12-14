using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PackageManagerClient.Commands
{
    class UpdateInstalledPlugins : ICommand
	{
		public const string kPluginEndpoint = "/GetPluginById";
		public string[] Aliases { get; } =
		{
			"refresh",
			"refreshplugins"
		};

		public string Description { get; } = "Refreshes the list of installed plugins.";

		public string GetUsage(string alias) => alias;

		public Task Execute(string[] args)
		{
			int Added = 0;
			foreach (string file in Directory.GetFiles(Environment.CurrentDirectory + Program.InstallationDirectory))
			{
				bool Installed = false;
				PluginDetails info = null;
				try
				{
					info = Attributes.GetAttributes(file);
					foreach(Package package in Program.InstalledPackages)
					{
						if(package.PackageId == info.id)
						{
							Installed = true;
						}
					}
				}
				catch(Exception e)
				{
					Logger.WriteLine($"Unable to Load Plugin {Path.GetFileName(file)}, missing dependencies ? Error: {e.Message}", ConsoleColor.Red);
				}
				if (!Installed && info != null)
				{
					Response<Package> response = Client.Get<Package>(kPluginEndpoint + "?PluginId=" + info.id);
					if (response)
					{
						Added++;
						PackageInfo Current = null;
						foreach(PackageInfo version in response.Data.Versions)
						{
							if (version.Version == info.version)
							{
								Current = version;
							}
						}
						if(Current != null)
						{
							response.Data.CurrentVersion = Current;
							Logger.WriteLine($"[+] Found Plugin {info.name}, Adding to installed!.");
						}
						else
						{
							Logger.WriteLine($"Version {info.version} not found for plugin {info.name}, assuming newest version.");
							response.Data.CurrentVersion = response.Data.GetNewestVersion();
						}
						Program.InstalledPackages.Add(response.Data);
						Program.SaveInstalled();
					}
				}
			}
			if(Added == 0)
			{
				Logger.WriteLine("Nothing to Add!", ConsoleColor.Green);
			}
			return Task.CompletedTask;
		}
	}
}
