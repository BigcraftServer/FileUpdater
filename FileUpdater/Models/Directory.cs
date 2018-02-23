using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileUpdater.Models {
  public class Directory {
    public string DirName { get; set; }
    public IList<File> Files { get; set; }
  }
}
