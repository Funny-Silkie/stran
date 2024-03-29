﻿using System;
using System.Runtime.InteropServices;

namespace Stran.Logics
{
    /// <summary>
    /// アミノ酸を表す構造体です。
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct AminoAcid : IEquatable<AminoAcid>, IComparable<AminoAcid>, IComparable, ISequenceComponent<AminoAcid>
    {
        internal const byte ValueGap = 0;

        internal const byte ValueA = (byte)'A';
        internal const byte ValueC = (byte)'C';
        internal const byte ValueD = (byte)'D';
        internal const byte ValueE = (byte)'E';
        internal const byte ValueF = (byte)'F';
        internal const byte ValueG = (byte)'G';
        internal const byte ValueH = (byte)'H';
        internal const byte ValueI = (byte)'I';
        internal const byte ValueK = (byte)'K';
        internal const byte ValueL = (byte)'L';
        internal const byte ValueM = (byte)'M';
        internal const byte ValueN = (byte)'N';
        internal const byte ValueP = (byte)'P';
        internal const byte ValueQ = (byte)'Q';
        internal const byte ValueR = (byte)'R';
        internal const byte ValueS = (byte)'S';
        internal const byte ValueT = (byte)'T';
        internal const byte ValueV = (byte)'V';
        internal const byte ValueW = (byte)'W';
        internal const byte ValueY = (byte)'Y';

        internal const byte ValueO = (byte)'O';
        internal const byte ValueU = (byte)'U';

        internal const byte ValueB = (byte)'B';
        internal const byte ValueJ = (byte)'J';
        internal const byte ValueZ = (byte)'Z';

        internal const byte ValueX = (byte)'X';

        internal const byte ValueError = 0;
        internal const byte ValueEnd = (byte)'*';

        /// <summary>
        /// ギャップを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid Gap { get; } = new AminoAcid(ValueGap);

        /// <summary>
        /// Alanineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid A { get; } = new AminoAcid(ValueA);

        /// <summary>
        /// Cysteineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid C { get; } = new AminoAcid(ValueC);

        /// <summary>
        /// Aspartic acidを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid D { get; } = new AminoAcid(ValueD);

        /// <summary>
        /// Gultamic acidを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid E { get; } = new AminoAcid(ValueE);

        /// <summary>
        /// Phenylalanineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid F { get; } = new AminoAcid(ValueF);

        /// <summary>
        /// Glycineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid G { get; } = new AminoAcid(ValueG);

        /// <summary>
        /// Histidineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid H { get; } = new AminoAcid(ValueH);

        /// <summary>
        /// Isoleucineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid I { get; } = new AminoAcid(ValueI);

        /// <summary>
        /// Lysineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid K { get; } = new AminoAcid(ValueK);

        /// <summary>
        /// Leucineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid L { get; } = new AminoAcid(ValueL);

        /// <summary>
        /// Methionineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid M { get; } = new AminoAcid(ValueM);

        /// <summary>
        /// Asparagineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid N { get; } = new AminoAcid(ValueN);

        /// <summary>
        /// Prolineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid P { get; } = new AminoAcid(ValueP);

        /// <summary>
        /// Glutamineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid Q { get; } = new AminoAcid(ValueQ);

        /// <summary>
        /// Arginineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid R { get; } = new AminoAcid(ValueR);

        /// <summary>
        /// Serineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid S { get; } = new AminoAcid(ValueS);

        /// <summary>
        /// Threonineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid T { get; } = new AminoAcid(ValueT);

        /// <summary>
        /// Valineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid V { get; } = new AminoAcid(ValueV);

        /// <summary>
        /// Tryptophanを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid W { get; } = new AminoAcid(ValueW);

        /// <summary>
        /// Tyrosineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid Y { get; } = new AminoAcid(ValueY);

        /// <summary>
        /// Pyrrolysineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid O { get; } = new AminoAcid(ValueO);

        /// <summary>
        /// Selenocysteineを表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid U { get; } = new AminoAcid(ValueU);

        /// <summary>
        /// <see cref="D"/>または<see cref="N"/>を表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid B { get; } = new AminoAcid(ValueB);

        /// <summary>
        /// <see cref="I"/>または<see cref="L"/>を表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid J { get; } = new AminoAcid(ValueJ);

        /// <summary>
        /// <see cref="E"/>または<see cref="Q"/>を表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid Z { get; } = new AminoAcid(ValueZ);

        /// <summary>
        /// 任意のアミノ酸を表すインスタンスを取得します。
        /// </summary>
        public static AminoAcid X { get; } = new AminoAcid(ValueX);

        /// <summary>
        /// 終止コドンを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>*</remarks>
        public static AminoAcid End { get; } = new AminoAcid(ValueEnd);

        static AminoAcid ISequenceComponent<AminoAcid>.Any => X;

        [FieldOffset(0)]
        internal readonly byte value;

        /// <summary>
        /// アミノ酸名を取得します。
        /// </summary>
        public char SingleName => FromValue(value);

        /// <summary>
        /// <see cref="AminoAcid"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="value">ビット値</param>
        private AminoAcid(byte value)
        {
            this.value = value;
        }

        /// <summary>
        /// 指定した文字がアミノ酸を表すかどうかを取得します。
        /// </summary>
        /// <param name="value">文字</param>
        /// <returns><paramref name="value"/>がアミノ酸を表す場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        private static bool IsAaChar(char value)
        {
            return value is '-' or '*' or (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');
        }

        /// <summary>
        /// 名前から<see cref="value"/>に対応する名前を取得します。
        /// </summary>
        /// <param name="name">名前</param>
        /// <returns><paramref name="name"/>に対応するビット値</returns>
        private static byte FromName(char name)
        {
            if (name == '-') return ValueGap;
            if (!IsAaChar(name)) return ValueError;
            return (byte)name;
        }

        /// <summary>
        /// <see cref="value"/>に対応する名前を取得します。
        /// </summary>
        /// <param name="value">ビット値</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/>が無効</exception>
        /// <returns><paramref name="value"/>に対応するアミノ酸名</returns>
        private static char FromValue(byte value)
        {
            if (value == ValueGap) return '-';
            if (value is (not ValueEnd) and (not >= ValueA or not <= ValueZ)) throw new ArgumentOutOfRangeException(nameof(value));
            return (char)value;
        }

        /// <inheritdoc/>
        public static AminoAcid Parse(char name)
        {
            byte value = FromName(name);
            if (value == ValueError) throw new FormatException();
            return new AminoAcid(value);
        }

        /// <inheritdoc/>
        public static bool TryParse(char name, out AminoAcid result)
        {
            byte value = FromName(name);
            if (value == ValueError)
            {
                result = default;
                return false;
            }
            result = new AminoAcid(value);
            return true;
        }

        /// <inheritdoc/>
        public static AminoAcid[] GetValues()
        {
            return new[]
            {
                Gap, A, C, D, E, F, G, H, I, K, L, M, N, P, Q, R, S, T, V, W, Y, Z, X, End,
            };
        }

        /// <inheritdoc/>
        public int CompareTo(AminoAcid obj)
        {
            return value.CompareTo(obj.value);
        }

        int IComparable.CompareTo(object? obj)
        {
            if (obj is AminoAcid other) return CompareTo(other);
            throw new ArgumentException("比較不能な型です", nameof(obj));
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is AminoAcid other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(AminoAcid other) => value == other.value;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(value);

        /// <inheritdoc/>
        public override string ToString() => SingleName.ToString();

        /// <inheritdoc cref="System.Numerics.IAdditionOperators{TSelf, TOther, TResult}"/>
        public static ProteinSequence operator +(AminoAcid left, AminoAcid right) => ProteinSequence.FromArrayDirect(new[] { left, right });

        /// <inheritdoc/>
        public static bool operator ==(AminoAcid left, AminoAcid right) => left.Equals(right);

        /// <inheritdoc/>
        public static bool operator !=(AminoAcid left, AminoAcid right) => !(left == right);
    }
}
