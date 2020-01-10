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
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			for (var x = 0; x < types.Length; x++)
			{
				foreach (Object a in types[x].GetCustomAttributes(false))
				{
					if (a.GetType().Name == "PluginDetails")
					{
						foreach (FieldInfo info in a.GetType().GetFields())
						{
							if (info.GetValue(a) != null)
							{
								attributes.Add(info.Name, info.GetValue(a).ToString());
							}
						}
					}
				}
			}

			return new PluginDetails()
			{
				SmodMajor = int.Parse(attributes["SmodMajor"]),
				SmodMinor = int.Parse(attributes["SmodMinor"]),
				SmodRevision = int.Parse(attributes["SmodRevision"]),
				author = attributes.ContainsKey("author") ? attributes["author"] : "None",
				configPrefix = attributes.ContainsKey("configPrefix") ? attributes["configPrefix"] : "",
				description = attributes.ContainsKey("description") ? attributes["description"] : "",
				id = attributes["id"],
				langFile = attributes.ContainsKey("langFile") ? attributes["langFile"] : "",
				name = attributes["name"],
				version = attributes["version"]
			};
		}
	}
}
