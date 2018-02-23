using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileUpdater.Models {
  public class ClientConfig {
    /// <summary>
    /// 当前版本
    /// </summary>
    public string CurrentVersion { get; set; }
    /// <summary>
    /// 服务端地址
    /// </summary>
    public string ServerUrl { get; set; }
    /// <summary>
    /// 主文件
    /// </summary>
    public string MainFile { get; set; }
    /// <summary>
    /// 生成目录(生成服务端用)
    /// </summary>
    public IList<string> GenerateDir { get; set; }
    /// <summary>
    /// 是否生成目录
    /// </summary>
    public bool? Debug { get; set; }
  }
}
