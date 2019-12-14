using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PackageManagerClient
{
	public interface ICommand
	{
		string[] Aliases { get; }

		string Description { get; }

		string GetUsage(string alias);

		Task Execute(string[] args);
	}
}
