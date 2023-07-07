using System;
using System.IO;

namespace Stran.Logics
{
    /// <summary>
    /// ORF情報を表します。
    /// </summary>
    /// <param name="Offset">翻訳時の読み枠のオフセットを取得します。</param>
    /// <param name="StartCodon">読み始めコドンを取得します。</param>
    /// <param name="StartIndex">核酸配列におけるORF開始インデックスを取得します。</param>
    /// <param name="EndCodon">読み終わりコドンを取得します。</param>
    /// <param name="EndIndex">核酸配列におけるORF終了インデックスを取得します。</param>
    /// <param name="State">状態を取得します。</param>
    /// <param name="Sequence">アミノ酸配列データを取得します。</param>
    [Serializable]
    public record class OrfInfo(int Offset, Triplet StartCodon, int StartIndex, Triplet EndCodon, int EndIndex, OrfState State, ReadOnlyMemory<AminoAcid> Sequence)
    {
        /// <summary>
        /// 配列長を取得します。
        /// </summary>
        public int Length => Sequence.Length;

        /// <summary>
        /// <see cref="OrfInfo"/>の新しいインスタンスを初期化します。
        /// </summary>
        public OrfInfo() : this(0, default, 0, default, 0, OrfState.Internal, default)
        {
        }

        /// <summary>
        /// 配列の文字列を取得します。
        /// </summary>
        /// <returns>配列を表す文字列</returns>
        /// <remarks><see cref="Sequence"/>を直接文字列に変換するのではなく，こちらを呼び出すこと</remarks>
        public ReadOnlySpan<char> GetSequenceString()
        {
            if (Length == 0) return default;

            ReadOnlySpan<AminoAcid> span = Sequence.Span;
            var array = new char[Length];
            for (int i = 0; i < Sequence.Length - 1; i++) array[i] = span[i].SingleName;
            if (!State.HasFlag(OrfState.Partial3)) array[^1] = AminoAcid.End.SingleName;
            else array[^1] = span[^1].SingleName;
            return array;
        }

        /// <summary>
        /// 配列データを出力します。
        /// </summary>
        /// <param name="writer">出力先</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="System.IO.IOException">I/Oエラーが発生した</exception>
        public void WriteSequence(TextWriter writer)
        {
            ArgumentNullException.ThrowIfNull(writer);

            if (Length == 0) return;

            ReadOnlySpan<AminoAcid> span = Sequence.Span;
            for (int i = 0; i < Sequence.Length - 1; i++) writer.Write(span[i].SingleName);
            if (!State.HasFlag(OrfState.Partial3)) writer.Write(AminoAcid.End.SingleName);
            else writer.Write(span[^1].SingleName);
        }
    }
}
