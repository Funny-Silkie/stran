using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Stran.Logics
{
    /// <summary>
    /// 核酸配列を表すクラスです。
    /// </summary>
    [Serializable]
    public sealed class NucleotideSequence : IEquatable<NucleotideSequence>, IComparable<NucleotideSequence>, IComparable, ICloneable, ISequence<NucleotideSequence, NucleotideBase>
    {
        private NucleotideBase[] items;
        private string? sequenceString;

        /// <summary>
        /// 空の配列を取得します。
        /// </summary>
        public static NucleotideSequence Empty { get; } = new NucleotideSequence(Array.Empty<NucleotideBase>());

        /// <inheritdoc/>
        public int Length => items.Length;

        /// <summary>
        /// <see cref="NucleotideSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        private NucleotideSequence()
        {
            items = default!;
        }

        /// <summary>
        /// <see cref="NucleotideSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">コピー元の配列</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>が<see langword="null"/></exception>
        public NucleotideSequence(NucleotideBase[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            items = (NucleotideBase[])source.Clone();
        }

        /// <summary>
        /// <see cref="NucleotideSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">コピー元のコレクション</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>が<see langword="null"/></exception>
        public NucleotideSequence(IEnumerable<NucleotideBase> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            items = source.ToArray();
        }

        /// <summary>
        /// <see cref="NucleotideSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="item">連続させる塩基</param>
        /// <param name="count">配列長</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/>が0未満</exception>
        public NucleotideSequence(NucleotideBase item, int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (count == 0) items = Array.Empty<NucleotideBase>();
            else
            {
                items = new NucleotideBase[count];
                Array.Fill(items, item);
            }
        }

        /// <summary>
        /// <see cref="NucleotideSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="array">コピー元の配列</param>
        /// <param name="start"><paramref name="array"/>におけるコピー開始インデックス</param>
        /// <param name="count">コピーする要素数</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>・<paramref name="count"/>のどちらかが0未満</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/>と<paramref name="count"/>の示すインデックスが<paramref name="array"/>を超える</exception>
        public NucleotideSequence(NucleotideBase[] array, int start, int count)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (start + count > array.Length) throw new ArgumentException("配列長が不足しています", nameof(array));
            if (count == 0) items = Array.Empty<NucleotideBase>();
            else
            {
                items = new NucleotideBase[count];
                Array.Copy(array, start, items, 0, count);
            }
        }

        /// <summary>
        /// <see cref="NucleotideSequence"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="value">コピー元</param>
        public NucleotideSequence(ReadOnlySpan<NucleotideBase> value)
        {
            items = value.ToArray();
        }

        /// <inheritdoc/>
        public NucleotideBase this[int index]
        {
            get
            {
                if (index < 0 || Length <= index) throw new ArgumentOutOfRangeException(nameof(index));
                return items[index];
            }
        }

        public NucleotideBase this[Index index] => items[index];

        public NucleotideSequence this[Range range] => FromArrayDirect(items[range]);

        /// <summary>
        /// 配列を結合します。
        /// </summary>
        /// <param name="seq0">最初に結合する配列</param>
        /// <param name="seq1">次に結合する配列</param>
        /// <returns>結合後の配列</returns>
        public static NucleotideSequence Concat(NucleotideSequence? seq0, NucleotideSequence? seq1)
        {
            if (IsNullOrEmpty(seq0)) return seq1 ?? Empty;
            if (IsNullOrEmpty(seq1)) return seq0;
            var array = new NucleotideBase[seq0.Length + seq1.Length];
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
        public static NucleotideSequence Concat(params NucleotideSequence?[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length == 0) return Empty;
            if (values.Length == 1) return values[0] ?? Empty;
            if (values.Length == 2) return Concat(values[0], values[1]);
            var totalLength = 0;
            for (int i = 0; i < values.Length; i++)
            {
                NucleotideSequence? current = values[i];
                if (!IsNullOrEmpty(current))
                    totalLength += current.Length;
            }

            var array = new NucleotideBase[totalLength];
            var position = 0;
            for (int i = 0; i < values.Length; i++)
            {
                NucleotideSequence? current = values[i];
                if (IsNullOrEmpty(current)) continue;
                NucleotideBase[] source = current.items;
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
        public static NucleotideSequence Concat(IEnumerable<NucleotideSequence?> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values is NucleotideSequence[] ar) return Concat(ar);
            var list = new List<NucleotideBase[]>();
            var totalLength = 0L;
            foreach (NucleotideSequence? current in values)
            {
                if (IsNullOrEmpty(current)) continue;
                list.Add(current.items);
                totalLength += current.items.Length;
            }
            if (totalLength == 0) return Empty;
            var array = new NucleotideBase[totalLength];
            var position = 0;
            for (int i = 0; i < list.Count; i++)
            {
                NucleotideBase[] source = list[i];
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
        public static unsafe bool Equals(NucleotideSequence? left, NucleotideSequence? right)
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
        public static bool Equals(NucleotideSequence? left, int leftStart, NucleotideSequence? right, int rightStart, int count)
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
        /// 引数チェックを行いません。引数チェックを行う場合は<see cref="Equals(NucleotideSequence, int, NucleotideSequence, int, int)"/>を使用します。
        /// </remarks>
        private static unsafe bool EqualsPrivate(NucleotideSequence? left, int leftStart, NucleotideSequence? right, int rightStart, int count)
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
        private static unsafe bool IsGap(NucleotideBase[] array)
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
        /// 配列から<see cref="NucleotideSequence"/>のインスタンスを生成します。
        /// </summary>
        /// <param name="array"></param>
        /// <returns><paramref name="array"/>に基づく<see cref="NucleotideSequence"/>の新しいインスタンス</returns>
        /// <remarks>パフォーマンス向上のために引数の配列を直接フィールドに代入。使用の際はImmutableを崩さないように要注意</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>が<see langword="null"/></exception>
        internal static NucleotideSequence FromArrayDirect(NucleotideBase[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            return new NucleotideSequence
            {
                items = array,
            };
        }

        /// <summary>
        /// 相補的な配列に書き換えます。
        /// </summary>
        /// <param name="array">元となる配列</param>
        /// <returns>相補的な配列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>が<see langword="null"/></exception>
        /// <remarks>元配列が書き換えられるので注意</remarks>
        internal static void ToComplement(NucleotideBase[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            if (array.Length == 0) return;

            for (int i = 0; i < array.Length; i++) array[i] = array[i].Complement;
        }

        /// <summary>
        /// 配列が<see langword="null"/>または空であるかどうかを検証します。
        /// </summary>
        /// <param name="value">検証する配列</param>
        /// <returns><paramref name="value"/>が<see langword="null"/>または空で<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool IsNullOrEmpty([NotNullWhen(false)] NucleotideSequence? value) => value == null || value.Length == 0;

        /// <summary>
        /// 配列が<see langword="null"/>または空，<see cref="NucleotideBase.Gap"/>のみからなるかどうかを検証します。
        /// </summary>
        /// <param name="value">検証する配列</param>
        /// <returns><paramref name="value"/>が<see langword="null"/>または空，<see cref="NucleotideBase.Gap"/>のみからなる場合で<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool IsNullOrGap([NotNullWhen(false)] NucleotideSequence? value)
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
        public static NucleotideSequence Parse(string value, SequenceParsingOption option = SequenceParsingOption.Error)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length == 0) return Empty;

            NucleotideBase[] array;
            string? seqString;
            switch (option)
            {
                case SequenceParsingOption.Error:
                    array = new NucleotideBase[value.Length];
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (NucleotideBase.TryParse(value[i], out NucleotideBase b)) array[i] = b;
                        else throw new FormatException($"{i}番目の塩基\"{value[i]}\"が無効です");
                    }
                    seqString = value;
                    break;

                case SequenceParsingOption.Skip:
                    var list = new List<NucleotideBase>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        if (NucleotideBase.TryParse(value[i], out NucleotideBase b))
                            list.Add(b);
                    array = list.ToArray();
                    seqString = list.Count == value.Length ? value : null;
                    break;

                case SequenceParsingOption.Gap:
                    array = new NucleotideBase[value.Length];
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (NucleotideBase.TryParse(value[i], out NucleotideBase b)) array[i] = b;
                        else array[i] = NucleotideBase.Gap;
                    }
                    seqString = value;
                    break;

                case SequenceParsingOption.Any:
                    array = new NucleotideBase[value.Length];
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (NucleotideBase.TryParse(value[i], out NucleotideBase b)) array[i] = b;
                        else array[i] = NucleotideBase.N;
                    }
                    seqString = value;
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(option));
            }
            NucleotideSequence result = FromArrayDirect(array);
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
        public static bool TryParse(string value, [NotNullWhen(true)] out NucleotideSequence? sequence)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length == 0)
            {
                sequence = Empty;
                return true;
            }
            var array = new NucleotideBase[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if (!NucleotideBase.TryParse(value[i], out NucleotideBase b))
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
        public ReadOnlySpan<NucleotideBase> AsReadOnlySpan() => new ReadOnlySpan<NucleotideBase>(items);

        /// <summary>
        /// インスタンスの複製を生成します。
        /// </summary>
        /// <returns>インスタンスの複製</returns>
        public NucleotideSequence Clone() => new NucleotideSequence
        {
            items = (NucleotideBase[])items.Clone(),
            sequenceString = sequenceString,
        };

        object ICloneable.Clone() => Clone();

        /// <inheritdoc/>
        public int CompareTo(NucleotideSequence? obj)
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
            if (obj is not NucleotideSequence other) throw new ArgumentException("比較不能な型です", nameof(obj));
            return CompareTo(other);
        }

        /// <summary>
        /// 指定した塩基が存在するかどうかを検索します。
        /// </summary>
        /// <param name="item">検索する塩基</param>
        /// <returns><paramref name="item"/>が存在したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool Contains(NucleotideBase item) => IndexOf(item) >= 0;

        /// <summary>
        /// 指定した塩基で終了するかどうかを検証します。
        /// </summary>
        /// <param name="item">検証する塩基</param>
        /// <returns><paramref name="item"/>で終了する場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool EndWith(NucleotideBase item) => Length >= 1 && items[^1] == item;

        /// <summary>
        /// 指定した配列で終了するかどうかを検証します。
        /// </summary>
        /// <param name="sequence">検証する配列</param>
        /// <returns><paramref name="sequence"/>で終了する場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="sequence"/>が<see langword="null"/></exception>
        public bool EndWith(NucleotideSequence? sequence)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (sequence.Length == 0) return true;
            if (Length < sequence.Length) return false;

            return EqualsPrivate(this, Length - sequence.Length, sequence, 0, sequence.Length);
        }

        /// <inheritdoc/>
        public bool Equals(NucleotideSequence? other) => Equals(this, other);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as NucleotideSequence);

        /// <summary>
        /// 相補的な配列を生成します。
        /// </summary>
        /// <returns>相補的な配列</returns>
        public NucleotideSequence ToComplement()
        {
            NucleotideBase[] array = ToArray();
            ToComplement(array);
            return FromArrayDirect(array);
        }

        /// <summary>
        /// 列挙をサポートするオブジェクトを取得します。
        /// </summary>
        /// <returns>列挙をサポートするオブジェクト</returns>
        public SequenceEnumerator<NucleotideSequence, NucleotideBase> GetEnumerator() => new SequenceEnumerator<NucleotideSequence, NucleotideBase>(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<NucleotideBase> IEnumerable<NucleotideBase>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <summary>
        /// 指定した範囲の配列を生成します。
        /// </summary>
        /// <param name="start">開始インデックス</param>
        /// <param name="count">配列長</param>
        /// <returns>指定した範囲の配列のインスタンス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>または<paramref name="count"/>が0未満 -または- <paramref name="start"/>と<paramref name="count"/>の示すインデックスが配列長を超える</exception>
        public NucleotideSequence GetRange(int start, int count)
        {
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (start + count > Length) throw new ArgumentOutOfRangeException(nameof(start), "最終インデックスが配列長を超えます");
            if (count == 0) return Empty;
            if (count == Length) return this;

            NucleotideBase[] array = GetRangeInternal(start, count);
            return FromArrayDirect(array);
        }

        /// <summary>
        /// 指定した範囲の配列を生成します。
        /// </summary>
        /// <param name="start">開始インデックス</param>
        /// <param name="count">配列長</param>
        /// <returns>指定した範囲の配列のインスタンス</returns>
        /// <remarks>引数チェックを行いません。</remarks>
        public NucleotideBase[] GetRangeInternal(int start, int count)
        {
            var result = new NucleotideBase[count];
            Array.Copy(items, start, result, 0, count);
            return result;
        }

        /// <summary>
        /// ギャップが全て取り除かれた配列を取得します。
        /// </summary>
        /// <returns>ギャップが全て取り除かれた配列</returns>
        public NucleotideSequence GetGapless()
        {
            byte[] bytes = Unsafe.As<NucleotideBase[], byte[]>(ref items);
            int totalLength = bytes.Count(x => x != NucleotideBase.ValueGap);

            if (totalLength == 0) return Empty;
            if (totalLength == Length) return this;

            byte[] result = new byte[totalLength];
            int srcIndex = 0;
            int dstIndex = 0;
            while (true)
            {
                srcIndex = Array.IndexOf(bytes, NucleotideBase.ValueGap, srcIndex);
                if (srcIndex == Length - 1)
                {
                    result[dstIndex] = bytes[srcIndex];
                    break;
                }

                int lastIndex = Array.IndexOf(bytes, NucleotideBase.ValueGap, srcIndex);
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
            return FromArrayDirect(Unsafe.As<byte[], NucleotideBase[]>(ref bytes));
        }

        /// <summary>
        /// 逆配列を生成します。
        /// </summary>
        /// <returns>逆配列</returns>
        public NucleotideSequence GetReverse()
        {
            if (Length <= 1) return this;
            var array = (NucleotideBase[])items.Clone();
            Array.Reverse(array);
            return FromArrayDirect(array);
        }

        /// <summary>
        /// 相補的な逆配列を生成します。
        /// </summary>
        /// <returns>相補的な逆配列</returns>
        public NucleotideSequence GetReverseComplement()
        {
            if (Length <= 1) return this;
            var array = new NucleotideBase[Length];
            for (int i = 0; i < array.Length; i++) array[i] = items[i].Complement;
            Array.Reverse(array);
            return FromArrayDirect(array);
        }

        /// <summary>
        /// 指定した塩基のうち最初のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索する塩基</param>
        /// <returns><paramref name="item"/>の最初のインデックス</returns>
        public int IndexOf(NucleotideBase item) => Array.IndexOf(items, item);

        /// <summary>
        /// 指定した塩基のうち最初のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索する塩基</param>
        /// <param name="start">検索開始インデックス</param>
        /// <returns><paramref name="item"/>の最初のインデックス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>が0未満または<see cref="Length"/>以上</exception>
        public int IndexOf(NucleotideBase item, int start) => Array.IndexOf(items, item, start);

        /// <summary>
        /// 指定した塩基のうち最初のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索する塩基</param>
        /// <param name="start">検索開始インデックス</param>
        /// <param name="count">検索する要素数</param>
        /// <returns><paramref name="item"/>の最初のインデックス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>または<paramref name="count"/>が0未満 -または- <paramref name="start"/>と<paramref name="count"/>の示すインデックスが配列長を超える</exception>
        public int IndexOf(NucleotideBase item, int start, int count) => Array.IndexOf(items, item, start, count);

        /// <summary>
        /// 指定した塩基のうち最後のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索する塩基</param>
        /// <returns><paramref name="item"/>の最後のインデックス</returns>
        public int LastIndexOf(NucleotideBase item) => Array.LastIndexOf(items, item);

        /// <summary>
        /// 指定した塩基のうち最後のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索する塩基</param>
        /// <param name="start">検索開始インデックス</param>
        /// <returns><paramref name="item"/>の最後のインデックス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>が0未満または<see cref="Length"/>以上</exception>
        public int LastIndexOf(NucleotideBase item, int start) => Array.LastIndexOf(items, item, start);

        /// <summary>
        /// 指定した塩基のうち最後のもののインデックスを検索します。
        /// </summary>
        /// <param name="item">検索する塩基</param>
        /// <param name="start">検索開始インデックス</param>
        /// <param name="count">検索する要素数</param>
        /// <returns><paramref name="item"/>の最後のインデックス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>または<paramref name="count"/>が0未満 -または- <paramref name="start"/>と<paramref name="count"/>の示すインデックスが配列長を超える</exception>
        public int LastIndexOf(NucleotideBase item, int start, int count) => Array.LastIndexOf(items, item, start, count);

        /// <summary>
        /// 指定した塩基で開始するかどうかを検証します。
        /// </summary>
        /// <param name="item">検証する塩基</param>
        /// <returns><paramref name="item"/>で開始する場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool StartWith(NucleotideBase item) => Length >= 1 && items[0] == item;

        /// <summary>
        /// 指定した配列で開始するかどうかを検証します。
        /// </summary>
        /// <param name="sequence">検証する配列</param>
        /// <returns><paramref name="sequence"/>で開始する場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="sequence"/>が<see langword="null"/></exception>
        public bool StartWith(NucleotideSequence sequence)
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
        public NucleotideBase[] ToArray() => (NucleotideBase[])items.Clone();

        /// <summary>
        /// トリプレットを取得します。
        /// </summary>
        /// <param name="offset">読み取り開始座位</param>
        /// <returns><paramref name="offset"/>から読み取り開始した際のトリプレット</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>が0未満</exception>
        public unsafe ReadOnlySpan<Triplet> ToTriplet(int offset = 0)
        {
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

            int length = (items.Length - offset) / 3;
            if (length == 0) return Array.Empty<Triplet>();
            fixed (NucleotideBase* ptr = items)
            {
                Triplet* usedPtr = (Triplet*)(ptr + offset);
                ref Triplet rf = ref Unsafe.AsRef<Triplet>(usedPtr);
                ReadOnlySpan<Triplet> span = MemoryMarshal.CreateReadOnlySpan(ref rf, length);
                return span;
            }
        }

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
        public NucleotideSequence Trim()
        {
            if (Length == 0) return Empty;

            int start = 0;
            for (int i = 0; i < Length; i++)
            {
                if (items[i] == NucleotideBase.Gap) start = i;
                else break;
            }
            if (start == Length - 1) return Empty;
            int end = Length - 1;
            for (int i = Length - 1; i >= start; i--)
            {
                if (items[i] == NucleotideBase.Gap) end = i;
                else break;
            }
            if (start == 0 && end == Length - 1) return this;
            return FromArrayDirect(items[new Range(start, (end + 1))]);
        }

        /// <summary>
        /// 末尾のギャップを取り除きます。
        /// </summary>
        /// <returns>末尾のギャップを除去された配列</returns>
        public NucleotideSequence TrimEnd()
        {
            if (Length == 0) return Empty;

            int end = Length - 1;
            for (int i = Length - 1; i >= 0; i--)
            {
                if (items[i] == NucleotideBase.Gap) end = i;
                else break;
            }
            if (end == Length - 1) return this;
            return FromArrayDirect(items[..end]);
        }

        /// <summary>
        /// 先頭のギャップを取り除きます。
        /// </summary>
        /// <returns>先頭のギャップを除去された配列</returns>
        public NucleotideSequence TrimStart()
        {
            if (Length == 0) return Empty;

            int start = 0;
            for (int i = 0; i < Length; i++)
            {
                if (items[i] == NucleotideBase.Gap) start = i;
                else break;
            }
            if (start == Length - 1) return Empty;
            if (start == 0) return this;
            return FromArrayDirect(items[start..]);
        }

        /// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}"/>
        public static NucleotideSequence operator +(NucleotideSequence? left, NucleotideSequence? right) => Concat(left, right);

        /// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}"/>
        public static NucleotideSequence operator +(NucleotideSequence? left, NucleotideBase right)
        {
            if (IsNullOrEmpty(left)) return new NucleotideSequence(right, 1);
            var array = new NucleotideBase[left.Length + 1];
            Array.Copy(left.items, 0, array, 0, left.Length);
            array[^1] = right;
            return FromArrayDirect(array);
        }

        /// <inheritdoc cref="IAdditionOperators{TSelf, TOther, TResult}"/>
        public static NucleotideSequence operator +(NucleotideBase left, NucleotideSequence? right) => right + left;

        /// <inheritdoc/>
        public static bool operator ==(NucleotideSequence? left, NucleotideSequence? right) => Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(NucleotideSequence? left, NucleotideSequence? right) => !(left == right);
    }
}
