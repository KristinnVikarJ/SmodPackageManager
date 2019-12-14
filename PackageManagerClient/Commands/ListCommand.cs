using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PackageManagerClient.Commands
{
	public class ListCommand : ICommand
	{
		public string[] Aliases { get; } =
		{
			"ls",
			"list",
			"plugins",
			"installed"
		};

		public string Description { get; } = "Lists all the packages in the local package list.";

		public string GetUsage(string alias) => alias;

		public Task Execute(string[] args)
		{
			string infoData = File.ReadAllText(Program.kInstalledFile);

			Package[] infos;
			try
			{
				infos = JsonConvert.DeserializeObject<Package[]>(infoData);
			}
			catch
			{
				Logger.WriteLine($"Error while parsing {Program.kInstalledFile}.", ConsoleColor.Red);
				return Task.CompletedTask;
			}

			foreach (Package info in infos)
			{
				Logger.WriteLine($"{info.Name} - {info.PackageName}, Version: {info.CurrentVersion.Version}", ConsoleColor.Cyan);
			}

			return Task.CompletedTask;
		}
	}
}
