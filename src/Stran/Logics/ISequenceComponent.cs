using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Stran.Logics
{
    /// <summary>
    /// 配列を成す構成単位を表します。
    /// </summary>
    /// <typeparam name="TSelf">自身の型</typeparam>
    public interface ISequenceComponent<TSelf> : IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>
        where TSelf : ISequenceComponent<TSelf>
    {
        /// <summary>
        /// ギャップを表すインスタンスを取得します。
        /// </summary>
        static abstract TSelf Gap { get; }

        /// <summary>
        /// 任意の値を表すインスタンスを取得します。
        /// </summary>
        static abstract TSelf Any { get; }

        /// <summary>
        /// 名前を取得します。
        /// </summary>
        char SingleName { get; }

        /// <summary>
        /// 取り得る値一覧を取得します。
        /// </summary>
        /// <returns>値一覧</returns>
        static abstract TSelf[] GetValues();

        /// <summary>
        /// 名前からインスタンスを取得します。
        /// </summary>
        /// <param name="name">使用する名前</param>
        /// <returns><paramref name="name"/>に対応するインスタンス</returns>
        /// <exception cref="FormatException"><paramref name="name"/>が無効</exception>
        static abstract TSelf Parse(char name);

        /// <summary>
        /// 名前からインスタンスを取得します。
        /// </summary>
        /// <param name="name">使用する名前</param>
        /// <param name="result"><paramref name="name"/>に対応するインスタンス</param>
        /// <returns><paramref name="result"/>を取得できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        static abstract bool TryParse(char name, [NotNullWhen(true)] out TSelf? result);
    }
}
