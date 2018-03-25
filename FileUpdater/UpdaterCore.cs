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
    private static UpdaterCore updaterCore;
    public static UpdaterCore GetUpdaterCore() {
      if (updaterCore == null)
        updaterCore = new UpdaterCore();
      return updaterCore;
    }
    /// <summary>
    /// 图标
    /// </summary>
    public static Lazy<Icon> Icon {
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
    public static ClientConfig clientConfig;
    /// <summary>
    /// 服务端配置文件
    /// </summary>
    public static ServerConfig serverConfig;
    public static string clientConfigPath;
    /// <summary>
    /// 客户端文件列表
    /// </summary>
    public static IList<Models.Directory> clientDirectories;
    #region ClientConfig
    /// <summary>
    /// 加载客户端配置文件
    /// </summary>
    public static UpdaterCore LoadClientConfig(string fileName = "updater.json", bool isFixed = false) {
      clientConfigPath = fileName;
      CheckAndGenerateClinetConfig(fileName, false);
      string clientConfigFromFile = FileHelper.ReadFromFile(fileName);
      try {
        clientConfig = JsonConvert.DeserializeObject<ClientConfig>(clientConfigFromFile);
      } catch (Exception) {
        if (!isFixed) {
          FixClientConfig(fileName);
          return LoadClientConfig(fileName, true);
        }
        throw;
      }
      return updaterCore;
    }
    /// <summary>
    /// 加载客户端文件列表
    /// </summary>
    public static UpdaterCore LoadClientDirectories() {
      clientDirectories = FileHelper.GetDirectories(serverConfig.Directories.Select(c => c.DirName).ToArray()).GroupBy(c => c.DirName).Select(c => c.First()).ToList();
      if (clientConfig.Debug.HasValue && clientConfig.Debug.Value)
        WriteFileForDebug();
      return updaterCore;
    }
    public static UpdaterCore UpdateClientConfig() {
      clientConfig.CurrentVersion = serverConfig.LatestVersion;
      clientConfig.Debug = false;
      clientConfig.Icon = serverConfig.Icon;
      return updaterCore;
    }
    public static UpdaterCore SaveClientConfig() {
      FileHelper.WriteToFile(clientConfig, clientConfigPath);
      return updaterCore;
    }
    private static void WriteFileForDebug() {
      FileHelper.WriteToFile(FileHelper.GetDirectories(clientConfig.GenerateDir.ToArray(), Enum.FileEventEnum.Add), "./serverfiles.json");
    }
    /// <summary>
    /// 修复客户端
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static void FixClientConfig(string fileName) {
      var breakedFileName = BackupClientConfig(fileName);
      CheckAndGenerateClinetConfig(fileName);
    }
    /// <summary>
    /// 生成默认配置文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="forceGenerate"></param>
    private static void CheckAndGenerateClinetConfig(string fileName, bool forceGenerate = false) {
      if (!System.IO.File.Exists(fileName) || forceGenerate) {
        ClientConfig defaultSettings = JsonConvert.DeserializeObject<ClientConfig>(Resources.DefaultSettings);
        FileHelper.WriteToFile(defaultSettings, fileName);
      }
    }
    /// <summary>
    /// 备份配置文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static string BackupClientConfig(string fileName) {
      var newFileName = Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.ToString("yyyyMMddhhmmss") + "_bak" + Path.GetExtension(fileName);
      System.IO.File.Move(fileName, newFileName);
      return newFileName;
    }
    #endregion
    #region ServerConfig
    /// <summary>
    /// 服务端Uri
    /// </summary>
    private static Lazy<Uri> serverUri {
      get {
        return new Lazy<Uri>(() => {
          return new Uri($"{clientConfig.ServerUrl}{clientConfig.MainFile}?r={Guid.NewGuid().ToString()}");
        });
      }
    }
    /// <summary>
    /// 加载服务端地址
    /// </summary>
    public static UpdaterCore LoadServerConfig() {
      try {
        serverConfig = HttpHelper.GetJsonFile<ServerConfig>(serverUri.Value);
        LoadServerDirectories();
      } catch (Exception) {
        throw;
      }
      return updaterCore;
    }
    /// <summary>
    /// 加载图标
    /// </summary>
    /// <param name="forceUpdate"></param>
    /// <returns></returns>
    public static UpdaterCore InitIcon(bool forceUpdate = false) {
      if (forceUpdate || (!string.IsNullOrEmpty(serverConfig.Icon) && !clientConfig.Icon.Equals(serverConfig.Icon))) {
        bool isAbsolutePath = Uri.TryCreate(serverConfig.Icon, UriKind.Absolute, out Uri uri);
        byte[] iconBytes;
        if (isAbsolutePath) {
          iconBytes = HttpHelper.GetBytes(uri);
        } else {
          bool isRelativePath = Uri.TryCreate($"{clientConfig.ServerUrl}{serverConfig.Icon}", UriKind.Absolute, out uri);
          if (isRelativePath) {
            iconBytes = HttpHelper.GetBytes(uri);
          } else {
            throw new Exception($"去它妈的什么路径?\n{clientConfig.ServerUrl}{serverConfig.Icon}");
          }
        }
        clientConfig.Base64Icon = Convert.ToBase64String(iconBytes);
      }
      return updaterCore;
    }
    /// <summary>
    /// 加载服务端文件列表
    /// </summary>
    /// <param name="serverFileAddress"></param>
    private static void LoadServerDirectories() {
      string serverFileAddress = serverConfig.FileAddress;
      bool isAbsolutePath = Uri.TryCreate(serverFileAddress, UriKind.Absolute, out Uri uri);
      IList<Models.Directory> directories = null;
      if (isAbsolutePath) {
        directories = HttpHelper.GetJsonFile<List<Models.Directory>>(uri);
      } else {
        bool isRelativePath = Uri.TryCreate(clientConfig.ServerUrl + serverFileAddress, UriKind.Absolute, out uri);
        if (isRelativePath) {
          directories = HttpHelper.GetJsonFile<List<Models.Directory>>(uri);
        } else {
          throw new Exception($"去它妈的什么路径?\n{serverFileAddress}");
        }
      }
      serverConfig.Directories = directories;
    }
    #endregion
    #region Core
    /// <summary>
    /// 获取相对应文件夹
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<Tuple<string, IList<Models.File>, IList<Models.File>>> GetReleativeDirectories() {
      var releativeDirs =
        from serverDir in serverConfig.Directories
        join clientDir in clientDirectories on serverDir.DirName equals clientDir.DirName
        select new Tuple<string, IList<Models.File>, IList<Models.File>>(serverDir.DirName, serverDir.Files, clientDir.Files);
      return releativeDirs;
    }
    #endregion
  }
}
