using System.Security.Cryptography;
using System.Text;

namespace FlowingDefault.Core.Utils
{
    public static class HashUtils
    {
        /// <summary>
        /// Generate MD5 hash for a string
        /// </summary>
        /// <param name="input">String to hash</param>
        /// <returns>MD5 hash as string</returns>
        public static string GenerateMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
} 