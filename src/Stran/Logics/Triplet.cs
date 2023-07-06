using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Stran.Logics
{
    /// <summary>
    /// トリプレットを表す構造体です。
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Triplet : IEquatable<Triplet>, IComparable<Triplet>, IComparable
    {
        [FieldOffset(0)]
        private readonly NucleotideBase first;

        [FieldOffset(1)]
        private readonly NucleotideBase second;

        [FieldOffset(2)]
        private readonly NucleotideBase third;

        /// <summary>
        /// 一つ目の塩基を取得します。
        /// </summary>
        public NucleotideBase First => first;

        /// <summary>
        /// 二つ目の塩基を取得します。
        /// </summary>
        public NucleotideBase Second => second;

        /// <summary>
        /// 三つ目の塩基を取得します。
        /// </summary>
        public NucleotideBase Third => third;

        /// <summary>
        /// <see cref="Triplet"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="first">一つ目のアミノ酸</param>
        /// <param name="second">二つ目のアミノ酸</param>
        /// <param name="third">三つ目のアミノ酸</param>
        public Triplet(NucleotideBase first, NucleotideBase second, NucleotideBase third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        /// <summary>
        /// <see cref="Triplet"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="sequence">RNA配列</param>
        /// <param name="start"><paramref name="sequence"/>における開始インデックス</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequence"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>が0未満</exception>
        /// <exception cref="ArgumentException"><paramref name="sequence"/>の配列長が不足している</exception>
        public Triplet(NucleotideSequence sequence, int start)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
            if (sequence.Length < start + 3) throw new ArgumentException("配列長が足りません", nameof(sequence));

            first = sequence[start];
            second = sequence[start + 1];
            third = sequence[start + 2];
        }

        /// <summary>
        /// 指定したインデックスの要素を取得します
        /// </summary>
        /// <param name="index">0-2のインデックス</param>
        /// <returns><paramref name="index"/>に対応する要素</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満または2より大きい</exception>
        public NucleotideBase this[int index]
        {
            get
            {
                return index switch
                {
                    0 => First,
                    1 => Second,
                    2 => Third,
                    _ => throw new ArgumentOutOfRangeException(nameof(index))
                };
            }
        }

        /// <summary>
        /// 文字列から<see cref="Triplet"/>の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="value">変換する文字列</param>
        /// <returns><paramref name="value"/>に基づく<see cref="Triplet"/>の新しいインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="value"/>の長さが不足している</exception>
        /// <exception cref="FormatException">書式のエラー</exception>
        public static Triplet Parse(string value) => Parse(value, 0);

        /// <summary>
        /// 文字列から<see cref="Triplet"/>の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="value">変換する文字列</param>
        /// <param name="start"><paramref name="value"/>の変換開始インデックス</param>
        /// <returns><paramref name="value"/>に基づく<see cref="Triplet"/>の新しいインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>が0未満</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/>の長さが不足している</exception>
        /// <exception cref="FormatException">書式のエラー</exception>
        public static Triplet Parse(string value, int start)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
            if (value.Length < start + 3) throw new ArgumentException("文字列長が足りません", nameof(value));

            NucleotideBase first = NucleotideBase.Parse(value[start]);
            NucleotideBase second = NucleotideBase.Parse(value[start + 1]);
            NucleotideBase third = NucleotideBase.Parse(value[start + 2]);
            return new Triplet(first, second, third);
        }

        /// <summary>
        /// 文字列から<see cref="Triplet"/>の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="value">変換する文字列</param>
        /// <param name="triplet"><paramref name="value"/>に基づく<see cref="Triplet"/>の新しいインスタンス</param>
        /// <param name="start"><paramref name="value"/>の変換開始インデックス</param>
        /// <returns><paramref name="triplet"/>を生成できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>が0未満</exception>
        public static bool TryParse(string value, out Triplet triplet, int start)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));

            if (value.Length < start + 3)
            {
                triplet = default;
                return false;
            }
            if (!NucleotideBase.TryParse(value[start], out NucleotideBase first) || !NucleotideBase.TryParse(value[start + 1], out NucleotideBase second) || !NucleotideBase.TryParse(value[start + 2], out NucleotideBase third))
            {
                triplet = default;
                return false;
            }

            triplet = new Triplet(first, second, third);
            return true;
        }

        /// <summary>
        /// AUGCのみからなる組に変換します。
        /// </summary>
        /// <returns>AUGCのみからなる組の一覧</returns>
        public IEnumerable<Triplet> AsAUGC()
        {
            foreach (NucleotideBase first in First.AsAUGC())
                foreach (NucleotideBase second in Second.AsAUGC())
                    foreach (NucleotideBase third in Third.AsAUGC())
                        yield return new Triplet(first, second, third);
        }

        /// <summary>
        /// <see cref="ReadOnlySpan{T}"/>に変換します。
        /// </summary>
        /// <returns><see cref="ReadOnlySpan{T}"/>のインスタンス</returns>
        public ReadOnlySpan<NucleotideBase> AsReadOnlySpan()
        {
            ref Triplet trpRef = ref Unsafe.AsRef(this);
            ref NucleotideBase nucRef = ref Unsafe.As<Triplet, NucleotideBase>(ref trpRef);
            return MemoryMarshal.CreateReadOnlySpan(ref nucRef, 3);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Deconstruct(out NucleotideBase first, out NucleotideBase second, out NucleotideBase third)
        {
            first = this.first;
            second = this.second;
            third = this.third;
        }

        /// <inheritdoc/>
        public int CompareTo(Triplet other)
        {
            int result;
            result = First.CompareTo(other.First);
            if (result != 0) return result;
            result = Second.CompareTo(other.Second);
            if (result != 0) return result;
            result = Third.CompareTo(other.Third);
            return result;
        }

        int IComparable.CompareTo(object? obj) => obj is Triplet other ? CompareTo(other) : throw new ArgumentException("無効な型が渡されました", nameof(obj));

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Triplet trimplet && Equals(trimplet);

        /// <inheritdoc/>
        public bool Equals(Triplet other) => AsReadOnlySpan().SequenceEqual(other.AsReadOnlySpan());

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int result = 0;
            result |= First.value << 0;
            result |= Second.value << 8;
            result |= Third.value << 16;
            return result.GetHashCode();
        }

        /// <summary>
        /// 配列に変換します。
        /// </summary>
        /// <returns><see cref="First"/>，<see cref="Second"/>，<see cref="Third"/>の順に要素を格納した配列</returns>
        public NucleotideBase[] ToArray() => new[] { First, Second, Third };

        /// <summary>
        /// <see cref="NucleotideSequence"/>に変換します。
        /// </summary>
        /// <returns><see cref="NucleotideSequence"/>の新しいインスタンス</returns>
        public NucleotideSequence ToSequence() => NucleotideSequence.FromArrayDirect(ToArray());

        /// <inheritdoc/>
        public override string ToString() => string.Create(null, stackalloc char[3], $"{First}{Second}{Third}");

        /// <inheritdoc cref="System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
        public static bool operator ==(Triplet left, Triplet right) => left.Equals(right);

        /// <inheritdoc cref="System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
        public static bool operator !=(Triplet left, Triplet right) => !(left == right);

        public static implicit operator (NucleotideBase first, NucleotideBase second, NucleotideBase third)(Triplet value) => (value.first, value.second, value.third);

        public static implicit operator Triplet((NucleotideBase first, NucleotideBase second, NucleotideBase third) value) => new Triplet(value.first, value.second, value.third);
    }
}
