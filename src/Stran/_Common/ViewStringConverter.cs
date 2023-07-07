using Stran.Logics;
using System;

namespace Stran
{
    /// <summary>
    /// 表示用の文字列に変換する処理を記述します。
    /// </summary>
    internal static class ViewStringConverter
    {
        /// <summary>
        /// 表示用文字列に変換します。
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>表示用文字列</returns>
        internal static string ToViewString(this Triplet value) => value == default ? "XXX" : value.ToString();

        /// <summary>
        /// 表示用文字列に変換します。
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>表示用文字列</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/>が未定義の値</exception>
        internal static string ToViewString(this OrfState value)
        {
            return value switch
            {
                OrfState.Complete => "complete",
                OrfState.Partial3 => "3'partial",
                OrfState.Partial5 => "5'partial",
                OrfState.Internal => "internal",
                _ => throw new ArgumentOutOfRangeException(nameof(value), "未定義の値です"),
            };
        }

        /// <summary>
        /// 表示用文字列に変換します。
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>表示用文字列</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/>が未定義の値</exception>
        internal static string ToViewString(this SeqStrand value)
        {
            return value switch
            {
                SeqStrand.Plus => "+",
                SeqStrand.Minus => "-",
                _ => throw new ArgumentOutOfRangeException(nameof(value), "未定義の値です"),
            };
        }
    }
}
