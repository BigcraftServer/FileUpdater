using FileUpdater.Helpers;
using FileUpdater.Models;
using FileUpdater.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FileUpdater {
  public class UpdaterCore {
    private static UpdaterCore instance;
    private UpdaterCore(string path) {
      this.clientConfigPath = path;
    }
    public static UpdaterCore Instance(string path = "./config.json") {
      if (instance == null) {
        instance = new UpdaterCore(path);
      }
      return instance;
    }

    #region properties
    /// <summary>
    /// 客户端配置文件地址(相对)
    /// </summary>
    public string clientConfigPath;
    /// <summary>
    /// 服务端Uri
    /// </summary>
    public Lazy<Uri> serverUri {
      get {
        return new Lazy<Uri>(() => {
          return new Uri($"{clientConfig.ServerUrl}{clientConfig.MainFile}?r={Guid.NewGuid().ToString()}");
        });
      }
    }
    public Lazy<Icon> Icon {
      get {
        return new Lazy<Icon>(() => {
          Icon icon = null;
          using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(clientConfig.Base64Icon))) {
            icon = new Icon(ms);
          }
          return icon;
        });
      }
    }
    /// <summary>
    /// 客户端配置文件
    /// </summary>
    public ClientConfig clientConfig;
    /// <summary>
    /// 服务端配置文件
    /// </summary>
    public ServerConfig serverConfig;
    /// <summary>
    /// 客户端文件列表
    /// </summary>
    public IList<Models.Directory> clientDirectories;
    #endregion
  }
}
