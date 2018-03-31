using FileUpdater.Helpers;
using FileUpdater.Models;
using FileUpdater.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileUpdater {
  public static class UpdaterCoreExtensions {
    #region ClientConfig
    /// <summary>
    /// 加载客户端配置文件
    /// </summary>
    public static UpdaterCore LoadClientConfig(this UpdaterCore updaterCore, bool isFixed = false) {
      string fileName = updaterCore.clientConfigPath;
      CheckAndGenerateClinetConfig(fileName, false);
      string clientConfigFromFile = FileHelper.ReadFromFile(fileName);
      try {
        updaterCore.clientConfig = JsonConvert.DeserializeObject<ClientConfig>(clientConfigFromFile);
      } catch (Exception) {
        if (!isFixed) {
          FixClientConfig(fileName);
          return updaterCore.LoadClientConfig(true);
        }
        throw;
      }
      return updaterCore;
    }
    /// <summary>
    /// 加载客户端文件列表
    /// </summary>
    public static UpdaterCore LoadClientDirectories(this UpdaterCore updaterCore) {
      updaterCore.clientDirectories = FileHelper.GetDirectories(updaterCore.serverConfig.Directories.Select(c => c.DirName).ToArray()).GroupBy(c => c.DirName).Select(c => c.First()).ToList();
      if (updaterCore.clientConfig.Debug.HasValue && updaterCore.clientConfig.Debug.Value)
        updaterCore.WriteFileForDebug();
      return updaterCore;
    }
    public static UpdaterCore UpdateClientConfig(this UpdaterCore updaterCore) {
      updaterCore.clientConfig.CurrentVersion = updaterCore.serverConfig.LatestVersion;
      updaterCore.clientConfig.Debug = false;
      updaterCore.clientConfig.Icon = updaterCore.serverConfig.Icon;
      return updaterCore;
    }
    public static UpdaterCore SaveClientConfig(this UpdaterCore updaterCore) {
      updaterCore.clientConfig.Debug = false;
      FileHelper.WriteToFile(updaterCore.clientConfig, updaterCore.clientConfigPath);
      return updaterCore;
    }
    private static void WriteFileForDebug(this UpdaterCore updaterCore) {
      FileHelper.WriteToFile(FileHelper.GetDirectories(updaterCore.clientConfig.GenerateDir.ToArray(), Enum.FileEventEnum.Add), "./serverfiles.json");
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
    /// 加载服务端地址
    /// </summary>
    public static UpdaterCore LoadServerConfig(this UpdaterCore updaterCore) {
      try {
        updaterCore.serverConfig = HttpHelper.GetJsonFile<ServerConfig>(updaterCore.serverUri.Value);
        updaterCore.LoadServerDirectories();
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
    public static UpdaterCore InitIcon(this UpdaterCore updaterCore, bool forceUpdate = false) {
      if (forceUpdate
        || (string.IsNullOrEmpty(updaterCore.clientConfig.Icon) && !string.IsNullOrEmpty(updaterCore.serverConfig.Icon))
        || (!string.IsNullOrEmpty(updaterCore.serverConfig.Icon) && !updaterCore.clientConfig.Icon.Equals(updaterCore.serverConfig.Icon))) {
        bool isAbsolutePath = Uri.TryCreate(updaterCore.serverConfig.Icon, UriKind.Absolute, out Uri uri);
        byte[] iconBytes;
        if (isAbsolutePath) {
          iconBytes = HttpHelper.GetBytes(uri);
        } else {
          bool isRelativePath = Uri.TryCreate($"{updaterCore.clientConfig.ServerUrl}{updaterCore.serverConfig.Icon}", UriKind.Absolute, out uri);
          if (isRelativePath) {
            iconBytes = HttpHelper.GetBytes(uri);
          } else {
            throw new Exception($"去它妈的什么路径?\n{updaterCore.clientConfig.ServerUrl}{updaterCore.serverConfig.Icon}");
          }
        }
        updaterCore.clientConfig.Icon = updaterCore.serverConfig.Icon;
        updaterCore.clientConfig.Base64Icon = Convert.ToBase64String(iconBytes);
      }
      return updaterCore;
    }
    /// <summary>
    /// 加载服务端文件列表
    /// </summary>
    /// <param name="serverFileAddress"></param>
    private static void LoadServerDirectories(this UpdaterCore updaterCore) {
      string serverFileAddress = updaterCore.serverConfig.FileAddress;
      bool isAbsolutePath = Uri.TryCreate(serverFileAddress, UriKind.Absolute, out Uri uri);
      IList<Models.Directory> directories = null;
      if (isAbsolutePath) {
        directories = HttpHelper.GetJsonFile<List<Models.Directory>>(uri);
      } else {
        bool isRelativePath = Uri.TryCreate(updaterCore.clientConfig.ServerUrl + serverFileAddress, UriKind.Absolute, out uri);
        if (isRelativePath) {
          directories = HttpHelper.GetJsonFile<List<Models.Directory>>(uri);
        } else {
          throw new Exception($"去它妈的什么路径?\n{serverFileAddress}");
        }
      }
      updaterCore.serverConfig.Directories = directories;
    }
    #endregion
    #region Core
    /// <summary>
    /// 获取相对应文件夹
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<(string, IList<Models.File>, IList<Models.File>)> GetReleativeDirectories(this UpdaterCore updaterCore) {
      var releativeDirs =
        from serverDir in updaterCore.serverConfig.Directories
        join clientDir in updaterCore.clientDirectories on serverDir.DirName equals clientDir.DirName
        select (serverDir.DirName, serverDir.Files, clientDir.Files);
      return releativeDirs;
    }
    public static (bool, IList<(Models.File, string)>, IList<(Models.File, string)>) VersionCheck(this UpdaterCore updaterCore, bool forceUpdate) {
      (bool, IList<(Models.File, string)>, IList<(Models.File, string)>) result = (false, null, null);
      if (forceUpdate || updaterCore.clientConfig.CurrentVersion != updaterCore.serverConfig.LatestVersion) {
        //需要处理
        result.Item1 = true;
        IList<(Models.File, string)> deleteFiles = result.Item2 = new List<(Models.File, string)>();
        IList<(Models.File, string)> downloadFiles = result.Item3 = new List<(Models.File, string)>();
        foreach (var releativeDirectory in updaterCore.GetReleativeDirectories()) {
          foreach (var file in releativeDirectory.Item2) {
            switch (file.Event) {
              case Enum.FileEventEnum.Add:
                bool fileExists = FileHelper.CheckFileExists(file, releativeDirectory.Item1);
                if (!fileExists) {
                  deleteFiles.Add((file, releativeDirectory.Item1));
                  downloadFiles.Add((file, releativeDirectory.Item1));
                }
                break;
              case Enum.FileEventEnum.Delete:
                deleteFiles.Add((file, releativeDirectory.Item1));
                break;
            }
          }
        }
      }
      return result;
    }
    #endregion
    #region Debug
    public static UpdaterCore GenerateDebugFolder(this UpdaterCore updaterCore, string targetPath = "./serverfiles.json") {
      if (updaterCore.clientConfig.Debug.HasValue && updaterCore.clientConfig.Debug.Value)
        FileHelper.WriteToFile(FileHelper.GetDirectories(updaterCore.clientConfig.GenerateDir.ToArray(), Enum.FileEventEnum.Add), targetPath);
      return updaterCore;
    }
    #endregion
  }
}
