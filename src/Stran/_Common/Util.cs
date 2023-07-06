using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Stran
{
    /// <summary>
    /// 共通処理を記述します。
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// アプリ名を取得します。
        /// </summary>
        public static string AppName => Assembly.GetExecutingAssembly().GetName().Name!;

        /// <summary>
        /// バージョンを取得します。
        /// </summary>
        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

        /// <summary>
        /// マッチする文字列を抜き出します。
        /// </summary>
        /// <param name="regex">正規表現</param>
        /// <param name="value">入力文字列</param>
        /// <returns>マッチした文字列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/>または<paramref name="value"/>がnull</exception>
        /// <exception cref="RegexMatchTimeoutException">正規表現の検索時にタイムアウトした</exception>
        public static string Extract(this Regex regex, string value)
        {
            ArgumentNullException.ThrowIfNull(regex);

            return regex.Match(value).Value;
        }
    }
}
