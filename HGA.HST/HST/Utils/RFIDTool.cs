using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Seagate.AAS.HGA.HST.Utils
{
    public class RFIDTool
    {
        public string filePath;

        //[DllImport("kernel32")]
        //private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public RFIDTool(string filePath)
        {
            this.filePath = filePath;
        }

        
        public string FileReadStringValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, filePath);
            return temp.ToString();
        }

        public int FileReadIntegerValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, filePath);
            string TempString = temp.ToString();
            if (TempString == "")
            {
                return 0;
            }
            else
            {
                return Int32.Parse(TempString);
            }
        }
    }
}
