using CuiLib.Log;
using System;

namespace Stran
{
    /// <summary>
    /// ロガーの拡張を記述します。
    /// </summary>
    internal static class LoggerExtension
    {
        /// <summary>
        /// 色付きで出力を行います。
        /// </summary>
        /// <param name="logger">使用するロガー</param>
        /// <param name="message">出力内容</param>
        /// <param name="foreground">文字色</param>
        /// <param name="background">背景色</param>
        public static void WriteWithColor(this Logger logger, string? message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            OperateWithColor(logger.Write, message, foreground, background);
        }

        /// <summary>
        /// 色付きで出力を行います。
        /// </summary>
        /// <param name="logger">使用するロガー</param>
        /// <param name="message">出力内容</param>
        /// <param name="foreground">文字色</param>
        /// <param name="background">背景色</param>
        public static void WriteWithColor(this Logger logger, ReadOnlySpan<char> message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            WriteWithColor(logger, message.ToString(), foreground, background);
        }

        /// <summary>
        /// 色付きで出力を行います。
        /// </summary>
        /// <param name="logger">使用するロガー</param>
        /// <param name="message">出力内容</param>
        /// <param name="foreground">文字色</param>
        /// <param name="background">背景色</param>
        public static void WriteWithColor(this Logger logger, object? message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            OperateWithColor(logger.Write, message, foreground, background);
        }

        /// <summary>
        /// 色付きで出力を行います。
        /// </summary>
        /// <param name="logger">使用するロガー</param>
        /// <param name="message">出力内容</param>
        /// <param name="foreground">文字色</param>
        /// <param name="background">背景色</param>
        public static void WriteLineWithColor(this Logger logger, string? message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            OperateWithColor(logger.WriteLine, message, foreground, background);
        }

        /// <summary>
        /// 色付きで出力を行います。
        /// </summary>
        /// <param name="logger">使用するロガー</param>
        /// <param name="message">出力内容</param>
        /// <param name="foreground">文字色</param>
        /// <param name="background">背景色</param>
        public static void WriteLineWithColor(this Logger logger, ReadOnlySpan<char> message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            WriteLineWithColor(logger, message.ToString(), foreground, background);
        }

        /// <summary>
        /// 色付きで出力を行います。
        /// </summary>
        /// <param name="logger">使用するロガー</param>
        /// <param name="message">出力内容</param>
        /// <param name="foreground">文字色</param>
        /// <param name="background">背景色</param>
        public static void WriteLineWithColor(this Logger logger, object? message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            OperateWithColor(logger.WriteLine, message, foreground, background);
        }

        /// <summary>
        /// 色付きでログメソッドを実行します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="value">処理に使う引数</param>
        /// <param name="action">ログ処理</param>
        /// <param name="foreground">文字色</param>
        /// <param name="background">背景色</param>
        private static void OperateWithColor<T>(Action<T> action, T value, ConsoleColor? foreground, ConsoleColor? background)
        {
            ConsoleColor bgColor = Console.BackgroundColor;
            ConsoleColor fgColor = Console.ForegroundColor;
            Console.BackgroundColor = background ?? bgColor;
            Console.ForegroundColor = foreground ?? fgColor;
            action.Invoke(value);
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
        }

        /// <summary>
        /// エラーを出力します。
        /// </summary>
        /// <param name="logger">使用するロガー</param>
        /// <param name="message">エラーメッセージ</param>
        public static void WriteError(this Logger logger, string? message)
        {
            logger.WriteLineWithColor(message, ConsoleColor.Red);
        }

        /// <summary>
        /// エラーを出力します。
        /// </summary>
        /// <param name="logger">使用するロガー</param>
        /// <param name="exception">例外</param>
        public static void WriteError(this Logger logger, Exception? exception)
        {
            if (exception is null) return;
            logger.WriteLineWithColor(exception, ConsoleColor.Red);
        }

        /// <summary>
        /// 警告を出力します。
        /// </summary>
        /// <param name="logger">使用するロガー</param>
        /// <param name="message">警告メッセージ</param>
        public static void WriteWarning(this Logger logger, string? message)
        {
            logger.WriteLineWithColor(message, ConsoleColor.Yellow);
        }
    }
}
