using FileUpdater.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FileUpdater.Helpers {
  public static class HttpHelper {
    public static T GetJsonFile<T>(Uri uri, int retryCount = 0) where T : class, new() {
      T result;
      using (WebClient DownloadClient = new WebClient()) {
        //去除缓存
        DownloadClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);
        try {
          Byte[] pageData = DownloadClient.DownloadData(uri);
          result = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(pageData));
        } catch (Exception ex) {
          //转换失败,重试
          if (retryCount < 3)
            result = GetJsonFile<T>(uri, ++retryCount);
          else
            throw ex;
        }
      }
      return result;
    }

    ///// <summary>        
    ///// c#,.net 下载文件        
    ///// </summary>        
    ///// <param name="URL">下载文件地址</param>       
    ///// <param name="filename">下载后的存放地址</param>        
    ///// <param name="prog">用于显示的进度条</param>        
    ///// 
    public static void DownloadFile(ServerConfig serverConfig, Models.File file, string parentPath, System.Windows.Forms.ProgressBar prog, System.Windows.Forms.Label label1) {
      string fullpath = $"{parentPath}{file.Name}";
      Uri uri = new Uri($"{serverConfig.CDNAddress}{fullpath}");
      float percent = 0;
      if (!System.IO.Directory.Exists(parentPath)) {
        System.IO.Directory.CreateDirectory(parentPath);
      }
      try {
        HttpWebRequest Myrq = (HttpWebRequest)WebRequest.Create(uri);
        using (HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse()) {
          long totalBytes = myrp.ContentLength;
          if (prog != null) {
            prog.Maximum = (int)totalBytes;
          }
          using (Stream st = myrp.GetResponseStream()) {
            using (Stream so = new FileStream(fullpath, FileMode.Create)) {
              long totalDownloadedByte = 0;
              byte[] by = new byte[1024];
              int osize = st.Read(by, 0, (int)by.Length);
              label1.Text = fullpath.Substring(fullpath.LastIndexOf("\\") + 1);
              while (osize > 0) {
                totalDownloadedByte = osize + totalDownloadedByte;
                System.Windows.Forms.Application.DoEvents();
                so.Write(by, 0, osize);
                if (prog != null) {
                  prog.Value = (int)totalDownloadedByte;
                }
                osize = st.Read(by, 0, (int)by.Length);

                percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
              }
            }
          }
        }
      } catch (Exception) {
        throw;
      }
    }
  }
}
