using System;
using System.IO;

namespace Stran.TableGenerator
{
    /// <summary>
    /// C#のコード出力ライターのクラスです。
    /// </summary>
    internal class CsWriter : IDisposable
    {
        private int indentLevel;
        private TextWriter writer;

        /// <summary>
        /// <see cref="CsWriter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="writer">出力に使用する<see cref="TextWriter"/></param>のインスタンス
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>が<see langword="null"/></exception>
        public CsWriter(TextWriter writer)
        {
            ArgumentNullException.ThrowIfNull(writer);
            this.writer = writer;
        }

        /// <inheritdoc/>
        public void Dispose() => writer.Dispose();

        /// <summary>
        /// ブロックを開始します。
        /// </summary>
        /// <param name="header">ブロック名</param>
        public void BeginBlock(string header)
        {
            WriteLine(header);
            WriteLine('{');
            indentLevel++;
        }

        /// <summary>
        /// ブロックを終了します。
        /// </summary>
        /// <param name="end">終了時に追加出力する文字列</param>
        public void EndBlock(string? end = null)
        {
            indentLevel--;
            WriteLine($"}}{end}");
        }

        /// <inheritdoc cref="TextWriter.WriteLine()"/>
        public void WriteLine() => writer.WriteLine();

        /// <inheritdoc cref="TextWriter.WriteLine(string?)"/>
        public void WriteLine(char value)
        {
            if (indentLevel > 0) writer.Write(new string(' ', indentLevel * 4));
            writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(string?)"/>
        public void WriteLine(string? value)
        {
            if (indentLevel > 0) writer.Write(new string(' ', indentLevel * 4));
            writer.WriteLine(value);
        }
    }
}
