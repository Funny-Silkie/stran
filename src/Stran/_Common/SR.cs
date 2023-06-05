using CuiLib.Log;

namespace Stran
{
    /// <summary>
    /// 静的リソースを管理します。
    /// </summary>
    internal static class SR
    {
        /// <summary>
        /// 標準出力へリダイレクトされているロガーを取得します。
        /// </summary>
        public static Logger StdOut { get; }

        /// <summary>
        /// 標準エラーへリダイレクトされているロガーを取得します。
        /// </summary>
        public static Logger StdErr { get; }

        static SR()
        {
            StdOut = new Logger()
            {
                ConsoleStdoutLogEnabled = true,
            };
            StdErr = new Logger()
            {
                ConsoleErrorEnabled = true,
            };
        }
    }
}
