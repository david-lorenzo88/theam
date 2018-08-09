using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Theam.API.Utils
{
    /// <summary>
    /// Helper class to manage files
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// Saves a file in the filesystem and returns the generated fileName
        /// </summary>
        /// <param name="file">File data</param>
        /// <param name="physicalBasePath">physical path where to save the file</param>
        /// <returns></returns>
        public static async Task<string> SaveFile(IFormFile file, string physicalBasePath)
        {
            try
            {
                Directory.CreateDirectory(physicalBasePath);

                var fileName = file.FileName;

                var physicalPath = Path.Combine(physicalBasePath, fileName);
                int i = 0;
                while (System.IO.File.Exists(physicalPath))
                {
                    fileName = string.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), i++, Path.GetExtension(file.FileName));
                    physicalPath = Path.Combine(physicalBasePath, fileName);
                }

                using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Deletes a specified file from the filesystem
        /// </summary>
        /// <param name="physicalPath">the file path</param>
        /// <returns>true if the file was deleted successfully, false otherwise</returns>
        public static bool DeleteFile(string physicalPath)
        {
            try
            {
                System.IO.File.Delete(physicalPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
