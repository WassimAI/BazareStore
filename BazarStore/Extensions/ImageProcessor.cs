using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace BazarStore.Extensions
{
    public static class ImageProcessor
    {
        public static bool SaveImage(int id, HttpPostedFileBase file, string folderName)
        {
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", HttpContext.Current.Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), folderName);
            var pathString2 = Path.Combine(originalDirectory.ToString(), folderName + "\\" + id.ToString());

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (file != null && file.ContentLength > 0)
            {
                // Get file extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    return false;
                }

                // Set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, file.FileName);

                // Save original
                file.SaveAs(path);

                #region AdditionalFolders
                //Only for Products

                var pathString3 = Path.Combine(originalDirectory.ToString(), folderName +"\\" + id.ToString() + "\\Thumbs");
                var pathString4 = Path.Combine(originalDirectory.ToString(), folderName + "\\" + id.ToString() + "\\Gallery");
                var pathString5 = Path.Combine(originalDirectory.ToString(), folderName + "\\" + id.ToString() + "\\Gallery\\Thumbs");

                if (!Directory.Exists(pathString3))
                    Directory.CreateDirectory(pathString3);

                if (!Directory.Exists(pathString4))
                    Directory.CreateDirectory(pathString4);

                if (!Directory.Exists(pathString5))
                    Directory.CreateDirectory(pathString5);

                var path2 = string.Format("{0}\\{1}", pathString3, file.FileName);
                // Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(150, 150);
                img.Save(path2);

                #endregion
            }

            return true;
        }

        public static void DeleteImage(int id, string folderName)
        {
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", HttpContext.Current.Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), folderName + "\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);
        }
    }
}