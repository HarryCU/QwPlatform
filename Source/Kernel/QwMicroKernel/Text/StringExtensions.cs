using System;
using System.Text;

namespace QwMicroKernel.Text
{
    public static class StringExtensions
    {
        /// <summary>
        /// 将汉字转换为Unicode
        /// </summary>
        /// <param name="text">要转换的字符串</param>
        /// <returns></returns>
        public static string ToUnicode(this string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            string lowCode = string.Empty, temp = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i % 2 == 0)
                {
                    //取出元素4编码内容（两位16进制）
                    temp = Convert.ToString(bytes[i], 16);
                    if (temp.Length < 2) temp = "0" + temp;
                }
                else
                {
                    //取出元素4编码内容（两位16进制）
                    string tString = Convert.ToString(bytes[i], 16);
                    if (tString.Length < 2) tString = "0" + tString;
                    lowCode = lowCode + @"\u" + tString + temp;
                }
            }
            return lowCode;
        }
    }
}