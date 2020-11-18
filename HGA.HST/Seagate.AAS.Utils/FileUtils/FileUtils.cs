using System;
using System.IO;

namespace Seagate.AAS.Utils
{
	/// <summary>
	/// Summary description for File.
	/// </summary>
	public class FileUtils
	{
        public static void RemoveOldFiles(string directory, string fileExtention, int maxRetentionDays)
        {
            // look through directory ...
            foreach (string file in Directory.GetFiles(directory, "*" + fileExtention))
            {
                FileInfo fileInfo = new FileInfo(file);
                System.Diagnostics.Trace.WriteLine( string.Format("Analyzing file: {0}, {1}", fileInfo.Name, fileInfo.LastWriteTime) );
				
                //delete file if match file extention and is older than retention days
                if (fileInfo.Extension.Equals(fileExtention) &&
                    fileInfo.LastWriteTime.AddDays(maxRetentionDays) < DateTime.Now)
                {
                    System.Diagnostics.Trace.WriteLine( string.Format("Remove old file: {0}", file) );
                    File.Delete(file);
                }
            }
        }
	}
}
