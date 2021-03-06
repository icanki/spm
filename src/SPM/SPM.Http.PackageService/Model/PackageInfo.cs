﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPM.Http.PackageService.Model
{
    public class PackageInfo
    {
        public PackageInfo(Package package, string downloadLinkFormat)
        {
            Name = package.Name.Split('@').First();
            Tag = package.Name.Split('@').Last();

            DownloadLink = string.Format(downloadLinkFormat, Name + "@" + Tag, package.FileHash);
        }

        public string Name { get; set; }
        public string Tag { get; set; }
        public string DownloadLink { get; set; }
    }
}
