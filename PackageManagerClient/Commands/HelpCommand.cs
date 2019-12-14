using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PackageManagerClient.Commands
{
	public class HelpCommand : ICommand
	{
		public string[] Aliases { get; } =
		{
			"help",
			"commands",
			"cmds"
		};

		public string Description { get; } = "Lists all commands and their usages/descriptions.";

		public string GetUsage(string alias) => alias + " [command]";

		private void WriteHelpInfo(ICommand command)
		{
			Logger.WriteLine("- " + command.Aliases[0] + (command.Aliases.Length > 1 ? $" (a.k.a. {string.Join(", ", command.Aliases.Skip(1))})" : ""), ConsoleColor.Cyan);
			Logger.WriteLine("  Usage: " + command.GetUsage(command.Aliases[0]));
			Logger.WriteLine("  Description: " + command.Description);
		}

		public Task Execute(string[] args)
		{
			switch (args.Length)
			{
				case 0:
					{
						foreach (ICommand command in Program.Commands)
						{
							WriteHelpInfo(command);
						}
					}
					break;

				case 1:
					{
						ICommand command = Program.Commands.FirstOrDefault(x => x.Aliases.Contains(args[0]));
						if (command != null)
						{
							WriteHelpInfo(command);
						}
						else
						{
							Logger.WriteLine("Command not found.", ConsoleColor.Red);
						}
					}
					break;

				default:
					Logger.WriteLine($"Correct Usage: {GetUsage(Aliases[0])}", ConsoleColor.Red);
					break;
			}

			return Task.CompletedTask;
		}
	}

}
