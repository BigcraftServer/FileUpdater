namespace FileUpdater {
  partial class Updater {
    /// <summary>
    /// 必需的设计器变量。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// 清理所有正在使用的资源。
    /// </summary>
    /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows 窗体设计器生成的代码

    /// <summary>
    /// 设计器支持所需的方法 - 不要修改
    /// 使用代码编辑器修改此方法的内容。
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Updater));
      this.UpdateBtn = new System.Windows.Forms.Button();
      this.LatestVersionLabel = new System.Windows.Forms.Label();
      this.CurrentVersionLabel = new System.Windows.Forms.Label();
      this.lbl_DownloadName = new System.Windows.Forms.Label();
      this.UpdatePgb = new System.Windows.Forms.ProgressBar();
      this.TitleLbl = new System.Windows.Forms.Label();
      this.CloseApplication = new System.Windows.Forms.Label();
      this.HeadBgd = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // UpdateBtn
      // 
      this.UpdateBtn.Location = new System.Drawing.Point(125, 33);
      this.UpdateBtn.Name = "UpdateBtn";
      this.UpdateBtn.Size = new System.Drawing.Size(116, 32);
      this.UpdateBtn.TabIndex = 18;
      this.UpdateBtn.Text = "更新|修复";
      this.UpdateBtn.UseVisualStyleBackColor = true;
      this.UpdateBtn.Click += new System.EventHandler(this.UpdateBtn_Click);
      // 
      // LatestVersionLabel
      // 
      this.LatestVersionLabel.AutoSize = true;
      this.LatestVersionLabel.ForeColor = System.Drawing.Color.PaleGreen;
      this.LatestVersionLabel.Location = new System.Drawing.Point(10, 49);
      this.LatestVersionLabel.Name = "LatestVersionLabel";
      this.LatestVersionLabel.Size = new System.Drawing.Size(83, 12);
      this.LatestVersionLabel.TabIndex = 15;
      this.LatestVersionLabel.Text = "更新检测中...";
      // 
      // CurrentVersionLabel
      // 
      this.CurrentVersionLabel.AutoSize = true;
      this.CurrentVersionLabel.ForeColor = System.Drawing.Color.PaleGreen;
      this.CurrentVersionLabel.Location = new System.Drawing.Point(10, 33);
      this.CurrentVersionLabel.Name = "CurrentVersionLabel";
      this.CurrentVersionLabel.Size = new System.Drawing.Size(53, 12);
      this.CurrentVersionLabel.TabIndex = 16;
      this.CurrentVersionLabel.Text = "当前版本";
      // 
      // lbl_DownloadName
      // 
      this.lbl_DownloadName.AutoSize = true;
      this.lbl_DownloadName.ForeColor = System.Drawing.Color.PaleGreen;
      this.lbl_DownloadName.Location = new System.Drawing.Point(10, 65);
      this.lbl_DownloadName.Name = "lbl_DownloadName";
      this.lbl_DownloadName.Size = new System.Drawing.Size(83, 12);
      this.lbl_DownloadName.TabIndex = 17;
      this.lbl_DownloadName.Text = "更新检测中...";
      this.lbl_DownloadName.Visible = false;
      // 
      // UpdatePgb
      // 
      this.UpdatePgb.Location = new System.Drawing.Point(12, 81);
      this.UpdatePgb.Name = "UpdatePgb";
      this.UpdatePgb.Size = new System.Drawing.Size(228, 15);
      this.UpdatePgb.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.UpdatePgb.TabIndex = 14;
      this.UpdatePgb.Value = 100;
      // 
      // TitleLbl
      // 
      this.TitleLbl.AutoSize = true;
      this.TitleLbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(73)))), ((int)(((byte)(120)))));
      this.TitleLbl.ForeColor = System.Drawing.Color.DarkOrange;
      this.TitleLbl.Location = new System.Drawing.Point(78, 10);
      this.TitleLbl.Name = "TitleLbl";
      this.TitleLbl.Size = new System.Drawing.Size(95, 12);
      this.TitleLbl.TabIndex = 13;
      this.TitleLbl.Text = "BigcraftUpdater";
      this.TitleLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.TitleLbl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeadBgd_MouseDown);
      this.TitleLbl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HeadBgd_MouseMove);
      // 
      // CloseApplication
      // 
      this.CloseApplication.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(73)))), ((int)(((byte)(120)))));
      this.CloseApplication.ForeColor = System.Drawing.Color.Transparent;
      this.CloseApplication.Image = ((System.Drawing.Image)(resources.GetObject("CloseApplication.Image")));
      this.CloseApplication.Location = new System.Drawing.Point(225, 7);
      this.CloseApplication.Name = "CloseApplication";
      this.CloseApplication.Size = new System.Drawing.Size(16, 16);
      this.CloseApplication.TabIndex = 12;
      this.CloseApplication.Click += new System.EventHandler(this.CloseApplication_Click);
      // 
      // HeadBgd
      // 
      this.HeadBgd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(73)))), ((int)(((byte)(120)))));
      this.HeadBgd.Location = new System.Drawing.Point(0, 0);
      this.HeadBgd.Name = "HeadBgd";
      this.HeadBgd.Size = new System.Drawing.Size(250, 27);
      this.HeadBgd.TabIndex = 11;
      this.HeadBgd.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.HeadBgd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeadBgd_MouseDown);
      this.HeadBgd.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HeadBgd_MouseMove);
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(73)))), ((int)(((byte)(120)))));
      this.pictureBox1.Location = new System.Drawing.Point(2, 1);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(25, 25);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBox1.TabIndex = 19;
      this.pictureBox1.TabStop = false;
      // 
      // Updater
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(94)))));
      this.ClientSize = new System.Drawing.Size(250, 105);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.UpdateBtn);
      this.Controls.Add(this.LatestVersionLabel);
      this.Controls.Add(this.CurrentVersionLabel);
      this.Controls.Add(this.lbl_DownloadName);
      this.Controls.Add(this.UpdatePgb);
      this.Controls.Add(this.TitleLbl);
      this.Controls.Add(this.CloseApplication);
      this.Controls.Add(this.HeadBgd);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "Updater";
      this.Text = "客户端更新器";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Updater_FormClosing);
      this.Load += new System.EventHandler(this.Updater_Load);
      this.Shown += new System.EventHandler(this.Updater_Shown);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button UpdateBtn;
    private System.Windows.Forms.Label LatestVersionLabel;
    private System.Windows.Forms.Label CurrentVersionLabel;
    private System.Windows.Forms.Label lbl_DownloadName;
    private System.Windows.Forms.ProgressBar UpdatePgb;
    private System.Windows.Forms.Label TitleLbl;
    private System.Windows.Forms.Label CloseApplication;
    private System.Windows.Forms.Label HeadBgd;
    private System.Windows.Forms.PictureBox pictureBox1;
  }
}

