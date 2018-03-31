using FileUpdater.Helpers;
using FileUpdater.Models;
using FileUpdater.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileUpdater {
  public partial class Updater : Form {
    //Environment.GetCommandLineArgs()?[0]
    private UpdaterCore updaterCore = UpdaterCore.Instance();
    public Updater() {
      InitializeComponent();
      // 允许随便更改控件
      Control.CheckForIllegalCrossThreadCalls = false;
    }

    #region 窗体事件
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
      CheckUpdate(true);
    }
    /// <summary>
    /// 窗体加载事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Updater_Load(object sender, EventArgs e) {
      updaterCore.LoadClientConfig().GenerateDebugFolder();
      //展示本地版本
      this.CurrentVersionLabel.Text = updaterCore.clientConfig.CurrentVersion;
    }
    /// <summary>
    /// 窗体显示事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Updater_Shown(object sender, EventArgs e) {
      updaterCore.LoadServerConfig().LoadClientDirectories().InitIcon();
      DisplayIcon();

      CheckUpdate();
    }
    /// <summary>
    /// 关闭事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseApplication_Click(object sender, EventArgs e) {
      Application.Exit();
    }
    /// <summary>
    /// 窗体关闭事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Updater_FormClosing(object sender, FormClosingEventArgs e) {
      updaterCore.SaveClientConfig();
    }
    #endregion

    #region 提示框简单类
    private static void ShowMessageBox(string showText, bool? isError = null) {
      if (!isError.HasValue) {
        MessageBox.Show(showText, "警告", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
      } else {
        if (isError.Value) {
          MessageBox.Show(showText, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } else {
          MessageBox.Show(showText, "提示", MessageBoxButtons.OK);
        }
      }
    }
    #endregion

    #region 更新检测
    private void CheckUpdate(bool forceUpdate = false) {
      (bool needUpdate, IList<(Models.File, string)> deleteFiles, IList<(Models.File, string)> downloadFiles) = updaterCore.VersionCheck(forceUpdate);
      DownloadFileName.Visible = true;
      DownloadFileName.ForeColor = Color.DarkOrange;
      DownloadFileName.Text = "检查中...";
      LatestVersionLabel.Text = updaterCore.serverConfig.LatestVersion;
      if (needUpdate) {
        LatestVersionLabel.ForeColor = Color.Red;
        if (MessageBox.Show($"发现客户端新版本:{updaterCore.serverConfig.LatestVersion}\n更新内容:\n{updaterCore.serverConfig.Remark}\n是否更新?", "发现新版本", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
          Upgrading(deleteFiles, downloadFiles);
          LatestVersionLabel.ForeColor = Color.PaleGreen;
        } else {
          UpdateCanceled();
        }
      } else {
        UpdateSuccessful(true);
      }
    }
    private void UpdateCanceled() {
      DownloadFileName.ForeColor = Color.Red;
      DownloadFileName.Text = "用户拒绝更新,已取消";
    }
    private void UpdateSuccessful(bool noUpdated = false) {
      if (!noUpdated) {
        updaterCore.clientConfig.CurrentVersion = updaterCore.serverConfig.LatestVersion;
        this.CurrentVersionLabel.Text = updaterCore.clientConfig.CurrentVersion;
        DownloadFileName.Text = $"完毕,可以愉悦的启动游戏了";
        DownloadFileName.ForeColor = Color.PaleGreen;
        LatestVersionLabel.ForeColor = Color.PaleGreen;
        ShowMessageBox("更新完成!", false);
      } else {
        DownloadFileName.Text = $"已经是最新版本了";
        DownloadFileName.ForeColor = Color.PaleGreen;
        LatestVersionLabel.ForeColor = Color.PaleGreen;
      }
    }
    private void Upgrading(IList<(Models.File, string)> deleteFiles, IList<(Models.File, string)> downloadFiles) {
      IList<Task> deleteTasks = deleteFiles.Select(file => {
        return new Task(() => {
          this.DownloadFileName.Text = $"删除{file.Item1.Name}中...";
          FileHelper.Delete(file.Item1, file.Item2);
        });
      }).ToList();
      IList<Task> downloadTasks = downloadFiles.Select(file => {
        return new Task(() => {
          this.DownloadFileName.Text = $"下载{file.Item1.Name}中...";
          HttpHelper.DownloadFile(updaterCore.serverConfig, file.Item1, file.Item2, this.UpdatePgb);
        });
      }).ToList();

      foreach (var deleteTask in deleteTasks) {
        deleteTask.RunSynchronously();
      }
      foreach (var downloadTask in downloadTasks) {
        downloadTask.RunSynchronously();
      }
      UpdateSuccessful();
    }
    #endregion
    #region 图标
    private void DisplayIcon() {
      this.Icon = updaterCore.Icon.Value;
      this.HeadIcon.Image = updaterCore.Icon.Value.ToBitmap();
    }
    #endregion
  }
}
