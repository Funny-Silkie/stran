using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Stran.Logics
{
    /// <summary>
    /// 核酸の塩基を表す構造体です。
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct NucleotideBase : IEquatable<NucleotideBase>, IComparable<NucleotideBase>, IComparable, ISequenceComponent<NucleotideBase>, IBitwiseOperators<NucleotideBase, NucleotideBase, NucleotideBase>
    {
        internal const byte ValueGap = 0b00000;
        internal const byte ValueA = 0b00001;
        internal const byte ValueU = 0b00010;
        internal const byte ValueG = 0b00100;
        internal const byte ValueC = 0b01000;
        internal const byte ValueN = 0b01111;
        internal const byte ValueError = 0b10000;

        /// <summary>
        /// ギャップを表すインスタンスを取得します。
        /// </summary>
        public static NucleotideBase Gap { get; } = new NucleotideBase(ValueGap);

        /// <summary>
        /// 塩基Aを表すインスタンスを取得します。
        /// </summary>
        public static NucleotideBase A { get; } = new NucleotideBase(ValueA);

        /// <summary>
        /// 塩基Uを表すインスタンスを取得します。
        /// </summary>
        public static NucleotideBase U { get; } = new NucleotideBase(ValueU);

        /// <summary>
        /// 塩基Gを表すインスタンスを取得します。
        /// </summary>
        public static NucleotideBase G { get; } = new NucleotideBase(ValueG);

        /// <summary>
        /// 塩基Cを表すインスタンスを取得します。
        /// </summary>
        public static NucleotideBase C { get; } = new NucleotideBase(ValueC);

        /// <summary>
        /// 塩基Wを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>A+U</remarks>
        public static NucleotideBase W { get; } = A | U;

        /// <summary>
        /// 塩基Rを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>A+G</remarks>
        public static NucleotideBase R { get; } = A | G;

        /// <summary>
        /// 塩基Mを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>A+C</remarks>
        public static NucleotideBase M { get; } = A | C;

        /// <summary>
        /// 塩基Kを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>U+G</remarks>
        public static NucleotideBase K { get; } = U | G;

        /// <summary>
        /// 塩基Yを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>U+C</remarks>
        public static NucleotideBase Y { get; } = U | C;

        /// <summary>
        /// 塩基Sを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>G+C</remarks>
        public static NucleotideBase S { get; } = G | C;

        /// <summary>
        /// 塩基Dを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>A+U+G</remarks>
        public static NucleotideBase D { get; } = A | U | G;

        /// <summary>
        /// 塩基Hを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>A+U+C</remarks>
        public static NucleotideBase H { get; } = A | U | C;

        /// <summary>
        /// 塩基Vを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>A+G+C</remarks>
        public static NucleotideBase V { get; } = A | G | C;

        /// <summary>
        /// 塩基Bを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>U+G+C</remarks>
        public static NucleotideBase B { get; } = U | G | C;

        /// <summary>
        /// 塩基Nを表すインスタンスを取得します。
        /// </summary>
        /// <remarks>A+U+G+C</remarks>
        public static NucleotideBase N { get; } = new NucleotideBase(ValueN);

        static NucleotideBase ISequenceComponent<NucleotideBase>.Any => N;

        [FieldOffset(0)]
        internal readonly byte value;

        /// <summary>
        /// 相補的なインスタンスを取得します。
        /// </summary>
        public NucleotideBase Complement => new NucleotideBase(GetComplement(value));

        char ISequenceComponent<NucleotideBase>.SingleName => Name;

        /// <summary>
        /// 塩基名を取得します。
        /// </summary>
        public char Name => FromValue(value);

        /// <summary>
        /// <see cref="NucleotideBase"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="value">ビット値</param>
        internal NucleotideBase(byte value)
        {
            this.value = value;
        }

        /// <summary>
        /// 名前から<see cref="value"/>に対応する名前を取得します。
        /// </summary>
        /// <param name="name">名前</param>
        /// <returns><paramref name="name"/>に対応するビット値</returns>
        internal static byte FromName(char name)
        {
            return name switch
            {
                '-' => ValueGap,
                'A' or 'a' => ValueA,
                'U' or 'u' or 'T' or 't' => ValueU,
                'G' or 'g' => ValueG,
                'C' or 'c' => ValueC,
                'W' or 'w' => ValueA | ValueU,
                'R' or 'r' => ValueA | ValueG,
                'M' or 'm' => ValueA | ValueC,
                'K' or 'k' => ValueU | ValueG,
                'Y' or 'y' => ValueU | ValueC,
                'S' or 's' => ValueG | ValueC,
                'D' or 'd' => ValueA | ValueU | ValueG,
                'H' or 'h' => ValueA | ValueU | ValueC,
                'V' or 'v' => ValueA | ValueG | ValueC,
                'B' or 'b' => ValueU | ValueG | ValueC,
                'N' or 'n' => ValueN,
                _ => ValueError,
            };
        }

        /// <summary>
        /// <see cref="value"/>に対応する名前を取得します。
        /// </summary>
        /// <param name="value">ビット値</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/>が0b0000～0b1111に収まらない</exception>
        /// <returns><paramref name="value"/>に対応する塩基名</returns>
        internal static char FromValue(byte value)
        {
            return value switch
            {
                ValueGap => '-',
                ValueA => 'A',
                ValueU => 'U',
                ValueG => 'G',
                ValueC => 'C',
                ValueA | ValueU => 'W',
                ValueA | ValueG => 'R',
                ValueA | ValueC => 'M',
                ValueU | ValueG => 'K',
                ValueU | ValueC => 'Y',
                ValueG | ValueC => 'S',
                ValueA | ValueU | ValueG => 'D',
                ValueA | ValueU | ValueC => 'H',
                ValueA | ValueG | ValueC => 'V',
                ValueU | ValueG | ValueC => 'B',
                ValueN => 'N',
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            };
        }

        /// <summary>
        /// 相補的なビット値を取得します。
        /// </summary>
        /// <param name="value">検証するビット値</param>
        /// <returns>相補塩基に相当するビット値</returns>
        internal static byte GetComplement(byte value)
        {
            byte result = ValueGap;
            if ((value & ValueA) == ValueA) result |= ValueU;
            if ((value & ValueU) == ValueU) result |= ValueA;
            if ((value & ValueG) == ValueG) result |= ValueC;
            if ((value & ValueC) == ValueC) result |= ValueG;
            return result;
        }

        /// <summary>
        /// 塩基名をもとに<see cref="NucleotideBase"/>のインスタンスを取得します。
        /// </summary>
        /// <param name="name">塩基名</param>
        /// <returns><paramref name="name"/>に対応する<see cref="NucleotideBase"/>のインスタンス</returns>
        /// <exception cref="FormatException"><paramref name="name"/>が無効</exception>
        public static NucleotideBase Parse(char name)
        {
            byte value = FromName(name);
            if (value == ValueError) throw new FormatException();
            return new NucleotideBase(value);
        }

        /// <summary>
        /// 塩基名をもとに<see cref="NucleotideBase"/>のインスタンスを取得します。
        /// </summary>
        /// <param name="name">塩基名</param>
        /// <param name="result"><paramref name="name"/>に対応する<see cref="NucleotideBase"/>のインスタンス</param>
        /// <returns><paramref name="result"/>を取得出来たら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="FormatException"><paramref name="name"/>が無効</exception>
        public static bool TryParse(char name, out NucleotideBase result)
        {
            byte value = FromName(name);
            if (value == ValueError)
            {
                result = default;
                return false;
            }
            result = new NucleotideBase(value);
            return true;
        }

        /// <inheritdoc/>
        public static NucleotideBase[] GetValues()
        {
            return new[]
            {
                Gap, A, U, G, C ,W, R, M, K, Y, S, D, H, V, B, N,
            };
        }

        /// <summary>
        /// 示す塩基をAUGCの単位で取得します。
        /// </summary>
        /// <returns>AUGCの格納される配列</returns>
        public NucleotideBase[] AsAUGC()
        {
            var list = new List<NucleotideBase>();
            if ((value & ValueA) == ValueA) list.Add(A);
            if ((value & ValueU) == ValueU) list.Add(U);
            if ((value & ValueG) == ValueG) list.Add(G);
            if ((value & ValueC) == ValueC) list.Add(C);
            return list.ToArray();
        }

        /// <inheritdoc/>
        public int CompareTo(NucleotideBase obj)
        {
            return value.CompareTo(obj.value);
        }

        int IComparable.CompareTo(object? obj)
        {
            if (obj is NucleotideBase other) return CompareTo(other);
            throw new ArgumentException("比較不能な型です", nameof(obj));
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is NucleotideBase other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(NucleotideBase other) => value == other.value;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(value);

        /// <inheritdoc/>
        public override string ToString() => Name.ToString();

        /// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}"/>
        public static NucleotideSequence operator +(NucleotideBase left, NucleotideBase right) => new NucleotideSequence(new[] { left, right });

        /// <inheritdoc/>
        public static NucleotideBase operator |(NucleotideBase left, NucleotideBase right) => new NucleotideBase((byte)(left.value | right.value));

        /// <inheritdoc/>
        public static NucleotideBase operator &(NucleotideBase left, NucleotideBase right) => new NucleotideBase((byte)(left.value & right.value));

        /// <inheritdoc/>
        public static NucleotideBase operator ^(NucleotideBase left, NucleotideBase right) => new NucleotideBase((byte)((byte)(left.value ^ right.value) & ValueN));

        /// <inheritdoc/>
        public static NucleotideBase operator ~(NucleotideBase value) => new NucleotideBase((byte)((byte)~value.value & ValueN));

        /// <inheritdoc/>
        public static bool operator ==(NucleotideBase left, NucleotideBase right) => left.Equals(right);

        /// <inheritdoc/>
        public static bool operator !=(NucleotideBase left, NucleotideBase right) => !(left == right);
    }
}
