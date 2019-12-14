using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PackageManagerClient
{
    class Attributes
    {
        public static PluginDetails GetAttributes(string plugin)
        {
			Assembly ass = Assembly.LoadFrom(plugin);

			Type[] types = ass.GetTypes();
			Dictionary<string, string> Attributes = new Dictionary<string, string>();
			for (var x = 0; x < types.Length; x++)
			{
				var attributes = types[x].GetCustomAttributes(false);
				foreach (Object a in attributes)
				{
					if (a.GetType().Name == "PluginDetails")
					{
						foreach (FieldInfo info in a.GetType().GetFields())
						{
							Attributes.Add(info.Name, info.GetValue(a).ToString());
						}
					}
				}
			}
			return new PluginDetails()
			{
				SmodMajor = int.Parse(Attributes["SmodMajor"]),
				SmodMinor = int.Parse(Attributes["SmodMinor"]),
				SmodRevision = int.Parse(Attributes["SmodRevision"]),
				author = Attributes["author"],
				configPrefix = Attributes["configPrefix"],
				description = Attributes["description"],
				id = Attributes["id"],
				langFile = Attributes["langFile"],
				name = Attributes["name"],
				version = Attributes["version"]
			};
		}
	}
}
