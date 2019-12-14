using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace PackageManagerClient.Commands
{
	public class AttributeCommand : ICommand
	{
		public string[] Aliases { get; } =
		{
			"attribute"
		};

		public string Description { get; } = "Gets All Attributes From a dll.";

		public string GetUsage(string alias) => alias;

		public Task Execute(string[] args)
		{
			PluginDetails details = Attributes.GetAttributes(args[0]);

			Logger.WriteLine($"Plugin Name: {details.name}");
			Logger.WriteLine($"Plugin Author: {details.author}");
			Logger.WriteLine($"Plugin Id: {details.id}");
			Logger.WriteLine($"Plugin Description: {details.description}");
			Logger.WriteLine($"Plugin Version: {details.version}");
			Logger.WriteLine($"Plugin Smod Version: {details.SmodMajor}.{details.SmodMinor}.{details.SmodRevision}");

			return Task.CompletedTask;
		}
	}
}
