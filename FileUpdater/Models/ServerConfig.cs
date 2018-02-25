using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileUpdater.Models {
  public class ServerConfig {
    public string LatestVersion { get; set; }
    public string FileAddress { get; set; }
    public string Remark { get; set; }
    public string CDNAddress { get; set; }
    public IList<Directory> Directories { get; set; }
    public string Icon { get; set; }
  }
}
