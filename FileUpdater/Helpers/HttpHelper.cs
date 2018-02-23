using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FileUpdater.Helpers {
  public static class HttpHelper {
    public static T GetJsonFile<T>(Uri uri, int retryCount = 0) where T : class, new() {
      T result;
      using (WebClient DownloadClient = new WebClient()) {
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
  }
}
