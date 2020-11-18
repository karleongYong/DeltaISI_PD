using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Drawing;

using System.Runtime.InteropServices;

namespace Seagate.AAS.HGA.HST.Utils
{
    public static class CommonTools
    {
        public enum UserTypes { Monitor = 0, Operator, Engineer, Administrator, Seagate }        

        /// <summary>
        /// Convert at DateTime to a formatted Timestamp; 00/00/00 HH:MM:SS.mmm
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDateToTimeStamp(DateTime dt)
        {
            return dt.ToShortDateString() + "," + string.Format("{0:00}:{1:00}:{2:00}.{3:000}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }

       
        /// <summary>
        /// Get a number at the end of a string.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static int GetNumberAtEnd(string inputString)
        {
            try
            {
                return Convert.ToInt16((Regex.Match(inputString, @"\d+").Value));
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Basic routine for finding if a file is available
        /// </summary>
        /// <param name="file"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        public static bool CheckFileReady(string file, int retries)
        {
            int retryCount = 0;

            while (retryCount < retries)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        using (Stream s = new FileStream(file, FileMode.Open))
                        {
                        }
                        return true;
                    }
                    else
                    {
                        retryCount++;
                        System.Threading.Thread.Sleep(1);
                    }
                }
                catch
                {
                    retryCount++;
                    System.Threading.Thread.Sleep(1);
                }
            }

            return false;
        }


        #region RSA Encryption and Decryption Routines
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToEncrypt"></param>
        /// <returns></returns>
        public static string Encrypt(string ToEncrypt)
        {
            return encrypt(ToEncrypt);
        }

        public static List<Tuple<string, string, string>> ReadAccessControlFile(string fileName)
        {
            return readAccessControlFile(fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cypherString"></param>
        /// <returns></returns>
        public static string Decrypt(string cypherString)
        {
            return decrypt(cypherString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToEncrypt"></param>
        /// <returns></returns>
        private static string encrypt(string ToEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(ToEncrypt);
            string Key = "XyratexOSC12345678901234";    // 24 Character - Don't change the number of characters!
            keyArray = UTF8Encoding.UTF8.GetBytes(Key);
            
            TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider();
            tDes.Key = keyArray;
            tDes.Mode = CipherMode.ECB;
            tDes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tDes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tDes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cypherString"></param>
        /// <returns></returns>
        private static string decrypt(string cypherString)
        {
            byte[] keyArray;
            byte[] toDecryptArray = Convert.FromBase64String(cypherString);
            string key = "XyratexOSC12345678901234";
            keyArray = UTF8Encoding.UTF8.GetBytes(key);
            
            TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider();
            tDes.Key = keyArray;
            tDes.Mode = CipherMode.ECB;
            tDes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tDes.CreateDecryptor();
            
            try
            {
                byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
                tDes.Clear();
            
                return UTF8Encoding.UTF8.GetString(resultArray, 0, resultArray.Length);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Read the encrypted file and return List
        /// </summary>
        private static List<Tuple<string, string, string>> readAccessControlFile(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            List<Tuple<string, string, string>> _decrytedList = new List<Tuple<string,string,string>>();

            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    // There is only one line in this file
                    sb.Append(sr.ReadLine());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
       
            try
            {
                string[] fileArrayEncryted = sb.ToString().Split(':');

                foreach (string es in fileArrayEncryted)
                {
                    // the string is still encrypted   user,password,type
                    string[] esArray = es.Split(',');
                    if (esArray.Length == 3)
                    {
                        _decrytedList.Add(new Tuple<string, string, string>(CommonTools.Decrypt(esArray[0]), CommonTools.Decrypt(esArray[1]), CommonTools.Decrypt(esArray[2])));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _decrytedList;
        }
        #endregion
    }
}
