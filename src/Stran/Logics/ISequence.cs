using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Stran.Logics
{
    /// <summary>
    /// 配列を表します。
    /// </summary>
    public interface ISequence : IEnumerable
    {
        /// <summary>
        /// 配列長を取得します。
        /// </summary>
        int Length { get; }
    }

    /// <summary>
    /// 配列を表します。
    /// </summary>
    /// <typeparam name="TSelf">自身の型</typeparam>
    public interface ISequence<TSelf> : ISequence, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>
        where TSelf : ISequence<TSelf>
    {
        /// <summary>
        /// 文字列から配列に変換します。
        /// </summary>
        /// <param name="value">変換する文字列</param>
        /// <param name="option">変換オプション</param>
        /// <returns>変換後の配列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="option"/>が無効な値</exception>
        /// <exception cref="FormatException"><paramref name="option"/>が<see cref="SequenceParsingOption.Error"/>の時，<paramref name="value"/>に無効な文字があった</exception>
        static abstract TSelf Parse(string value, SequenceParsingOption option);

        /// <summary>
        /// 文字列から配列に変換します。
        /// </summary>
        /// <param name="value">変換する文字列</param>
        /// <param name="sequence">変換後の配列 変換に失敗したら<see langword="null"/></param>
        /// <returns><paramref name="sequence"/>の生成に成功したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <remarks><see cref="Parse(string, SequenceParsingOption)"/>で<see cref="SequenceParsingOption.Error"/>を指定した時の挙動に相当</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>が<see langword="null"/></exception>
        static abstract bool TryParse(string value, [NotNullWhen(true)] out TSelf? sequence);
    }

    /// <summary>
    /// 配列を表します。
    /// </summary>
    /// <typeparam name="TSelf">自身の型</typeparam>
    /// <typeparam name="TComponent">構成要素の型</typeparam>
    public interface ISequence<TSelf, TComponent> : ISequence<TSelf>, IEnumerable<TComponent>
        where TSelf : ISequence<TSelf, TComponent>
    {
        /// <summary>
        /// 空の配列を取得します。
        /// </summary>
        static abstract TSelf Empty { get; }

        /// <summary>
        /// インデックスに対応する要素を取得します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満または<see cref="ISequence.Length"/>以上</exception>
        /// <returns><paramref name="index"/>に対応する要素</returns>
        TComponent this[int index] { get; }

        /// <summary>
        /// 配列からインスタンスを生成します。
        /// </summary>
        /// <param name="array">使用する配列</param>
        /// <returns><paramref name="array"/>によって生成された<typeparamref name="TSelf"/>のインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>がnull</exception>
        internal static abstract TSelf FromArray(TComponent[] array);

        /// <summary>
        /// <see cref="ReadOnlySpan{T}"/>に変換します。
        /// </summary>
        /// <returns><see cref="ReadOnlySpan{T}"/>のインスタンス</returns>
        ReadOnlySpan<TComponent> AsSpan();
    }
}
