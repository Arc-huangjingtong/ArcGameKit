using System;
using System.Text.RegularExpressions;

namespace HaiPackage
{
    public static class RegularExpressionsTool
    {
        /// <summary>
        /// 是否是一组数字
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool IsNumber(string message)
        {
            return Regex.IsMatch(message, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// 至少有一个数字
        /// @"[ <see langword="minNum"/> - <see langword="maxNum"/> ]+"
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="minNum">最小数字 这个数字不能小于0</param>
        /// <param name="maxNum">最大数字 这个数字不能大于9</param>
        /// <returns>结果</returns>
        public static bool Number(string message, byte minNum = 0, byte maxNum = 9)
        {
            if (maxNum > 9) maxNum = 9;
            if (minNum >= maxNum) minNum = maxNum;
            return Regex.IsMatch(message, @"[" + minNum + "-" + maxNum + "]+");
        }

        /// <summary>
        /// 至少有一个大写字母
        /// @"[A-Z]+" 
        /// </summary>
        /// <param name="message">信息</param>
        /// <returns>结果</returns>
        public static bool Letter(string message)
        {
            return Regex.IsMatch(message, @"[A-Z]+");
        }

        /// <summary>
        /// 至少有一个小写字母
        /// @"[a-z]+"
        /// </summary>
        /// <param name="message">信息</param>
        /// <returns>结果</returns>
        public static bool Lowercase(string message)
        {
            return Regex.IsMatch(message, @"[a-z]+");
        }

        /// <summary>
        /// 长度 @"^.{minNum,maxNum}$"
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="minNum">最小密码长度</param>
        /// <param name="maxNum">最大密码长度</param>
        /// <returns>结果</returns>
        public static bool Length(string message, int minNum = 8, int maxNum = 16)
        {
            if (minNum >= maxNum) minNum = maxNum;
            return Regex.IsMatch(message, @"^.{" + minNum + "," + maxNum + "}$");
        }

        /// <summary>
        /// 长度 @"^.{minNum,maxNum}$"
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="minNum">最小密码长度</param>
        /// <param name="maxNum">最大密码长度</param>
        /// <returns>结果</returns>
        public static bool Length(string message, byte minNum = 8, byte maxNum = 16)
        {
            if (minNum >= maxNum) minNum = maxNum;
            return Regex.IsMatch(message, @"^.{" + minNum + "," + maxNum + "}$");
        }

        #region 密码
        /// <summary>
        /// 一个正常的密码
        /// 一个只有数字和字母的密码
        /// </summary>
        /// <param name="message"></param>
        /// <param name="minNum">最小密码长度</param>
        /// <param name="maxNum">最大密码长度</param>
        /// <returns></returns>
        public static bool NormalPassword(string message, uint minNum = 0, uint maxNum = int.MaxValue)
        {
            return Regex.IsMatch(message, @"^\w{" + minNum + "," + maxNum + "}$");
        }

        /// <summary>
        /// 可以自定义一些条件的密码
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rules"></param>
        /// <param name="minNum">最小密码长度</param>
        /// <param name="maxNum">最大密码长度</param>
        /// <returns></returns>
        public static bool PasswordPro(string message, PasswordRules rules, uint minNum = 0, uint maxNum = int.MaxValue)
        {
            var pattern = rules switch
            {
                PasswordRules.DigitalPassword => @"^ \d{" + minNum + "," + maxNum + "}$",
                PasswordRules.Lowercase => @"^ [a-z]{" + minNum + "," + maxNum + "}$",
                PasswordRules.Capital => @"^ [A-Z]{" + minNum + "," + maxNum + "}$",
                PasswordRules.Letter => @"^ [a-zA-Z]{" + minNum + "," + maxNum + "}$",
                PasswordRules.NumberPlusLetter => @"^ [0-9a-zA-Z]{" + minNum + "," + maxNum + "}$",
                PasswordRules.Level1 => "^(?=.*[0-9]+)[0-9A-Za-z]{" + minNum + "," + maxNum + "}$",
                PasswordRules.Level2 => "^(?=.*[0-9]+)(?=.*[a-zA-Z]+)[0-9A-Za-z]{" + minNum + "," + maxNum + "}$",
                PasswordRules.Level3 => "^(?=.*[0-9]+)(?=.*[a-z]+)(?=.*[A-Z]+)[0-9A-Za-z]{" + minNum + "," + maxNum + "}$",
                PasswordRules.Level4 => "^(?=.*[0-9]+)(?=.*[a-z]+)(?=.*[A-Z]+)(?=.*[$@$!%*#?&]+)[0-9A-Za-z$@$!%*#?&]{" +
                                        minNum + "," + maxNum + "}$",
                _ => ""
            };
            return Regex.IsMatch(message, pattern);
        }

        public enum PasswordRules
        {
            /// <summary>
            /// 只有数字
            /// </summary>
            DigitalPassword,
            /// <summary>
            /// 只有小写字母
            /// </summary>
            Lowercase,
            /// <summary>
            /// 只有大写字母
            /// </summary>
            Capital,
            /// <summary>
            /// 只有字母
            /// </summary>
            Letter,
            /// <summary>
            /// 数字加字母
            /// </summary>
            NumberPlusLetter,
            /// <summary>
            /// 必须要有一个数字
            /// </summary>
            Level1,
            /// <summary>
            /// 必须有一个数字，一个字母
            /// </summary>
            Level2,
            /// <summary>
            /// 必须有一个数字，一个小写字母，一个大写字母
            /// </summary>
            Level3,
            /// <summary>
            /// 必须有一个数字，一个小写字母，一个大写字母，一个特殊字符
            /// </summary>
            Level4
        }
        #endregion
    }
}