using System;
using System.Threading;
using System.Windows.Forms;

namespace FileUpdater {
  static class Program {
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      Application.ThreadException += ApplicationThreadException;
      AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

      Application.Run(new Updater());
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
      var message = $"{((Exception)e.ExceptionObject).Message}\r\n{((Exception)e.ExceptionObject).StackTrace}\r\n请联系管理员或重试";
      MessageBox.Show(message, "程序出错");
    }

    private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e) {
      var message = $"{e.Exception.Message}\r\n{e.Exception.StackTrace}\r\n请联系管理员或重试";
      MessageBox.Show(message, "程序出错");
    }
  }
}
