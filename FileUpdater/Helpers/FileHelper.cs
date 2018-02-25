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
        using (var stream = System.IO.File.OpenRead(filename)) {
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
    public static IList<Models.Directory> GetDirectories(string[] directories, Enum.FileEventEnum fileEvent = Enum.FileEventEnum.None) {
      List<Models.Directory> result = new List<Models.Directory>();
      if (directories.Any()) {
        foreach (var dir in directories) {

          if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
          }

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
          var subDirectories = System.IO.Directory.GetDirectories(dir);
          if (subDirectories.Any()) {
            subDirectories = subDirectories.Select(c => c += "\\").ToArray();
            result.AddRange(GetDirectories(subDirectories, fileEvent));
          }
        }
      }
      return result;
    }
    public static Models.Directory GetDirectory(string dir, Enum.FileEventEnum fileEvent = Enum.FileEventEnum.None) {
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
      return directory;
    }
    public static void Delete(Models.File file, string parentPath) {
      var fullPath = $"{parentPath}{file.Name}";
      if (File.Exists(fullPath)) {
        //存在
        File.Delete(fullPath);
      } else {
        var directory = GetDirectory(parentPath);
        var sameFiles = directory.Files.Where(c => c.MD5.Equals(file.MD5)).ToList();
        foreach (var sameFile in sameFiles) {
          Delete(sameFile, parentPath);
        }
      }
    }
    public static bool CheckFileExists(Models.File file, string parentPath) {
      bool result = false;
      var fullPath = $"{parentPath}{file.Name}";
      if (File.Exists(fullPath) && CalculateMD5(fullPath).Equals(file.MD5)) {
        result = true;
      } else {
        result = false;
      }
      return result;
    }
  }
}
