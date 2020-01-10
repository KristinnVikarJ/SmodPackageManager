using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace PackageManagerClient.Commands
{
	public class PackageCommand : ICommand
	{
		public string[] Aliases { get; } =
		{
			"package"
		};

		public string Description { get; } = "Creates a Package from Plugin dll.";

		public string GetUsage(string alias) => $"{alias} [plugin_dll]";

		public Task Execute(string[] args)
		{
			try
			{
				PluginDetails details = Attributes.GetAttributes(args[0]);

				PackageInfo info = new PackageInfo() { 
					Version = details.version,
					PublishDate = DateTime.Now,
					VersionID = 1,
					Changelog = "",
				};

				Package pack = new Package()
				{
					Author = details.author,
					Description = details.description,
					Name = details.name,
					PackageName = Path.GetFileNameWithoutExtension(args[0]).ToLower(),
					PackageId = details.id,
					Versions = new List<PackageInfo>() { info }
				};

				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(args[0]).ToLower());
				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(args[0]).ToLower() + "/Versions");

				Logger.WriteLine($"Plugin Name: {details.name}");
				Logger.WriteLine($"Plugin Author: {details.author}");
				Logger.WriteLine($"Plugin Id: {details.id}");
				Logger.WriteLine($"Plugin Description: {details.description}");
				Logger.WriteLine($"Plugin Version: {details.version}");
				Logger.WriteLine($"Plugin Smod Version: {details.SmodMajor}.{details.SmodMinor}.{details.SmodRevision}");

				StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "/" + Path.GetFileNameWithoutExtension(args[0]).ToLower() + ".json");
				sw.WriteLine(JsonConvert.SerializeObject(pack, Formatting.Indented));
				sw.Flush();
				sw.Close();
			}
			catch
			{
				Logger.WriteLine("Could Not Find File!", ConsoleColor.Red);
			}


			return Task.CompletedTask;
		}
	}
}
