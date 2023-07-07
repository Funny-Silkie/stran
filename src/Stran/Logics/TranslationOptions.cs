using System;
using System.Collections.Generic;

namespace Stran.Logics
{
    /// <summary>
    /// 翻訳処理時のオプションを表します。
    /// </summary>
    /// <param name="Start">開始コドンを取得します。</param>
    /// <param name="AlternativeStart">Alternativeな開始コドンを取得します。</param>
    /// <param name="OutputAllStarts">Alternativeな開始コドンスタートの配列を省略せず全て出力するかどうかを取得します。</param>
    [Serializable]
    public record class TranslationOptions(HashSet<Triplet> Start, HashSet<Triplet> AlternativeStart, bool OutputAllStarts)
    {
        /// <summary>
        /// <see cref="TranslationOptions"/>の新しいインスタンスを初期化します。
        /// </summary>
        public TranslationOptions() : this(new HashSet<Triplet>(), new HashSet<Triplet>(), false)
        {
        }
    }
}
