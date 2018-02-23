using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FileUpdater.Helpers {
  public static class FileHelper {
    public static string CalculateMD5(string filename) {
      using (var md5 = MD5.Create()) {
        using (var stream = File.OpenRead(filename)) {
          var hash = md5.ComputeHash(stream);
          return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }
      }
    }
    public static void WriteToFile(object obj, string targetPath) {
      JsonSerializerSettings serializerSettings = new JsonSerializerSettings() {
        Converters = { new StringEnumConverter() },
        Formatting = Formatting.Indented
      };

      System.IO.File.WriteAllText(
        targetPath,
        JsonConvert.SerializeObject(
          obj,
          serializerSettings
        )
      );
    }
    public static IList<Models.Directory> GetDirectories(string[] directories, Enum.FileEventEnum fileEvent = Enum.FileEventEnum.None, bool searchSubDirectories = true) {
      List<Models.Directory> result = new List<Models.Directory>();
      if (directories.Any()) {
        foreach (var dir in directories) {
          Models.Directory directory = new Models.Directory();
          directory.DirName = dir;
          directory.Files = new List<Models.File>();
          foreach (var file in System.IO.Directory.GetFiles(dir)) {
            directory.Files.Add(new Models.File() {
              Name = file.Substring(file.LastIndexOf("\\") + 1),
              Event = fileEvent,
              MD5 = FileHelper.CalculateMD5(file)
            });
          }
          result.Add(directory);
          if (searchSubDirectories) {
            result.AddRange(GetDirectories(Directory.GetDirectories(dir), fileEvent));
          }
        }
      }
      return result;
    }
    public static void DeleteFile(Models.File file, string parentPath, bool needSearchParentPath = true) {
      var fullPath = $"{parentPath}{file.Name}";
      if (File.Exists(fullPath)) {
        //存在
        File.Delete(fullPath);
      } else {
        var allFiles = GetDirectories(new string[] { parentPath }, searchSubDirectories: false);
        var sameMD5Files = allFiles[0].Files.Where(c => c.MD5.Equals(file.MD5)).ToList();
        foreach (var sameFile in sameMD5Files) {
          DeleteFile(sameFile, parentPath, false);
        }
      }
    }
  }
}
