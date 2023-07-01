using System;

namespace Stran.Logics
{
    /// <summary>
    /// ORF情報を表します。
    /// </summary>
    [Serializable]
    public class OrfInfo
    {
        /// <summary>
        /// 翻訳時の読み枠のオフセットを取得します。
        /// </summary>
        public int Offset { get; init; }

        /// <summary>
        /// 読み始めコドンを取得します。
        /// </summary>
        public Triplet StartCodon { get; init; }

        /// <summary>
        /// 核酸配列におけるORF開始インデックスを取得します。
        /// </summary>
        public int StartIndex { get; init; }

        /// <summary>
        /// 読み終わりコドンを取得します。
        /// </summary>
        public Triplet EndCodon { get; init; }

        /// <summary>
        /// 核酸配列におけるORF終了インデックスを取得します。
        /// </summary>
        public int EndIndex { get; init; }

        /// <summary>
        /// 状態を取得します。
        /// </summary>
        public OrfState State { get; init; }

        /// <summary>
        /// アミノ酸配列データを取得します。
        /// </summary>
        public ReadOnlyMemory<AminoAcid> Sequence { get; init; }
    }
}
