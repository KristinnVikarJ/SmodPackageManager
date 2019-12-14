using System;
using System.Collections.Generic;
using System.Text;

namespace PackageManagerClient
{
    public class Package
    {
        public PackageInfo GetNewestVersion()
        {
            PackageInfo pack = null;
            foreach (PackageInfo version in Versions)
            {
                if (pack == null || version.VersionID > pack.VersionID)
                    pack = version;
            }
            return pack;
        }


        public string Name { get; set; } // Full Name of Package, not PackageName

        public string PackageName { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Readme { get; set; }

        public PackageInfo CurrentVersion { get; set; }

        public List<PackageInfo> Versions { get; set; }
    }
    public class Dependency
    {
        public string PackageName { get; set; }
        public string Version { get; set; }
    }

    public class PackageInfo
    {
        public DateTime PublishDate { get; set; }

        public string Changelog { get; set; }

        public string Version { get; set; }

        public int VersionID { get; set; }

        public List<Dependency> Dependencies { get; set; }

        //public string FileName { get; set; }
    }
}
