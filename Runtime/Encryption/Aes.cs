using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace HaiPackage.Tool
{
    public class Aes
    {
        private static string _aesHead = "AESEncrypt";

        /// <summary>
        /// 文件加密，传入文件路径
        /// </summary>
        public static void AesFileEncrypt(string path, string encryptKey)
        {
            if (!File.Exists(path))
                return;
            try
            {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var headBuff = new byte[10];
                    fs.Read(headBuff, 0, 10);
                    var headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag == _aesHead)
                    {
#if UNITY_EDITOR
                        Debug.Log(path + "已经加密过了！");
#endif
                        return;
                    }

                    fs.Seek(0, SeekOrigin.Begin);
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0);
                    var headBuffer = Encoding.UTF8.GetBytes(_aesHead);
                    fs.Write(headBuffer, 0, 10);
                    var encBuffer = AesEncrypt(buffer, encryptKey);
                    fs.Write(encBuffer, 0, encBuffer.Length);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 文件解密，传入文件路径  (改动了加密文件，不合适运行时)
        /// </summary>
        public static void AesFileDecrypt(string path, string encryptKey)
        {
            if (!File.Exists(path))
                return;
            try
            {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var headBuff = new byte[10];
                    fs.Read(headBuff, 0, headBuff.Length);
                    var headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag != _aesHead) return;
                    var buffer = new byte[fs.Length - headBuff.Length];
                    fs.Read(buffer, 0, Convert.ToInt32(fs.Length - headBuff.Length));
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0);
                    var encBuffer = AesDecrypt(buffer, encryptKey);
                    fs.Write(encBuffer, 0, encBuffer.Length);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 文件解密，传入文件路径,返回字节
        /// </summary>
        public static byte[] AesFileByteDecrypt(string path, string encryptKey)
        {
            if (!File.Exists(path))
                return null;
            byte[] encBuffer = null;
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    {
                        var headBuff = new byte[10];
                        fs.Read(headBuff, 0, headBuff.Length);
                        var headTag = Encoding.UTF8.GetString(headBuff);
                        if (headTag == _aesHead)
                        {
                            var buffer = new byte[fs.Length - headBuff.Length];
                            fs.Read(buffer, 0, Convert.ToInt32(fs.Length - headBuff.Length));
                            encBuffer = AesDecrypt(buffer, encryptKey);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }

            return encBuffer;
        }

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="encryptString">待加密密文</param>
        /// <param name="encryptKey">加密密钥</param>
        // public static string AesEncrypt(string encryptString, string encryptKey)
        // {
        //     return Convert.ToBase64String(AesEncrypt(Encoding.Default.GetBytes(encryptString), encryptKey));
        // }

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="encryptByte">待加密密文</param>
        /// <param name="encryptKey">加密密钥</param>
        public static byte[] AesEncrypt(byte[] encryptByte, string encryptKey)
        {
            if (encryptByte.Length == 0)
            {
                throw (new Exception("明文不得为空"));
            }

            if (string.IsNullOrEmpty(encryptKey))
            {
                throw (new Exception("密钥不得为空"));
            }

            byte[] m_strEncrypt;
            var m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
            var m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
            var m_AESProvider = Rijndael.Create();
            try
            {
                MemoryStream m_stream = new MemoryStream();
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(encryptKey, m_salt);
                ICryptoTransform transform = m_AESProvider.CreateEncryptor(pdb.GetBytes(32), m_btIV);
                CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
                m_csstream.Write(encryptByte, 0, encryptByte.Length);
                m_csstream.FlushFinalBlock();
                m_strEncrypt = m_stream.ToArray();
                m_stream.Close();
                m_stream.Dispose();
                m_csstream.Close();
                m_csstream.Dispose();
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                m_AESProvider.Clear();
            }

            return m_strEncrypt;
        }
        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="decryptString">待解密密文</param>
        /// <param name="decryptKey">解密密钥</param>
        // public static string AesDecrypt(string decryptString, string decryptKey)
        // {
        //     return Convert.ToBase64String(AesDecrypt(Encoding.Default.GetBytes(decryptString), decryptKey));
        // }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="decryptByte">待解密密文</param>
        /// <param name="decryptKey">解密密钥</param>
        public static byte[] AesDecrypt(byte[] decryptByte, string decryptKey)
        {
            if (decryptByte.Length == 0)
            {
                throw (new Exception("密文不得为空"));
            }

            if (string.IsNullOrEmpty(decryptKey))
            {
                throw (new Exception("密钥不得为空"));
            }

            byte[] m_strDecrypt;
            var m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
            var m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
            var m_AESProvider = Rijndael.Create();
            try
            {
                MemoryStream m_stream = new MemoryStream();
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(decryptKey, m_salt);
                ICryptoTransform transform = m_AESProvider.CreateDecryptor(pdb.GetBytes(32), m_btIV);
                CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
                m_csstream.Write(decryptByte, 0, decryptByte.Length);
                m_csstream.FlushFinalBlock();
                m_strDecrypt = m_stream.ToArray();
                m_stream.Close();
                m_stream.Dispose();
                m_csstream.Close();
                m_csstream.Dispose();
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                m_AESProvider.Clear();
            }

            return m_strDecrypt;
        }
    }
}