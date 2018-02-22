using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FileUpdater {
  public partial class Updater : Form {
    public Updater() {
      InitializeComponent();
    }

    private void lbl_CloseApplication_Click(object sender, EventArgs e) {
      Application.Exit();
    }
    #region 移动窗口
    private Point mPoint = new Point();
    private void HeadBgd_MouseDown(object sender, MouseEventArgs e) {
      mPoint.X = e.X;
      mPoint.Y = e.Y;
    }
    #endregion

    private void HeadBgd_MouseMove(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        Point myPosittion = MousePosition;
        myPosittion.Offset(-mPoint.X, -mPoint.Y);
        Location = myPosittion;
      }
    }
    
    private void UpdateBtn_Click(object sender, EventArgs e) {
      Newtonsoft.Json.JsonConvert.SerializeObject(new object());
    }
  }
}
