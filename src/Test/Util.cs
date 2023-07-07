using System.IO;

namespace Test
{
    /// <summary>
    /// 共通処理を記述します。
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// テスト用データのファイルパスを取得します。
        /// </summary>
        /// <param name="filename">ファイル名</param>
        /// <returns>テスト用データのファイルパス</returns>
        public static string GetDataFilePath(string filename) => Path.Combine(SR.SourceDir, filename);
    }
}
