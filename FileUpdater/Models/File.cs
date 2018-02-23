using FileUpdater.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileUpdater.Models {
  public class File {
    public string Name { get; set; }
    public string MD5 { get; set; }
    public FileEventEnum Event { get; set; } = FileEventEnum.None;
  }
}
