using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace PackageManagerClient
{
    class Program
	{
		public const string kInstalledFile = "installed.json";
		public const string kSettingsFile = "settings.json";

		public static List<Package> InstalledPackages;
		//public static Settings Settings { get; }

		public static Dictionary<string, ICommand> commandMap;
		public static List<ICommand> Commands;

		public static string InstallationDirectory;
        static void Main(string[] args)
        {
			InstallationDirectory = "/sm_plugins";

			commandMap = new Dictionary<string, ICommand>();
			Commands = new List<ICommand>();
			if (File.Exists(Environment.CurrentDirectory + "/" + kInstalledFile))
			{
				InstalledPackages = JsonConvert.DeserializeObject<List<Package>>(File.ReadAllText(Environment.CurrentDirectory + "/" + kInstalledFile));
			}
			else
			{
				InstalledPackages = new List<Package>();
			}


			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!typeof(ICommand).IsAssignableFrom(type) || type == typeof(ICommand)) continue;

				ICommand instance = (ICommand)Activator.CreateInstance(type);
				Commands.Add(instance);
				foreach (string alias in instance.Aliases)
				{
					commandMap.Add(alias, instance);
				}
			}

			if(args.Length == 0)
			{
				while (true)
				{
					Console.Write("> ");
					args = Console.ReadLine().Split(' ');
					if (commandMap.ContainsKey(args[0].ToLower()))
					{
						commandMap[args[0].ToLower()].Execute(args.ToList().Skip(1).ToArray());
					}
					else
					{
						Logger.WriteLine("Unknown Command!", ConsoleColor.Red);
					}
				}
			}
		}

		public static void ReadMarkup(string text)
		{
			Logger.SilentWriteLine("Reading markup...");
			ConsoleColor prevColor = Console.ForegroundColor;

			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '$' && (i == 0 || text[i - 1] != '\\') && i + 1 < text.Length)
				{
					int colorLength = 0;
					while (char.IsDigit(text[++i])) colorLength++;

					if (int.TryParse(text.Substring(i - colorLength, colorLength), out int colorCode))
					{
						Console.ForegroundColor = (ConsoleColor)colorCode;
					}
				}
				else
				{
					Console.Write(text[i]);
				}
			}

			Console.ForegroundColor = prevColor;
			Console.WriteLine();
		}


		public static void SaveInstalled()
		{
			File.WriteAllText(kInstalledFile, JsonConvert.SerializeObject(InstalledPackages, Formatting.Indented));
		}

		/*
		public static void SaveSettings()
		{
			File.WriteAllText(kSettingsFile, JsonConvert.SerializeObject(Settings, Formatting.Indented));
		}
		*/
	}
}
