using FileUpdater.Helpers;
using FileUpdater.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FileUpdater {
  public partial class Updater : Form {
    private ClientConfig clientConfig;
    private ServerConfig serverConfig;
    private IList<Models.Directory> clientDirectories;
    public Updater() {
      InitializeComponent();
    }

    #region 移动窗口
    private Point mPoint = new Point();
    private void HeadBgd_MouseDown(object sender, MouseEventArgs e) {
      mPoint.X = e.X;
      mPoint.Y = e.Y;
    }
    private void HeadBgd_MouseMove(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        Point myPosittion = MousePosition;
        myPosittion.Offset(-mPoint.X, -mPoint.Y);
        Location = myPosittion;
      }
    }
    #endregion
    /// <summary>
    /// 更新按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateBtn_Click(object sender, EventArgs e) {
      //JsonConvert.SerializeObject(new object());
      //NeedDeleteFiles();
      NeedDownloadFiles();
    }
    /// <summary>
    /// 窗体加载事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Updater_Load(object sender, EventArgs e) {
      LoadClientConfig();
      LoadServerConfig();
      if (clientConfig.Debug.HasValue && clientConfig.Debug.Value)
        GenerateLocalFiles();
    }
    /// <summary>
    /// 关闭事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseApplication_Click(object sender, EventArgs e) {
      Application.Exit();
    }

    #region 加载Config
    /// <summary>
    /// 加载客户端Config
    /// </summary>
    /// <param name="path"></param>
    private void LoadClientConfig(string path = "./updater.json") {
      using (StreamReader fs = new StreamReader(path, Encoding.UTF8)) {
        this.clientConfig = JsonConvert.DeserializeObject<ClientConfig>(fs.ReadToEnd());
      }
    }
    private void LoadClientDirectories() {
      this.clientDirectories = FileHelper.GetDirectories(this.serverConfig.Directories.Select(c => c.DirName).ToArray());
    }
    /// <summary>
    /// 加载服务端Config
    /// </summary>
    private void LoadServerConfig() {
      Uri serverConfigUri = new Uri($"{this.clientConfig.ServerUrl}{this.clientConfig.MainFile}?r={Guid.NewGuid().ToString()}");
      this.serverConfig = HttpHelper.GetJsonFile<ServerConfig>(serverConfigUri);
      LoadServerDirectories();
    }
    /// <summary>
    /// 加载服务端Files文件
    /// </summary>
    private void LoadServerDirectories() {
      bool isAbsolutePath = Uri.TryCreate(serverConfig.FileAddress, UriKind.Absolute, out Uri uri);
      IList<Models.Directory> directories = null;
      if (isAbsolutePath) {
        directories = HttpHelper.GetJsonFile<List<Models.Directory>>(uri);
      } else {
        bool isRelativePath = Uri.TryCreate($"{clientConfig.ServerUrl}{serverConfig.FileAddress}", UriKind.Absolute, out uri);
        if (isRelativePath) {
          directories = HttpHelper.GetJsonFile<List<Models.Directory>>(uri);
        } else {
          throw new Exception("去它妈的什么路径");
        }
      }
      this.serverConfig.Directories = directories;
      LoadClientDirectories();
    }
    #endregion

    #region 读取本地数据生成Json
    private void GenerateLocalFiles() {
      FileHelper.WriteToFile(FileHelper.GetDirectories(clientConfig.GenerateDir.ToArray(), Enum.FileEventEnum.Add), "./serverfiles.json");
    }
    #endregion

    #region 对比差异
    /// <summary>
    /// 获取目录相对关系 方便使用
    /// </summary>
    /// <returns></returns>
    private IEnumerable<Tuple<string, IList<Models.File>, IList<Models.File>>> GetReleativeDirectories() {
      var releativeDirs =
        from serverDir in serverConfig.Directories
        join clientDir in this.clientDirectories on serverDir.DirName equals clientDir.DirName
        select new Tuple<string, IList<Models.File>, IList<Models.File>>(serverDir.DirName, serverDir.Files, clientDir.Files);
      return releativeDirs;
    }
    private void Upgrading() {
      foreach (var releativeDirectory in GetReleativeDirectories()) {
        foreach (var file in releativeDirectory.Item2) {
          switch (file.Event) {
            case Enum.FileEventEnum.Add:
              bool fileExists = FileHelper.CheckFileExists(file, releativeDirectory.Item1);
              if (!fileExists)
                HttpHelper.DownloadFile(serverConfig, file, releativeDirectory.Item1, this.UpdatePgb, this.lbl_DownloadName);
              //Download
              break;
            case Enum.FileEventEnum.Delete:
              FileHelper.Delete(file, releativeDirectory.Item1);
              //Delete
              break;
          }
        }
      }
    }
    private IList<Models.Directory> NeedDeleteFiles() {
      IList<Models.Directory> result = new List<Models.Directory>();
      foreach (var releativeDir in GetReleativeDirectories()) {
        var deleteFiles =
          (from server in releativeDir.Item2
           join client in releativeDir.Item3 on server.Name equals client.Name
           where server.Event == Enum.FileEventEnum.Delete
           select client).ToList();
        deleteFiles.AddRange(
          (from server in releativeDir.Item2
           join client in releativeDir.Item3 on server.MD5 equals client.MD5
           where server.Event == Enum.FileEventEnum.Delete
           select client).ToList());
        if (deleteFiles.Any()) {
          result.Add(new Models.Directory() {
            DirName = releativeDir.Item1,
            Files = deleteFiles.Distinct().ToList()
          });
        }
      }
      return result;
    }
    private IList<Models.Directory> NeedDownloadFiles() {
      IList<Models.Directory> result = new List<Models.Directory>();
      foreach (var releativeDir in GetReleativeDirectories()) {
        var downloadFiles =
          (from server in releativeDir.Item2
           where server.Event == Enum.FileEventEnum.Add && releativeDir.Item3.FirstOrDefault(c => c.Name == server.Name && c.MD5 == server.MD5) == null
           select server).ToList();
        if (downloadFiles.Any()) {
          result.Add(new Models.Directory() {
            DirName = releativeDir.Item1,
            Files = downloadFiles.Distinct().ToList()
          });
        }
      }
      return result;
    }
    #endregion
  }
}
