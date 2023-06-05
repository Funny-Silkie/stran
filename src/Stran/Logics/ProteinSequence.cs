using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Stran.Logics
{
    /// <summary>
    /// アミノ酸配列を表すクラスです。
    /// </summary>
    [Serializable]
    public sealed class ProteinSequence : IEquatable<ProteinSequence>, IComparable<ProteinSequence>, IComparable, ICloneable, ISequence<ProteinSequence, AminoAcid>
    {
        private AminoAcid[] items;
        private string? sequenceString;

        /// <summary>
        /// 空の配列を取得します。
        /// </summary>
        public static ProteinSequence Empty { get; } = new ProteinSequence(Array.Empty<AminoAcid>());

        /// <inheritdoc/>
        public int Length => items.Length;

        /// <summary>
        /// <see cref="ProteinSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        private ProteinSequence()
        {
            items = default!;
        }

        /// <summary>
        /// <see cref="ProteinSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">コピー元の配列</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>が<see langword="null"/></exception>
        public ProteinSequence(AminoAcid[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            items = (AminoAcid[])source.Clone();
        }

        /// <summary>
        /// <see cref="ProteinSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">コピー元のコレクション</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>が<see langword="null"/></exception>
        public ProteinSequence(IEnumerable<AminoAcid> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            items = source.ToArray();
        }

        /// <summary>
        /// <see cref="ProteinSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="item">連続させるアミノ酸</param>
        /// <param name="count">配列長</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/>が0未満</exception>
        public ProteinSequence(AminoAcid item, int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (count == 0) items = Array.Empty<AminoAcid>();
            else
            {
                items = new AminoAcid[count];
                Array.Fill(items, item);
            }
        }

        /// <summary>
        /// <see cref="ProteinSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="array">コピー元の配列</param>
        /// <param name="start"><paramref name="array"/>におけるコピー開始インデックス</param>
        /// <param name="count">コピーする要素数</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>・<paramref name="count"/>のどちらかが0未満</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/>と<paramref name="count"/>の示すインデックスが<paramref name="array"/>を超える</exception>
        public ProteinSequence(AminoAcid[] array, int start, int count)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (start + count > array.Length) throw new ArgumentException("配列長が不足しています", nameof(array));
            if (count == 0) items = Array.Empty<AminoAcid>();
            else
            {
                items = new AminoAcid[count];
                Array.Copy(array, start, items, 0, count);
            }
        }

        /// <summary>
        /// <see cref="ProteinSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="value">コピー元</param>
        public ProteinSequence(ReadOnlySpan<AminoAcid> value)
        {
            items = value.ToArray();
        }

        /// <inheritdoc/>
        public AminoAcid this[int index]
        {
            get
            {
                if (index < 0 || Length <= index) throw new ArgumentOutOfRangeException(nameof(index));
                return items[index];
            }
        }

        public AminoAcid this[Index index] => items[index];

        public ProteinSequence this[Range range] => FromArrayDirect(items[range]);

        /// <summary>
        /// 配列を結合します。
        /// </summary>
        /// <param name="seq0">最初に結合する配列</param>
        /// <param name="seq1">次に結合する配列</param>
        /// <returns>結合後の配列</returns>
        public static ProteinSequence Concat(ProteinSequence? seq0, ProteinSequence? seq1)
        {
            if (IsNullOrEmpty(seq0)) return seq1 ?? Empty;
            if (IsNullOrEmpty(seq1)) return seq0;
            var array = new AminoAcid[seq0.Length + seq1.Length];
            Array.Copy(seq0.items, 0, array, 0, seq0.Length);
            Array.Copy(seq1.items, 0, array, seq0.Length, seq1.Length);
            return FromArrayDirect(array);
        }

        /// <summary>
        /// 配列を結合します。
        /// </summary>
        /// <param name="values">結合する配列のリスト</param>
        /// <returns>結合後の配列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/>が<see langword="null"/></exception>
        public static ProteinSequence Concat(params ProteinSequence?[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) return Empty;
            if (values.Length == 1) return values[0] ?? Empty;
            if (values.Length == 2) return Concat(values[0], values[1]);
            var totalLength = 0;
            for (int i = 0; i < values.Length; i++)
            {
                ProteinSequence? current = values[i];
                if (!IsNullOrEmpty(current))
                    totalLength += current.Length;
            }

            var array = new AminoAcid[totalLength];
            var position = 0;
            for (int i = 0; i < values.Length; i++)
            {
                ProteinSequence? current = values[i];
                if (IsNullOrEmpty(current)) continue;
                AminoAcid[] source = current.items;
                Array.Copy(source, 0, array, position, source.Length);
                position += source.Length;
            }
            return FromArrayDirect(array);
        }

        /// <summary>
        /// 配列を結合します。
        /// </summary>
        /// <param name="values">結合する配列のコレクション</param>
        /// <returns>結合された配列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/>が<see langword="null"/></exception>
        public static ProteinSequence Concat(IEnumerable<ProteinSequence?> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values is ProteinSequence[] ar) return Concat(ar);
            var list = new List<AminoAcid[]>();
            var totalLength = 0L;
            foreach (ProteinSequence? current in values)
            {
                if (IsNullOrEmpty(current)) continue;
                list.Add(current.items);
                totalLength += current.items.Length;
            }
            if (totalLength == 0) return Empty;
            var array = new AminoAcid[totalLength];
            var position = 0;
            for (int i = 0; i < list.Count; i++)
            {
                AminoAcid[] source = list[i];
                Array.Copy(source, 0, array, position, source.Length);
                position += source.Length;
            }
            return FromArrayDirect(array);
        }

        /// <summary>
        /// 2つの配列の等価性を検証します。
        /// </summary>
        /// <param name="left">検証する配列</param>
        /// <param name="right">検証する配列</param>
        /// <returns><paramref name="left"/>と<paramref name="right"/>の等価性が認められたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static unsafe bool Equals(ProteinSequence? left, ProteinSequence? right)
        {
            if (left is null) return right is null;
            if (right is null) return false;
            if (left.Length != right.Length) return false;

            int width = sizeof(Vector<byte>) / sizeof(byte);
            int count = left.Length / width;
            int offset = width * count;
            int rem = left.Length - count;

            fixed (void* ptrLeft = left.items)
            {
                fixed (void* ptrRight = right.items)
                {
                    var vPtrLeft = (Vector<byte>*)ptrLeft;
                    var vPtrRight = (Vector<byte>*)ptrRight;

                    for (int i = 0; i < count; i++)
                        if (vPtrLeft[i] != vPtrRight[i])
                            return false;

                    for (int i = 0; i < rem; i++)
                        if (((byte*)vPtrLeft)[offset + i] != ((byte*)vPtrRight)[offset + i])
                            return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 2つの配列の等価性を検証します。
        /// </summary>
        /// <param name="left">検証する配列</param>
        /// <param name="leftStart"><paramref name="left"/>における比較開始インデックス</param>
        /// <param name="right">検証する配列</param>
        /// <param name="rightStart"><paramref name="right"/>における比較開始インデックス</param>
        /// <param name="count">比較する要素数</param>
        /// <exception cref="ArgumentException"><paramref name="left"/>または<paramref name="right"/>の要素数が不足している</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="leftStart"/>が<paramref name="left"/>の範囲外
        /// -または-
        /// <paramref name="rightStart"/>が<paramref name="right"/>の範囲外
        /// -または-
        /// <paramref name="count"/>が0未満
        /// </exception>
        /// <returns><paramref name="left"/>と<paramref name="right"/>の等価性が認められたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool Equals(ProteinSequence? left, int leftStart, ProteinSequence? right, int rightStart, int count)
        {
            if (left is null) return right is null;
            if (right is null) return false;
            if (leftStart < 0 || left.Length <= leftStart) throw new ArgumentOutOfRangeException(nameof(leftStart));
            if (rightStart < 0 || right.Length <= rightStart) throw new ArgumentOutOfRangeException(nameof(rightStart));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (leftStart + count > left.Length) throw new ArgumentException("配列長が足りません", nameof(left));
            if (rightStart + count > right.Length) throw new ArgumentException("配列長が足りません", nameof(right));
            return EqualsPrivate(left, leftStart, right, rightStart, count);
        }

        /// <summary>
        /// 2つの配列の等価性を検証します。
        /// </summary>
        /// <param name="left">検証する配列</param>
        /// <param name="leftStart"><paramref name="left"/>における比較開始インデックス</param>
        /// <param name="right">検証する配列</param>
        /// <param name="rightStart"><paramref name="right"/>における比較開始インデックス</param>
        /// <param name="count">比較する要素数</param>
        /// <returns><paramref name="left"/>と<paramref name="right"/>の等価性が認められたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <remarks>
        /// 引数チェックを行いません。引数チェックを行う場合は<see cref="Equals(ProteinSequence, int, ProteinSequence, int, int)"/>を使用します。
        /// </remarks>
        private static unsafe bool EqualsPrivate(ProteinSequence? left, int leftStart, ProteinSequence? right, int rightStart, int count)
        {
            if (left is null) return right is null;
            if (right is null) return false;
            if (count == 0) return true;

            fixed (void* _ptrLeft = left.items)
            {
                fixed (void* _ptrRight = right.items)
                {
                    var ptrLeft = (byte*)_ptrLeft;
                    var ptrRight = (byte*)_ptrRight;

                    for (int i = 0; i < count; i++)
                        if (ptrLeft[i + leftStart] != ptrRight[i + rightStart])
                            return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 配列が全てギャップかどうかを検証します。
        /// </summary>
        /// <param name="array">検証する配列</param>
        /// <returns><paramref name="array"/>が全てギャップだったら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        private static unsafe bool IsGap(AminoAcid[] array)
        {
            Vector<byte> zero = Vector<byte>.Zero;
            int width = sizeof(Vector<byte>) / sizeof(byte);
            int count = array.Length / width;
            int offset = width * count;
            int rem = array.Length - count;

            fixed (void* ptr = array)
            {
                var vPtr = (Vector<byte>*)ptr;

                for (int i = 0; i < count; i++)
                    if (vPtr[i] != zero)
                        return false;

                for (int i = 0; i < rem; i++)
                    if (((byte*)vPtr)[offset + i] != 0)
                        return false;
            }
            return true;
        }

        /// <summary>
        /// 配列から<see cref="ProteinSequence"/>のインスタンスを生成します。
        /// </summary>
        /// <param name="array"></param>
        /// <returns><paramref name="array"/>に基づく<see cref="ProteinSequence"/>の新しいインスタンス</returns>
        /// <remarks>パフォーマンス向上のために引数の配列を直接フィールドに代入。使用の際はImmutableを崩さないように要注意</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>が<see langword="null"/></exception>
        internal static ProteinSequence FromArrayDirect(AminoAcid[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            return new ProteinSequence
            {
                items = array,
            };
        }

        /// <summary>
        /// 配列が<see langword="null"/>または空であるかどうかを検証します。
        /// </summary>
        /// <param name="value">検証する配列</param>
        /// <returns><paramref name="value"/>が<see langword="null"/>または空で<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool IsNullOrEmpty([NotNullWhen(false)] ProteinSequence? value) => value == null || value.Length == 0;

        /// <summary>
        /// 配列が<see langword="null"/>または空，<see cref="AminoAcid.Gap"/>のみからなるかどうかを検証します。
        /// </summary>
        /// <param name="value">検証する配列</param>
        /// <returns><paramref name="value"/>が<see langword="null"/>または空，<see cref="AminoAcid.Gap"/>のみからなる場合で<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool IsNullOrGap([NotNullWhen(false)] ProteinSequence? value)
        {
            if (IsNullOrEmpty(value)) return true;
            return IsGap(value.items);
        }

        /// <summary>
        /// 文字列から配列に変換します。
        /// </summary>
        /// <param name="value">変換する文字列</param>
        /// <param name="option">変換オプション</param>
        /// <returns>変換後の配列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="option"/>が無効な値</exception>
        /// <exception cref="FormatException"><paramref name="option"/>が<see cref="SequenceParsingOption.Error"/>の時，<paramref name="value"/>に無効な文字があった</exception>
        public static ProteinSequence Parse(string value, SequenceParsingOption option = SequenceParsingOption.Error)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length == 0) return Empty;

            AminoAcid[] array;
            string? seqString;
            switch (option)
            {
                case SequenceParsingOption.Error:
                    array = new AminoAcid[value.Length];
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (AminoAcid.TryParse(value[i], out AminoAcid b)) array[i] = b;
                        else throw new FormatException($"{i}番目のアミノ酸\"{value[i]}\"が無効です");
                    }
                    seqString = value;
                    break;

                case SequenceParsingOption.Skip:
                    var list = new List<AminoAcid>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        if (AminoAcid.TryParse(value[i], out AminoAcid b))
                            list.Add(b);
                    array = list.ToArray();
                    seqString = list.Count == value.Length ? value : null;
                    break;

                case SequenceParsingOption.Gap:
                    array = new AminoAcid[value.Length];
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (AminoAcid.TryParse(value[i], out AminoAcid b)) array[i] = b;
                        else array[i] = AminoAcid.Gap;
                    }
                    seqString = value;
                    break;

                case SequenceParsingOption.Any:
                    array = new AminoAcid[value.Length];
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (AminoAcid.TryParse(value[i], out AminoAcid b)) array[i] = b;
                        else array[i] = AminoAcid.X;
                    }
                    seqString = value;
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(option));
            }
            ProteinSequence result = FromArrayDirect(array);
            result.sequenceString = seqString;
            return result;
        }

        /// <summary>
        /// 文字列から配列に変換します。
        /// </summary>
        /// <param name="value">変換する文字列</param>
        /// <param name="sequence">変換後の配列 変換に失敗したら<see langword="null"/></param>
        /// <returns><paramref name="sequence"/>の生成に成功したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <remarks><see cref="Parse(string, SequenceParsingOption)"/>で<see cref="SequenceParsingOption.Error"/>を指定した時の挙動に相当</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>が<see langword="null"/></exception>
        public static bool TryParse(string value, [NotNullWhen(true)] out ProteinSequence? sequence)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length == 0)
            {
                sequence = Empty;
                return true;
            }
            var array = new AminoAcid[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if (!AminoAcid.TryParse(value[i], out AminoAcid b))
                {
                    sequence = null;
                    return false;
                }
                array[i] = b;
            }
            sequence = FromArrayDirect(array);
            sequence.sequenceString = value;
            return true;
        }

        /// <summary>
        /// <see cref="ReadOnlySpan{T}"/>に変換します。
        /// </summary>
        /// <returns><see cref="ReadOnlySpan{T}"/>のインスタンス</returns>
        public ReadOnlySpan<AminoAcid> AsReadOnlySpan() => new ReadOnlySpan<AminoAcid>(items);

        /// <summary>
        /// インスタンスの複製を生成します。
        /// </summary>
        /// <returns>インスタンスの複製</returns>
        public ProteinSequence Clone() => new ProteinSequence
        {
            items = (AminoAcid[])items.Clone(),
            sequenceString = sequenceString,
        };

        object ICloneable.Clone() => Clone();

        /// <inheritdoc/>
        public int CompareTo(ProteinSequence? obj)
        {
            if (obj is null) return 1;
            for (int i = 0; i < Math.Min(Length, obj.Length); i++)
            {
                int result = items[i].CompareTo(obj.items[i]);
                if (result != 0) return result;
            }
            return Length.CompareTo(obj.Length);
        }

        int IComparable.CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is not ProteinSequence other) throw new ArgumentException("比較不能な型です", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 指定したアミノ酸が存在するかどうかを検索します。
        /// </summary>
        /// <param name="item">検索するアミノ酸</param>
        /// <returns><paramref name="item"/>が存在したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool Contains(AminoAcid item) => IndexOf(item) >= 0;

        /// <summary>
        /// 指定したアミノ酸で終了するかどうかを検証します。
        /// </summary>
        /// <param name="item">検証するアミノ酸</param>
        /// <returns><paramref name="item"/>で終了する場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool EndWith(AminoAcid item) => Length >= 1 && items[^1] == item;

        /// <summary>
        /// 指定した配列で終了するかどうかを検証します。
        /// </summary>
        /// <param name="sequence">検証する配列</param>
        /// <returns><paramref name="sequence"/>で終了する場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="sequence"/>が<see langword="null"/></exception>
        public bool EndWith(ProteinSequence? sequence)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (sequence.Length == 0) return true;
            if (Length < sequence.Length) return false;

            return EqualsPrivate(this, Length - sequence.Length, sequence, 0, sequence.Length);
        }

        /// <inheritdoc/>
        public bool Equals(ProteinSequence? other) => Equals(this, other);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as ProteinSequence);

        /// <summary>
        /// 列挙をサポートするオブジェクトを取得します。
        /// </summary>
        /// <returns>列挙をサポートするオブジェクト</returns>
        public SequenceEnumerator<ProteinSequence, AminoAcid> GetEnumerator() => new SequenceEnumerator<ProteinSequence, AminoAcid>(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<AminoAcid> IEnumerable<AminoAcid>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <summary>
        /// 指定した範囲の配列を生成します。
        /// </summary>
        /// <param name="start">開始インデックス</param>
        /// <param name="count">配列長</param>
        /// <returns>指定した範囲の配列のインスタンス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>または<paramref name="count"/>が0未満 -または- <paramref name="start"/>と<paramref name="count"/>の示すインデックスが配列長を超える</exception>
        public ProteinSequence GetRange(int start, int count)
        {
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (start + count > Length) throw new ArgumentOutOfRangeException(nameof(start), "最終インデックスが配列長を超えます");
            if (count == 0) return Empty;
            if (count == Length) return this;

            AminoAcid[] array = GetRangeInternal(start, count);
            return FromArrayDirect(array);
        }

        /// <summary>
        /// 指定した範囲の配列を生成します。
        /// </summary>
        /// <param name="start">開始インデックス</param>
        /// <param name="count">配列長</param>
        /// <returns>指定した範囲の配列のインスタンス</returns>
        /// <remarks>引数チェックを行いません。</remarks>
        public AminoAcid[] GetRangeInternal(int start, int count)
        {
            var result = new AminoAcid[count];
            Array.Copy(items, start, result, 0, count);
            return result;
        }

        /// <summary>
        /// ギャップが全て取り除かれた配列を取得します。
        /// </summary>
        /// <returns>ギャップが全て取り除かれた配列</returns>
        public ProteinSequence GetGapless()
        {
            byte[] bytes = Unsafe.As<AminoAcid[], byte[]>(ref items);
            int totalLength = bytes.Count(x => x != AminoAcid.ValueGap);

            if (totalLength == 0) return Empty;
            if (totalLength == Length) return this;

            byte[] result = new byte[totalLength];
            int srcIndex = 0;
            int dstIndex = 0;
            while (true)
            {
                srcIndex = Array.IndexOf(bytes, AminoAcid.ValueGap, srcIndex);
                if (srcIndex == Length - 1)
                {
                    result[dstIndex] = bytes[srcIndex];
                    break;
                }

                int lastIndex = Array.IndexOf(bytes, AminoAcid.ValueGap, srcIndex);
                if (lastIndex == -1 || lastIndex == Length - 1)
                {
                    Array.Copy(bytes, srcIndex, result, dstIndex, totalLength - srcIndex);
                    break;
                }
                if (srcIndex < 0) break;

                int partialLength = lastIndex - srcIndex + 1;
                Array.Copy(bytes, srcIndex, result, dstIndex, partialLength);
                dstIndex += partialLength;
            }
            return FromArrayDirect(Unsafe.As<byte[], AminoAcid[]>(ref bytes));
        }

        /// <summary>
        /// 逆配列を生成します。
        /// </summary>
        /// <returns>逆配列</returns>
        public ProteinSequence GetReverse()
        {
            if (Length <= 1) return this;
            var array = (AminoAcid[])items.Clone();
            Array.Reverse(array);
            return FromArrayDirect(array);
        }

        /// <summary>
        /// 指定したアミノ酸のうち最初のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索するアミノ酸</param>
        /// <returns><paramref name="item"/>の最初のインデックス</returns>
        public int IndexOf(AminoAcid item) => Array.IndexOf(items, item);

        /// <summary>
        /// 指定したアミノ酸のうち最初のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索するアミノ酸</param>
        /// <param name="start">検索開始インデックス</param>
        /// <returns><paramref name="item"/>の最初のインデックス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>が0未満または<see cref="Length"/>以上</exception>
        public int IndexOf(AminoAcid item, int start) => Array.IndexOf(items, item, start);

        /// <summary>
        /// 指定したアミノ酸のうち最初のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索するアミノ酸</param>
        /// <param name="start">検索開始インデックス</param>
        /// <param name="count">検索する要素数</param>
        /// <returns><paramref name="item"/>の最初のインデックス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>または<paramref name="count"/>が0未満 -または- <paramref name="start"/>と<paramref name="count"/>の示すインデックスが配列長を超える</exception>
        public int IndexOf(AminoAcid item, int start, int count) => Array.IndexOf(items, item, start, count);

        /// <summary>
        /// 指定したアミノ酸のうち最後のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索するアミノ酸</param>
        /// <returns><paramref name="item"/>の最後のインデックス</returns>
        public int LastIndexOf(AminoAcid item) => Array.LastIndexOf(items, item);

        /// <summary>
        /// 指定したアミノ酸のうち最後のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索するアミノ酸</param>
        /// <param name="start">検索開始インデックス</param>
        /// <returns><paramref name="item"/>の最後のインデックス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>が0未満または<see cref="Length"/>以上</exception>
        public int LastIndexOf(AminoAcid item, int start) => Array.LastIndexOf(items, item, start);

        /// <summary>
        /// 指定したアミノ酸のうち最後のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索するアミノ酸</param>
        /// <param name="start">検索開始インデックス</param>
        /// <param name="count">検索する要素数</param>
        /// <returns><paramref name="item"/>の最後のインデックス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>または<paramref name="count"/>が0未満 -または- <paramref name="start"/>と<paramref name="count"/>の示すインデックスが配列長を超える</exception>
        public int LastIndexOf(AminoAcid item, int start, int count) => Array.LastIndexOf(items, item, start, count);

        /// <summary>
        /// 指定したアミノ酸で開始するかどうかを検証します。
        /// </summary>
        /// <param name="item">検証するアミノ酸</param>
        /// <returns><paramref name="item"/>で開始する場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool StartWith(AminoAcid item) => Length >= 1 && items[0] == item;

        /// <summary>
        /// 指定した配列で開始するかどうかを検証します。
        /// </summary>
        /// <param name="sequence">検証する配列</param>
        /// <returns><paramref name="sequence"/>で開始する場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="sequence"/>が<see langword="null"/></exception>
        public bool StartWith(ProteinSequence sequence)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (sequence.Length == 0) return true;
            if (Length < sequence.Length) return false;
            return Equals(this, 0, sequence, 0, sequence.Length);
        }

        /// <summary>
        /// 配列に変換します。
        /// </summary>
        /// <returns>配列</returns>
        public AminoAcid[] ToArray() => (AminoAcid[])items.Clone();

        /// <inheritdoc/>
        public override string ToString()
        {
            if (sequenceString == null)
            {
                var builder = new StringBuilder(Length);
                for (int i = 0; i < Length; i++) builder.Append(items[i]);
                sequenceString = builder.ToString();
            }
            return sequenceString;
        }

        /// <summary>
        /// 先頭と末尾のギャップを取り除きます。
        /// </summary>
        /// <returns>先頭と末尾のギャップを除去された配列</returns>
        public ProteinSequence Trim()
        {
            if (Length == 0) return Empty;

            int start = 0;
            for (int i = 0; i < Length; i++)
            {
                if (items[i] == AminoAcid.Gap) start = i;
                else break;
            }
            if (start == Length - 1) return Empty;
            int end = Length - 1;
            for (int i = Length - 1; i >= start; i--)
            {
                if (items[i] == AminoAcid.Gap) end = i;
                else break;
            }
            if (start == 0 && end == Length - 1) return this;
            return FromArrayDirect(items[new Range(start, (end + 1))]);
        }

        /// <summary>
        /// 末尾のギャップを取り除きます。
        /// </summary>
        /// <returns>末尾のギャップを除去された配列</returns>
        public ProteinSequence TrimEnd()
        {
            if (Length == 0) return Empty;

            int end = Length - 1;
            for (int i = Length - 1; i >= 0; i--)
            {
                if (items[i] == AminoAcid.Gap) end = i;
                else break;
            }
            if (end == Length - 1) return this;
            return FromArrayDirect(items[..end]);
        }

        /// <summary>
        /// 先頭のギャップを取り除きます。
        /// </summary>
        /// <returns>先頭のギャップを除去された配列</returns>
        public ProteinSequence TrimStart()
        {
            if (Length == 0) return Empty;

            int start = 0;
            for (int i = 0; i < Length; i++)
            {
                if (items[i] == AminoAcid.Gap) start = i;
                else break;
            }
            if (start == Length - 1) return Empty;
            if (start == 0) return this;
            return FromArrayDirect(items[start..]);
        }

        /// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}"/>
        public static ProteinSequence operator +(ProteinSequence? left, ProteinSequence? right) => Concat(left, right);

        /// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}"/>
        public static ProteinSequence operator +(ProteinSequence? left, AminoAcid right)
        {
            if (IsNullOrEmpty(left)) return new ProteinSequence(right, 1);
            var array = new AminoAcid[left.Length + 1];
            Array.Copy(left.items, 0, array, 0, left.Length);
            array[^1] = right;
            return FromArrayDirect(array);
        }

        /// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}"/>
        public static ProteinSequence operator +(AminoAcid left, ProteinSequence? right) => right + left;

        /// <inheritdoc/>
        public static bool operator ==(ProteinSequence? left, ProteinSequence? right) => Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(ProteinSequence? left, ProteinSequence? right) => !(left == right);
    }
}
