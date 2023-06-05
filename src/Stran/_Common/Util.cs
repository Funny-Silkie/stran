using System;
using System.Reflection;

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
    }
}
